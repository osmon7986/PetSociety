using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetSocietyWeb.Areas.Class.Extensions;
using PetSocietyWeb.Areas.Class.Models.Enum;
using PetSocietyWeb.Areas.Class.Services;
using PetSocietyWeb.Areas.Class.ViewModels;
using PetSocietyWeb.Models.Generated;

namespace PetSocietyWeb.Areas.Class.Controllers
{
    [Area("Class")]
    // 針對同個Controller底下的所有action函式一併定義Route
    [Route("[area]/Courses/[action]/{CourseId?}")]
    public class CoursesController : Controller
    {   
        private readonly PetSocietyContext _context;
        private readonly CourseService _service;

        public CoursesController(PetSocietyContext context, CourseService service)
        {
            _context = context;
            _service = service;
        }

        [HttpGet]
        // GET: Class/Courses/Index
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // GET: Class/Courses/List
        [HttpGet]
        public async Task<IActionResult> List()
        {
            List<CourseListViewModel> cfvm = await loadData();
            return View(cfvm);
        }

        public async Task<List<CourseListViewModel>> loadData()
        {
            // 拿db資料
            var list = await _context.Courses
                .Include(c => c.CourseDetails)
                .OrderByDescending(c => c.CreatedDate)
                .Select(c => new CourseListViewModel
                {
                    CourseId = c.CourseId,
                    Title = c.Title,
                    Description = c.Description,
                    CategoryId = c.CategoryId,
                    CategoryName = c.Category.CategoryName,
                    Type = c.Type,
                    CourseDetailId = c.CourseDetails.FirstOrDefault().CourseDetailId,
                    AreaId = c.CourseDetails.FirstOrDefault().AreaId,
                    InstructorId = c.CourseDetails.FirstOrDefault().InstructorId,
                    Name = c.CourseDetails.FirstOrDefault().Instructor.Name,
                    Price = c.CourseDetails.FirstOrDefault().Price,
                    Status = (CourseStatus)c.CourseDetails.FirstOrDefault().Status,
                    StartDate = c.CourseDetails.FirstOrDefault().StartDate,
                    EndDate = c.CourseDetails.FirstOrDefault().EndDate,
                    ImageUrl = c.CourseDetails.FirstOrDefault().ImageUrl,

                })
                .ToListAsync();
            
            return list;
        }

        [HttpPost]
        // DataTable
        // POST: Class/Courses/IndexJson
        public async Task<IActionResult> IndexJson()
        {
            // select Course 資料
            var courseData = await _context.Courses
                .Select(c => new
                {
                    courseId = c.CourseId,
                    imageUrl = c.CourseDetails
                                .Select(cd => cd.ImageUrl)
                                .FirstOrDefault(),
                    title = c.Title,
                    categoryName = c.Category.CategoryName,
                    instructorName = c.CourseDetails
                                      .Select(cd => cd.Instructor.Name)
                                      .FirstOrDefault(),
                    price = c.CourseDetails
                             .Select(cd => cd.Price) 
                             .FirstOrDefault(),
                    status = c.CourseDetails
                              .Select(cd => (int)cd.Status)
                              .FirstOrDefault()== (int)CourseStatus.Active ?
                              (int)CourseStatus.Active:
                                  c.CourseDetails
                                  .Select(cd => (int)cd.Status)
                                  .FirstOrDefault() == (int)CourseStatus.Disabled ?
                                   (int)CourseStatus.Disabled : (int)CourseStatus.Draft,
                    createdDate = c.CourseDetails
                                   .Select(cd => cd.CreatedDate)
                                   .FirstOrDefault(),
                })
                .ToListAsync();

            return Json(courseData);
        }


        [HttpGet]
        // GET: Class/Courses/Create
        public async Task<IActionResult> Create()
        {
            // 新增 ViewModels
            var vm = new CourseFormViewModel
            {
                Course = new CourseViewModel(),
                Details = new CourseDetailViewModel(),
                Chapters = new List<CourseChaptersViewModel>
                {
                    new CourseChaptersViewModel() // 給一個空白章節
                },
                
            };
            
            // 載入下拉選單
            await LoadDropDownsAsync(vm);

            return View(vm);
        }

        // POST: Class/Courses/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CourseFormViewModel model)
        {
            await LoadDropDownsAsync(model);
            // ModelState 驗證
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            
            // 更新 DB 資料
            try
            {
                // 呼叫service層
                await _service.CreateCourseAsync(model);
                return RedirectToAction(nameof(List));

            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "資料庫儲存失敗，請檢查資料是否重複或格式不正確。");
                return View(model);
            }
            
        }

        [HttpGet]
        // GET: Class/Courses/Edit/5
        public async Task<IActionResult> Edit(int? CourseId)
        {
            if (CourseId == null)
            {
                return NotFound();
            }

            // Course要顯示的資料
            var course = await _context.Courses
                        .Where(c => c.CourseId == CourseId)
                        .Select(c => new CourseViewModel
                        {
                            CourseId = c.CourseId,
                            Title = c.Title,
                            CategoryId = c.CategoryId,
                            Description = c.Description,
                            Type = c.Type
                        })
                        .FirstOrDefaultAsync();
            if (course == null)
            {
                return NotFound();
            }
            // CourseDetial要顯示的資料
            var detail = await _context.CourseDetails
                .Where(cd => cd.CourseId == CourseId)
                .OrderBy(cd => cd.CourseDetailId)
                .Select(cd => new CourseDetailViewModel
                {
                    
                    ImageUrl = string.IsNullOrWhiteSpace(cd.ImageUrl)? null : cd.ImageUrl,
                    CourseDetailId = cd.CourseDetailId,
                    AreaId = cd.AreaId,
                    InstructorId = cd.InstructorId,
                    Status = (CourseStatus)cd.Status,
                    Price = cd.Price,
                    StartDate = cd.StartDate,
                    EndDate = cd.EndDate,
                })
                .FirstOrDefaultAsync();
            // CourseChapter要顯示的資料
            if (detail != null)
            {
                detail.Chapters = await _context.CourseChapters
                    .Where(cc => cc.CourseDetailId == detail.CourseDetailId)
                    .OrderBy(cc => cc.SortOrder)
                    .Select(cc => new CourseChaptersViewModel
                    {
                        ChapterId = cc.ChapterId,
                        ChapterName = cc.ChapterName,
                        Summary = cc.Summary,
                        Duration = cc.Duration,
                    }).ToListAsync();
            }


            // 合併 Course Form 頁面資料 for Edit view
            var courseEditVM = new CourseFormViewModel
            {
                Course = course,
                Details = detail,
                
            };

            await LoadDropDownsAsync(courseEditVM);

            return View(courseEditVM);
        }

        

        // POST: Class/Courses/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(CourseFormViewModel model)
        {
            int courseId = model.Course.CourseId;

            // 檢查 Course Id 是否存在
            var course = await _context.Courses
                .Include(c => c.CourseDetails)
                .FirstOrDefaultAsync(c => c.CourseId == courseId);
            if (course == null)
            {
                return NotFound();
            }

            await LoadDropDownsAsync(model);
            // ModelState 驗證
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            // 更新DB資料
            course.Title = model.Course.Title;
            course.Description = model.Course.Description;
            course.CategoryId = model.Course.CategoryId;
            course.Type = model.Course.Type;

            var detail = course.CourseDetails.FirstOrDefault();
            
            detail.ImageUrl = model.Details.ImageUrl;
            detail.Price = model.Details.Price;
            //detail.Status = 0;
            detail.AreaId = model.Details.AreaId;
            detail.InstructorId = model.Details.InstructorId;
            detail.StartDate = model.Details.StartDate;
            detail.EndDate = model.Details.EndDate;


            // 存檔
            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException )
            {
                if (!CourseExists(course.CourseId))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }
            return RedirectToAction(nameof(Index));

        }

        // POST: Class/Courses/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int CourseId)
        {
          
            var ok = await _service.DeleteCourseAsync(CourseId);
            if (!ok)
            {
                return NotFound();
            }

            return Json(new { success = true });
            
        }

        private bool CourseExists(int CourseId)
        {
            return _context.Courses.Any(e => e.CourseId == CourseId);
        }

        // 讀取 CourseFormViewModel 所有下拉選單
        private async Task LoadDropDownsAsync(CourseFormViewModel model)
        {
            // 讀 CategoryList
            model.CategoryList = await _context.CourseCategories
                .Select(c => new SelectListItem
                {
                    Value = c.CategoryId.ToString(),
                    Text = c.CategoryName,
                })
                .ToListAsync();

            // 讀 AreaList
            model.AreaList = (await _context.AreaDetails
               .Select(c => new SelectListItem
               {
                   Value = c.AreaId.ToString(),
                   Text = c.AreaName,
               })
               .ToListAsync())
               .InsertEmpty();

            // 讀 InstructorList
            model.InstructorList = (await _context.CourseInstructors
                .Select(c => new SelectListItem
                {
                    Value = c.InstructorId.ToString(),
                    Text = c.Name
                })
                .ToListAsync())
                .InsertEmpty();

            // 讀 StatusList
            model.StatusList = Enum.GetValues(typeof(CourseStatus))
            .Cast<CourseStatus>()
            .Select(e => new SelectListItem
            {
                Value = ((byte)e).ToString(), 
                Text = e.GetDisplayName(),
            });
        }


        // POST: Class/Courses/GenerateSummary
        [HttpPost]
        public async Task<IActionResult> GenerateSummary (string videoId)
        {
            // 取得 Youtube 字幕
            string caption = await _service.GetVideoCaptionAsync(videoId);

            // 取得 Google AI 摘要
            string summary = await _service.GenerateSummaryAsync(caption);

            return Json(new
            {
                summary
            });
        }
    }
}
