using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using MvcDriverVerify.Models;

namespace MvcDriverVerify.Controllers
{
    public class PlatformController : Controller
    {
        private readonly VerifyDriverContext _context;

        public PlatformController(VerifyDriverContext context)
        {
            _context = context;
        }

        // GET: Platform
        public async Task<IActionResult> Index()
        {
            return View(await _context.Platform.ToListAsync());
        }

        // GET: Platform/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platform = await _context.Platform
                .FirstOrDefaultAsync(m => m.PId == id);
            if (platform == null)
            {
                return NotFound();
            }

            return View(platform);
        }

        // GET: Platform/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Platform/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PId,PName,PType")] Platform platform)
        {
            if (ModelState.IsValid)
            {
                _context.Add(platform);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(platform);
        }

        // GET: Platform/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platform = await _context.Platform.FindAsync(id);
            if (platform == null)
            {
                return NotFound();
            }
            return View(platform);
        }

        // POST: Platform/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("PId,PName,PType")] Platform platform)
        {
            if (id != platform.PId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(platform);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PlatformExists(platform.PId))
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
            return View(platform);
        }

        // GET: Platform/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var platform = await _context.Platform
                .FirstOrDefaultAsync(m => m.PId == id);
            if (platform == null)
            {
                return NotFound();
            }

            return View(platform);
        }

        // POST: Platform/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var platform = await _context.Platform.FindAsync(id);
            _context.Platform.Remove(platform);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PlatformExists(long id)
        {
            return _context.Platform.Any(e => e.PId == id);
        }
    }
}
