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
    public class UsertypeController : Controller
    {
        private readonly VerifyDriverContext _context;

        public UsertypeController(VerifyDriverContext context)
        {
            _context = context;
        }

        // GET: Usertype
        public async Task<IActionResult> Index()
        {
            return View(await _context.UserTypes.ToListAsync());
        }

        // GET: Usertype/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userTypes = await _context.UserTypes
                .FirstOrDefaultAsync(m => m.UTId == id);
            if (userTypes == null)
            {
                return NotFound();
            }

            return View(userTypes);
        }

        // GET: Usertype/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Usertype/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UTId,UTDescription")] UserTypes userTypes)
        {
            if (ModelState.IsValid)
            {
                _context.Add(userTypes);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(userTypes);
        }

        // GET: Usertype/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userTypes = await _context.UserTypes.FindAsync(id);
            if (userTypes == null)
            {
                return NotFound();
            }
            return View(userTypes);
        }

        // POST: Usertype/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("UTId,UTDescription")] UserTypes userTypes)
        {
            if (id != userTypes.UTId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(userTypes);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserTypesExists(userTypes.UTId))
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
            return View(userTypes);
        }

        // GET: Usertype/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var userTypes = await _context.UserTypes
                .FirstOrDefaultAsync(m => m.UTId == id);
            if (userTypes == null)
            {
                return NotFound();
            }

            return View(userTypes);
        }

        // POST: Usertype/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var userTypes = await _context.UserTypes.FindAsync(id);
            _context.UserTypes.Remove(userTypes);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserTypesExists(long id)
        {
            return _context.UserTypes.Any(e => e.UTId == id);
        }
    }
}
