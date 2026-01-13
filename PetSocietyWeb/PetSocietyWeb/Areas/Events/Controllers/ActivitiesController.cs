using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using PetSocietyWeb.Models.Generated;

namespace PetSocietyWeb.Areas.Events.Controllers
{
    [Area("Events")]
    public class ActivitiesController : Controller
    {
        private readonly PetSocietyContext _context;

        public ActivitiesController(PetSocietyContext context)
        {
            _context = context;
        }

        // GET: Events/Activities
        public async Task<IActionResult> Index()
        {
            return View();
        }

        //POST : Events/Activities/IndexJson
        [HttpPost]
        public async Task<IActionResult> IndexJson()
        {
            var list = await _context.Activities
                .Select(a => new {
                    a.ActivityId,
                    a.Title,
                    a.Description,
                    a.StartTime,
                    a.EndTime,
                    a.Location,
                    a.MaxCapacity,
                    a.ActivityCheck,
                    a.ActivityImg
                })
                .ToListAsync();

            return Json(list);
        }

        // GET: Events/Activities/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities
                .Include(a => a.MemberId)
                .FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // GET: Events/Activities/Check
        [HttpGet]
        public IActionResult Check()
        {
            return View();
        }

        // Post: Events/Activities/CheckJson
        [HttpPost]
        public async Task<IActionResult> CheckJson()
        {
            var list = await _context.Activities
                .Include(a => a.MemberId)
                .Where(a => a.ActivityCheck == 0) //只顯示未審核的活動
                .Select(a => new {
                    a.ActivityId,
                    a.Title,
                    a.Description,
                    a.ActivityCheck,
                })
                .ToListAsync();
            return Json(list);
        }

        // GET: Events/Activities/Create
        public IActionResult Create()
        {
            ViewBag.MemberList = _context.Members
            .Select(c => new SelectListItem
            {
                Value = c.MemberId.ToString(),
                Text = c.Name,
            })
            .ToList();

            
            return View();
        }

        // POST: Events/Activities/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ActivityId,MemberId,Title,Description,StartTime,EndTime,Location,MaxCapacity,RegistrationStartTime,RegistrationEndTime,Status,Latitude,Longitude,ActivityImg")] Activity activity)
        {
            if (ModelState.IsValid)
            {
                _context.Add(activity);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewBag["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", activity.MemberId); 
            activity.CreatedDate = DateTime.Now;
            activity.ActivityCheck = 0; //預設活動審核狀態為0(未審核)
            return View(activity);
        }

        // GET: Events/Activities/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            //ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", activity.MemberId);
            return View(activity);
        }

        
        // POST: Events/Activities/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ActivityId,Title,Description,StartTime,EndTime,Location,MaxCapacity,RegistrationStartTime,RegistrationEndTime,Status,Latitude,Longitude,CreatedDate,MemberId,ActivityImg")] Activity activity)
        {
            if (id != activity.ActivityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity.ActivityId))
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
            //ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", activity.MemberId);
            return View(activity);
        }

        //Todo: 活動審核未修好
        public async Task<IActionResult> CheckDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities.FindAsync(id);
            if (activity == null)
            {
                return NotFound();
            }
            //ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", activity.MemberId);
            return View(activity);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CheckDetail(int id, [Bind("ActivityId,Title,Description,ActivityCheck")] Activity activity)
        {
            if (id != activity.ActivityId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(activity);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ActivityExists(activity.ActivityId))
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
            //ViewData["MemberId"] = new SelectList(_context.Members, "MemberId", "Name", activity.MemberId);
            return View(activity);
        }
        
        // GET: Events/Activities/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var activity = await _context.Activities
                .Include(a => a.MemberId)
                .FirstOrDefaultAsync(m => m.ActivityId == id);
            if (activity == null)
            {
                return NotFound();
            }

            return View(activity);
        }

        // POST: Events/Activities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var activity = await _context.Activities.FindAsync(id);
            if (activity != null)
            {
                _context.Activities.Remove(activity);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ActivityExists(int id)
        {
            return _context.Activities.Any(e => e.ActivityId == id);
        }
    }
}
