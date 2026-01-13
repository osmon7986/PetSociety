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
    public class CommentsController : Controller
    {
        private readonly PetSocietyContext _context;

        public CommentsController(PetSocietyContext context)
        {
            _context = context;
        }

        //// GET: Community/Comments
        //public async Task<IActionResult> Index()
        //{
        //    var petSocietyContext = _context.Comments.Include(c => c.Article).Include(c => c.Member);
        //    return View(await petSocietyContext.ToListAsync());
        //}

        // GET: Community/Comments
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // POST: Community/Comments/IndexJson
        [HttpPost]
        [Route("/Community/Comments/IndexJson")]
        public async Task<IActionResult> IndexJson()
        {

            return Json(_context.Comments);
        }

        // GET: Community/Comments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Article)
                .Include(c => c.Member)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // GET: Community/Comments/Create
        public IActionResult Create()
        {
            ViewData["ArticleId"] = new SelectList(_context.Articles, "ArticleId", "Content");
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name");
            return View();
        }

        // POST: Community/Comments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CommentId,ArticleId,MemberId,Content,PostDate,Picture")] Comment comment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["ArticleId"] = new SelectList(_context.Articles, "ArticleId", "Content", comment.ArticleId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", comment.MemberId);
            return View(comment);
        }

        // GET: Community/Comments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments.FindAsync(id);
            if (comment == null)
            {
                return NotFound();
            }
            ViewData["ArticleId"] = new SelectList(_context.Articles, "ArticleId", "Content", comment.ArticleId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", comment.MemberId);
            return View(comment);
        }

        // POST: Community/Comments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CommentId,ArticleId,MemberId,Content,PostDate,Picture")] Comment comment)
        {
            if (id != comment.CommentId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentExists(comment.CommentId))
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
            ViewData["ArticleId"] = new SelectList(_context.Articles, "ArticleId", "Content", comment.ArticleId);
            ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", comment.MemberId);
            return View(comment);
        }

        // GET: Community/Comments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comment = await _context.Comments
                .Include(c => c.Article)
                .Include(c => c.Member)
                .FirstOrDefaultAsync(m => m.CommentId == id);
            if (comment == null)
            {
                return NotFound();
            }

            return View(comment);
        }

        // POST: Community/Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var comment = await _context.Comments.FindAsync(id);
            if (comment != null)
            {
                _context.Comments.Remove(comment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentExists(int id)
        {
            return _context.Comments.Any(e => e.CommentId == id);
        }
    }
}
