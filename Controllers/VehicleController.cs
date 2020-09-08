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
    public class VehicleController : Controller
    {
        private readonly VerifyDriverContext _context;

        public VehicleController(VerifyDriverContext context)
        {
            _context = context;
        }

        // GET: Vehicle
        public async Task<IActionResult> Index( string s)
        {
            
            var vehicle = from x in _context.Vehicle.Include(v => v.VParter).Include(v => v.VPlatform) 
                         select x;
            
            if ( !String.IsNullOrEmpty(s))
            {
                vehicle = vehicle.Where(y => y.Vregistration.Contains(s));
            }

            return View(await vehicle.ToListAsync());
        }

        // GET: Vehicle/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .Include(v => v.VParter)
                .Include(v => v.VPlatform)
                .FirstOrDefaultAsync(m => m.VId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // GET: Vehicle/Create
        public IActionResult Create()
        {
            ViewData["VParterId"] = new SelectList(_context.User, "UId", "UNames");
            ViewData["VPlatformId"] = new SelectList(_context.Platform, "PId", "PName");
            return View();
        }

        // POST: Vehicle/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("VId,Vregistration,VMake,VModelName,VModelYear,VParterId,VPlatformId")] Vehicle vehicle)
        {
            if (ModelState.IsValid)
            {
                _context.Add(vehicle);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["VParterId"] = new SelectList(_context.User, "UId", "UNames", vehicle.VParterId);
            ViewData["VPlatformId"] = new SelectList(_context.Platform, "PId", "PName", vehicle.VPlatformId);
            return View(vehicle);
        }

        // GET: Vehicle/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle.FindAsync(id);
            if (vehicle == null)
            {
                return NotFound();
            }
            ViewData["VParterId"] = new SelectList(_context.User, "UId", "UNames", vehicle.VParterId);
            ViewData["VPlatformId"] = new SelectList(_context.Platform, "PId", "PName", vehicle.VPlatformId);
            return View(vehicle);
        }

        // POST: Vehicle/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("VId,Vregistration,VMake,VModelName,VModelYear,VParterId,VPlatformId")] Vehicle vehicle)
        {
            if (id != vehicle.VId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(vehicle);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!VehicleExists(vehicle.VId))
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
            ViewData["VParterId"] = new SelectList(_context.User, "UId", "UNames", vehicle.VParterId);
            ViewData["VPlatformId"] = new SelectList(_context.Platform, "PId", "PName", vehicle.VPlatformId);
            return View(vehicle);
        }

        // GET: Vehicle/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var vehicle = await _context.Vehicle
                .Include(v => v.VParter)
                .Include(v => v.VPlatform)
                .FirstOrDefaultAsync(m => m.VId == id);
            if (vehicle == null)
            {
                return NotFound();
            }

            return View(vehicle);
        }

        // POST: Vehicle/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var vehicle = await _context.Vehicle.FindAsync(id);
            _context.Vehicle.Remove(vehicle);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool VehicleExists(long id)
        {
            return _context.Vehicle.Any(e => e.VId == id);
        }
    }
}
