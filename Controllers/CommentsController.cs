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
    public class CommentsController : Controller
    {
        private readonly VerifyDriverContext _context;

        public CommentsController(VerifyDriverContext context)
        {
            _context = context;
        }

        // GET: Comments
        public async Task<IActionResult> Index( string s)
        {
            var comment = from x in _context.Comments
                                            .Include(c => c.CP)
                                            .Include(c => c.CU)
                          select x;
            
            if (!String.IsNullOrEmpty(s))
            {
                comment = comment.Where(y => y.CText.Contains(s));
            }
            
            return View(await comment.ToListAsync());
        }

        // GET: Comments/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Include(c => c.CP)
                .Include(c => c.CU)
                .FirstOrDefaultAsync(m => m.CId == id);
            if (comments == null)
            {
                return NotFound();
            }

            return View(comments);
        }

        // GET: Comments/Create
        public IActionResult Create()
        {
            ViewData["CPid"] = new SelectList(_context.User, "UId", "UNames");
            ViewData["CUid"] = new SelectList(_context.User, "UId", "UNames");
            return View();
        }

        // POST: Comments/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CId,CText,CDateTime,CUid,CPid")] Comments comments)
        {
            if (ModelState.IsValid)
            {
                _context.Add(comments);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["CPid"] = new SelectList(_context.User, "UId", "UNames", comments.CPid);
            ViewData["CUid"] = new SelectList(_context.User, "UId", "UNames", comments.CUid);
            return View(comments);
        }

        // GET: Comments/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments.FindAsync(id);
            if (comments == null)
            {
                return NotFound();
            }
            ViewData["CPid"] = new SelectList(_context.User, "UId", "UNames", comments.CPid);
            ViewData["CUid"] = new SelectList(_context.User, "UId", "UNames", comments.CUid);
            return View(comments);
        }

        // POST: Comments/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("CId,CText,CDateTime,CUid,CPid")] Comments comments)
        {
            if (id != comments.CId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(comments);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CommentsExists(comments.CId))
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
            ViewData["CPid"] = new SelectList(_context.User, "UId", "UNames", comments.CPid);
            ViewData["CUid"] = new SelectList(_context.User, "UId", "UNames", comments.CUid);
            return View(comments);
        }

        // GET: Comments/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var comments = await _context.Comments
                .Include(c => c.CP)
                .Include(c => c.CU)
                .FirstOrDefaultAsync(m => m.CId == id);
            if (comments == null)
            {
                return NotFound();
            }

            return View(comments);
        }

        // POST: Comments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var comments = await _context.Comments.FindAsync(id);
            _context.Comments.Remove(comments);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CommentsExists(long id)
        {
            return _context.Comments.Any(e => e.CId == id);
        }
    }
}
