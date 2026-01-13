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
    public class ForumTagsController : Controller
    {
        private readonly PetSocietyContext _context;

        public ForumTagsController(PetSocietyContext context)
        {
            _context = context;
        }

        //// GET: Community/ForumTags
        //public async Task<IActionResult> Index()
        //{
        //    return View(await _context.ForumTags.ToListAsync());
        //}

        // GET: Community/ForumTags
        public async Task<IActionResult> Index()
        {
            return View();
        }

        // POST: Community/ForumTags/IndexJson
        [HttpPost]
        [Route("/Community/ForumTags/IndexJson")]
        public async Task<IActionResult> IndexJson()
        {

            return Json(_context.ForumTags);
        }

        // GET: Community/ForumTags/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTag = await _context.ForumTags
                .FirstOrDefaultAsync(m => m.TagId == id);
            if (forumTag == null)
            {
                return NotFound();
            }

            return View(forumTag);
        }

        // GET: Community/ForumTags/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Community/ForumTags/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("TagId,TagName")] ForumTag forumTag)
        {
            if (ModelState.IsValid)
            {
                _context.Add(forumTag);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(forumTag);
        }

        // GET: Community/ForumTags/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTag = await _context.ForumTags.FindAsync(id);
            if (forumTag == null)
            {
                return NotFound();
            }
            return View(forumTag);
        }

        // POST: Community/ForumTags/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TagId,TagName")] ForumTag forumTag)
        {
            if (id != forumTag.TagId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(forumTag);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ForumTagExists(forumTag.TagId))
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
            return View(forumTag);
        }

        // GET: Community/ForumTags/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var forumTag = await _context.ForumTags
                .FirstOrDefaultAsync(m => m.TagId == id);
            if (forumTag == null)
            {
                return NotFound();
            }

            return View(forumTag);
        }

        // POST: Community/ForumTags/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var forumTag = await _context.ForumTags.FindAsync(id);
            if (forumTag != null)
            {
                _context.ForumTags.Remove(forumTag);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ForumTagExists(int id)
        {
            return _context.ForumTags.Any(e => e.TagId == id);
        }
    }
}
