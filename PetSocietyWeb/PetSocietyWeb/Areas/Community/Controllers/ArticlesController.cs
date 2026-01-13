using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetSocietyWeb.Models.Generated;

namespace PetSocietyWeb.Areas.Community.Controllers
{
    [Area("Community")]
    public class ArticlesController : Controller
    {
        private readonly PetSocietyContext _context;

        public ArticlesController(PetSocietyContext context)
        {
            _context = context;
        }

        //// GET: Community/Articles
        //public async Task<IActionResult> Index()
        //{
        //    var petSocietyContext = _context.Articles.Include(a => a.Category).Include(a => a.Member).Include(a => a.Tag);
        //    return View(await petSocietyContext.ToListAsync());
        //}

        // GET: Community/Articles
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // POST: Community/Articles/IndexJson
        [HttpPost]
        [Route("/Community/Articles/IndexJson")]
        public async Task<IActionResult> IndexJson()
        {

            return Json(_context.Articles);
        }

        // GET: Community/Articles/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Member)
                .Include(a => a.Tag)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // GET: Community/Articles/Create
        public IActionResult Create()
        {
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName");
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name");
            ViewData["TagId"] = new SelectList(_context.ForumTags, "TagId", "TagName");
            return View();
        }

        // POST: Community/Articles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ArticleId,CategoryId,TagId,MemberId,Title,Content,PostDate,LastUpdate,Like,DisLike,Popular,CommentCount,Picture")] Article article)
        {
            if (ModelState.IsValid)
            {
                _context.Add(article);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", article.CategoryId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", article.MemberId);
            ViewData["TagId"] = new SelectList(_context.ForumTags, "TagId", "TagName", article.TagId);
            return View(article);
        }

        // GET: Community/Articles/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles.FindAsync(id);
            if (article == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", article.CategoryId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", article.MemberId);
            ViewData["TagId"] = new SelectList(_context.ForumTags, "TagId", "TagName", article.TagId);
            return View(article);
        }

        // POST: Community/Articles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ArticleId,CategoryId,TagId,MemberId,Title,Content,PostDate,LastUpdate,Like,DisLike,Popular,CommentCount,Picture")] Article article)
        {
            if (id != article.ArticleId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(article);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ArticleExists(article.ArticleId))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "CategoryId", "CategoryName", article.CategoryId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", article.MemberId);
            ViewData["TagId"] = new SelectList(_context.ForumTags, "TagId", "TagName", article.TagId);
            return View(article);
        }

        // GET: Community/Articles/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var article = await _context.Articles
                .Include(a => a.Category)
                .Include(a => a.Member)
                .Include(a => a.Tag)
                .FirstOrDefaultAsync(m => m.ArticleId == id);
            if (article == null)
            {
                return NotFound();
            }

            return View(article);
        }

        // POST: Community/Articles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var article = await _context.Articles.FindAsync(id);
            if (article != null)
            {
                _context.Articles.Remove(article);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ArticleExists(int id)
        {
            return _context.Articles.Any(e => e.ArticleId == id);
        }
    }
}
