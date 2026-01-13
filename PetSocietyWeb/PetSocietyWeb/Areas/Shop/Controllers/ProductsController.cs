using ClosedXML.Excel;
using DocumentFormat.OpenXml.Spreadsheet;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PetSocietyWeb.Models.Generated;
using PetSocietyWeb.ViewModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace PetSocietyWeb.Areas.Shop.Controllers
{
    [Area("Shop")]
    public class ProductsController : Controller
    {
        private readonly PetSocietyContext _context;
        private readonly IWebHostEnvironment _hostEnvironment; // ★ 1. 注入環境變數 (為了拿 wwwroot 路徑)

        public ProductsController(PetSocietyContext context, IWebHostEnvironment hostEnvironment)
        {
            _context = context;
            _hostEnvironment = hostEnvironment;
        }

        public async Task<IActionResult> ExportToExcel()
        {
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Member)
                .OrderByDescending(p => p.CreateDate)
                .ToListAsync();
            using (var workbook = new XLWorkbook())
            {
                var worksheet = workbook.Worksheets.Add("商品列表");

                worksheet.Cell(1, 1).Value = "商品編號";
                worksheet.Cell(1, 2).Value = "分類名稱";
                worksheet.Cell(1, 3).Value = "賣家信箱";
                worksheet.Cell(1, 4).Value = "商品名稱";
                worksheet.Cell(1, 5).Value = "單價";
                worksheet.Cell(1, 6).Value = "庫存";
                worksheet.Cell(1, 7).Value = "狀態";
                worksheet.Cell(1, 8).Value = "建立時間";

                var headerRange = worksheet.Range("A1:H1");
                headerRange.Style.Font.Bold = true;
                headerRange.Style.Fill.BackgroundColor = XLColor.LightGray;
                int row = 2;
                foreach (var item in products)
                {
                    worksheet.Cell(row, 1).Value = item.ProductId;
                    worksheet.Cell(row, 2).Value = item.Category?.CategoryName ?? "未分類";
                    worksheet.Cell(row, 3).Value = item.Member?.Email ?? "未知";
                    worksheet.Cell(row, 4).Value = item.ProductName;
                    worksheet.Cell(row, 5).Value = item.Price;
                    worksheet.Cell(row, 6).Value = item.Stock;
                    worksheet.Cell(row, 7).Value = item.Status;
                    worksheet.Cell(row, 8).Value = item.CreateDate;
                    row++;
                }
                worksheet.Columns().AdjustToContents();
                using (var stream = new MemoryStream())
                {
                    workbook.SaveAs(stream);
                    var content = stream.ToArray();


                    string fileName = $"Products_{DateTime.Now:yyyyMMdd_HHmm}.xlsx";

                    return File(content, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
                }

            }

        }



        //GET : Shop/Products/Index
        [HttpGet]
        public IActionResult Index()
        {
            return View(); //Index.cshtml
        }

        //POST : Shop/Products/IndexJson
        // POST: Shop/Products/IndexJson
        [HttpPost]
        public async Task<JsonResult> IndexJson()
        {
            // 1. 撈出所有商品，並關聯圖片 (只撈第一張圖當縮圖即可)
            // 這裡使用 Select 投影成一個匿名物件或是 ViewModel，只回傳列表需要的欄位，減少傳輸量
            var products = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Member) // 如果你需要賣家信箱
                .Select(p => new
                {
                    p.ProductId,
                    CategoryName = p.Category.CategoryName, // 直接拿名稱，不要拿 ID
                    SellerEmail = p.Member.Email,           // 拿賣家信箱，不要拿 MemberId
                    p.ProductName,
                    p.Price,
                    p.Stock,
                    p.Status,
                    p.CreateDate,
                    // ★ 撈取第一張圖片的二進位資料
                    FirstImagePath = _context.ProductImages
                                         .Where(img => img.ProductId == p.ProductId)
                                         .Select(img => img.ImagePath)
                                         .FirstOrDefault()
                })
                .ToListAsync();

            // 2. 處理圖片轉 Base64 (因為 JSON 不能直接傳 byte[])
            // 雖然可以在 SQL 端做，但在記憶體裡做比較簡單直覺
            var resultList = products.Select(p => new
            {
                p.ProductId,
                p.CategoryName,
                p.SellerEmail,
                p.ProductName,
                p.Price,
                p.Stock,
                p.Status,
                p.CreateDate,
                // ★ 修改：如果有路徑，就組合成完整網址給前端
                // 例如: https://localhost:7138/images/products/abc.jpg
                Thumbnail = !string.IsNullOrEmpty(p.FirstImagePath)
                    ? $"{Request.Scheme}://{Request.Host}/{p.FirstImagePath}"
                    : null 
            });

            return Json(resultList);
        }

        // GET: Shop/Products
        //public async Task<IActionResult> Index()
        //{
        //    var petSocietyContext = _context.Products.Include(p => p.Category).Include(p => p.Member);
        //    return View(await petSocietyContext.ToListAsync());
        //}

        // GET: Shop/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Member)
                .FirstOrDefaultAsync(m => m.ProductId == id);
            if (product == null)
            {
                return NotFound();
            }

            // ★ 修改：撈出路徑
            var imagePaths = await _context.ProductImages
                .Where(p => p.ProductId == id)
                .Select(p => p.ImagePath)
                .ToListAsync();

            // ★ 修改：將路徑轉成完整網址
            var imgUrlList = imagePaths
                .Select(path => $"{Request.Scheme}://{Request.Host}/{path}")
                .ToList();

            // ★ 修改：組裝 ViewModel
            var viewModel = new ProductDetailViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category?.CategoryName,
                SellerEmail = product.Member?.Email,
                Status = product.Status,
                CreateDate = product.CreateDate,
                ImageUrlList = imgUrlList
            };

            return View(viewModel);
        }

        // GET: Shop/Products/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "CategoryId", "CategoryName");
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Email");
            return View();
        }

        // POST: Shop/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductCreateViewModel model)
        {
            // ★ 移除了原本手動設定 DateTime.Now 的部分，改在下面建立物件時設定

            if (ModelState.IsValid)
            {
                // ★ 修改：手動將 ViewModel 對應到 Product 實體
                var product = new Product
                {
                    ProductName = model.ProductName,
                    Description = model.Description,
                    Price = model.Price,
                    Stock = model.Stock,
                    CategoryId = model.CategoryId,
                    MemberId = model.MemberId, // 儲存選擇的賣家
                    Status = model.Status,
                    CreateDate = DateTime.Now // 在這裡設定時間
                };

                _context.Add(product);
                await _context.SaveChangesAsync(); // 先存檔以取得 ProductId

                // ★ 修改重點：圖片上傳邏輯
                if (model.Photos != null && model.Photos.Count > 0)
                {
                    // 1. 設定目標路徑：wwwroot/img/shop
                    string wwwRootPath = _hostEnvironment.WebRootPath;
                    // Path.Combine 會自動處理斜線，不用擔心
                    string folderPath = Path.Combine(wwwRootPath, "img", "shop");

                    // 如果資料夾不存在，就自動建立 (防呆)
                    if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                    // 2. 迴圈處理每一張圖片 (這就是多圖上傳的關鍵！)
                    foreach (var file in model.Photos)
                    {
                        if (file.Length > 0)
                        {
                            // 1. 產生一個短的隨機碼 (用 Guid 取前 4 碼，夠亂又不長)
                            string randomCode = Guid.NewGuid().ToString("N").Substring(0, 4);
                            // 2. 組合檔名：Product-{ID}-{時間}-{隨機碼}.jpg
                            // 結果範例：Product-10-20260108120000-a1b2.jpg
                            string fileName = $"Product-{product.ProductId}-{DateTime.Now:yyyyMMddHHmmss}-{randomCode}{Path.GetExtension(file.FileName)}";

                            // 3. 組合完整硬碟路徑 (例如 C:\...\wwwroot\img\shop\abc.jpg)
                            string fullPath = Path.Combine(folderPath, fileName);

                            // 4. 存到硬碟
                            using (var fileStream = new FileStream(fullPath, FileMode.Create))
                            {
                                await file.CopyToAsync(fileStream);
                            }

                            // D. 存相對路徑到資料庫 (給前端用的網址片段)
                            var productImage = new ProductImage
                            {
                                ProductId = product.ProductId,
                                // ★ 這裡存的是 "img/shop/檔名"，前端只要前面加網域就能讀到了
                                ImagePath = $"img/shop/{fileName}"
                            };
                            _context.Add(productImage);
                        }
                    }
                    // 儲存所有圖片
                    await _context.SaveChangesAsync();
                }

                return RedirectToAction(nameof(Index));
            }

            // 如果失敗，重新載入下拉選單
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "CategoryId", "CategoryName", model.CategoryId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Email", model.MemberId);

            return View(model);
        }

        // GET: Shop/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products.FindAsync(id);
            if (product == null) return NotFound();

            // 1. 把資料庫的 Product 轉成 ViewModel 丟給前端
            var viewModel = new ProductEditViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryId = product.CategoryId,
                MemberId = product.MemberId,
                Status = product.Status
            };

            // 2. 準備下拉選單
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "CategoryId", "CategoryName", product.CategoryId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Email", product.MemberId);

            // ★ 修改：撈出所有舊圖並放入 ViewModel
            var currentPaths = await _context.ProductImages
                .Where(p => p.ProductId == id)
                .Select(p => p.ImagePath)
                .ToListAsync();

            foreach (var path in currentPaths)
            {
                if (!string.IsNullOrEmpty(path))
                {
                    // 這裡先借用原本的 ImageBase64List 欄位來放網址
                    viewModel.ImageUrlList.Add($"{Request.Scheme}://{Request.Host}/{path}");
                }
            }

            return View(viewModel);
        }

        // POST: Shop/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, ProductEditViewModel model)
        {
            if (id != model.ProductId) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    // 1. 撈出原本的資料
                    var product = await _context.Products.FindAsync(id);
                    if (product == null) return NotFound();

                    // 2. 更新欄位 (將 ViewModel 的值寫回資料庫實體)
                    product.ProductName = model.ProductName;
                    product.Description = model.Description;
                    product.Price = model.Price;
                    product.Stock = model.Stock;
                    product.CategoryId = model.CategoryId;
                    product.MemberId = model.MemberId;
                    product.Status = model.Status;

                    _context.Update(product);

                    // 3. ★ 圖片更新邏輯
                    // 如果使用者有上傳新圖片 (model.Photos 有東西)
                    if (model.Photos != null && model.Photos.Count > 0)
                    {
                        // A. 先刪除舊圖片 (策略：舊的不去新的不來，避免累積太多圖)
                        string wwwRootPath = _hostEnvironment.WebRootPath;

                        var oldImages = await _context.ProductImages.Where(p => p.ProductId == id).ToListAsync();
                        foreach (var oldImg in oldImages)
                        {
                            if (!string.IsNullOrEmpty(oldImg.ImagePath))
                            {
                                string oldFilePath = Path.Combine(wwwRootPath, oldImg.ImagePath);
                                if (System.IO.File.Exists(oldFilePath)) System.IO.File.Delete(oldFilePath);
                            }
                        }
                        _context.ProductImages.RemoveRange(oldImages);

                        // B. 存入新圖片 (存到 img/shop)
                        string folderPath = Path.Combine(wwwRootPath, "img", "shop");
                        if (!Directory.Exists(folderPath)) Directory.CreateDirectory(folderPath);

                        foreach (var file in model.Photos)
                        {
                            if (file.Length > 0)
                            {
                                // 1. 產生一個短的隨機碼 (用 Guid 取前 4 碼，夠亂又不長)
                                string randomCode = Guid.NewGuid().ToString("N").Substring(0, 4);
                                // 2. 組合檔名：Product-{ID}-{時間}-{隨機碼}.jpg
                                // 結果範例：Product-10-20260108120000-a1b2.jpg
                                string fileName = $"Product-{product.ProductId}-{DateTime.Now:yyyyMMddHHmmss}-{randomCode}{Path.GetExtension(file.FileName)}";

                                string fullPath = Path.Combine(folderPath, fileName);

                                using (var fileStream = new FileStream(fullPath, FileMode.Create))
                                {
                                    await file.CopyToAsync(fileStream);
                                }

                                var productImage = new ProductImage
                                {
                                    ProductId = product.ProductId,
                                    ImagePath = $"img/shop/{fileName}"
                                };
                                _context.Add(productImage);
                                }
                            }
                        }         
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(model.ProductId)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            // 驗證失敗，重載下拉選單
            ViewData["CategoryId"] = new SelectList(_context.ProductCategories, "CategoryId", "CategoryName", model.CategoryId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Email", model.MemberId);

            return View(model);
        }

        // GET: Shop/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Member)
                .FirstOrDefaultAsync(m => m.ProductId == id);

            if (product == null) return NotFound();

            // ★ 修改：撈出該商品的所有圖片
            var images = await _context.ProductImages
                .Where(p => p.ProductId == id)
                .ToListAsync();

            var imgList = new List<string>();
            foreach (var img in images)
            {
                // ★ 只要有路徑，就組裝成完整的網址
                if (!string.IsNullOrEmpty(img.ImagePath))
                {
                    imgList.Add($"{Request.Scheme}://{Request.Host}/{img.ImagePath}");
                }
            }

            // 重用 DetailViewModel
            var viewModel = new ProductDetailViewModel
            {
                ProductId = product.ProductId,
                ProductName = product.ProductName,
                Description = product.Description,
                Price = product.Price,
                Stock = product.Stock,
                CategoryName = product.Category?.CategoryName,
                SellerEmail = product.Member?.Email,
                Status = product.Status,
                CreateDate = product.CreateDate,
                ImageUrlList = imgList
            };

            return View(viewModel);
        }

        // POST: Shop/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            // 找出商品
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                // 1. 先撈出所有相關圖片 (為了拿到路徑)
                var images = await _context.ProductImages
                                   .Where(p => p.ProductId == id)
                                   .ToListAsync();

                // 2. ★ 新增：刪除實體檔案的邏輯
                string wwwRootPath = _hostEnvironment.WebRootPath; // 記得要有注入 IWebHostEnvironment 喔！

                foreach (var img in images)
                {
                    if (!string.IsNullOrEmpty(img.ImagePath))
                    {
                        // 組合出硬碟上的真實路徑
                        // 例如: C:\Projects\PetWeb\wwwroot\img\shop\abc.jpg
                        string filePath = Path.Combine(wwwRootPath, img.ImagePath);

                        // 如果檔案存在，就刪掉它！
                        if (System.IO.File.Exists(filePath))
                        {
                            System.IO.File.Delete(filePath);
                        }
                    }
                }

                // 3. 刪除資料庫裡的圖片紀錄
                _context.ProductImages.RemoveRange(images);

                // 4. 最後刪除商品
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.ProductId == id);
        }
    }
}
