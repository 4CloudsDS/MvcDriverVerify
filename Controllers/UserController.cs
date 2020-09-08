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
    public class UserController : Controller
    {
        private readonly VerifyDriverContext _context;

        public UserController(VerifyDriverContext context)
        {
            _context = context;
        }

        // GET: User
        public async Task<IActionResult> Index( string s, string v )
        {

            IQueryable<String> vehicleQuery = from w in _context.Vehicle
                                              where w.Vregistration.Equals(v)  
                                              orderby w.VId
                                              select w.Vregistration;    

            var user = from x in _context.User
                                         .Include(u => u.UPartner)
                                         .Include(u => u.UPlatform)
                                         .Include(u => u.UUsertype)
                                         .Include(u => u.UV)
                       select x;

            if (!String.IsNullOrEmpty(s))
            {
                user = user.Where(y => y.UNames.Contains(s));
            }

            if (!String.IsNullOrEmpty(v))
            {
                user = user.Where(y => y.Vehicle.Contains( y.UV ));
            }

            var vehicleDriverVM = new Models.DriverVehicleModel
            {
                Vehicles = new SelectList(await vehicleQuery.Distinct().ToListAsync()),
                Users = await user.ToListAsync()
            };

            return View(vehicleDriverVM);
        }

        // GET: User/Details/5
        public async Task<IActionResult> Details(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.UPartner)
                .Include(u => u.UPlatform)
                .Include(u => u.UUsertype)
                .Include(u => u.UV)
                .FirstOrDefaultAsync(m => m.UId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // GET: User/Create
        public IActionResult Create()
        {
            ViewData["UPartnerId"] = new SelectList(_context.User, "UId", "UNames");
            ViewData["UPlatformId"] = new SelectList(_context.Platform, "PId", "PName");
            ViewData["UUsertypeId"] = new SelectList(_context.UserTypes, "UTId", "UTDescription");
            ViewData["UVid"] = new SelectList(_context.Vehicle, "VId", "Vregistration");
            return View();
        }

        // POST: User/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UId,UNames,UGender,UAge,URating,UVid,UPartnerId,UUsertypeId,UPlatformId")] User user)
        {
            if (ModelState.IsValid)
            {
                _context.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["UPartnerId"] = new SelectList(_context.User, "UId", "UNames", user.UPartnerId);
            ViewData["UPlatformId"] = new SelectList(_context.Platform, "PId", "PName", user.UPlatformId);
            ViewData["UUsertypeId"] = new SelectList(_context.UserTypes, "UTId", "UTDescription", user.UUsertypeId);
            ViewData["UVid"] = new SelectList(_context.Vehicle, "VId", "Vregistration", user.UVid);
            return View(user);
        }

        // GET: User/Edit/5
        public async Task<IActionResult> Edit(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User.FindAsync(id);
            if (user == null)
            {
                return NotFound();
            }
            ViewData["UPartnerId"] = new SelectList(_context.User, "UId", "UNames", user.UPartnerId);
            ViewData["UPlatformId"] = new SelectList(_context.Platform, "PId", "PName", user.UPlatformId);
            ViewData["UUsertypeId"] = new SelectList(_context.UserTypes, "UTId", "UTDescription", user.UUsertypeId);
            ViewData["UVid"] = new SelectList(_context.Vehicle, "VId", "Vregistration", user.UVid);
            return View(user);
        }

        // POST: User/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(long id, [Bind("UId,UNames,UGender,UAge,URating,UVid,UPartnerId,UUsertypeId,UPlatformId")] User user)
        {
            if (id != user.UId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(user);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!UserExists(user.UId))
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
            ViewData["UPartnerId"] = new SelectList(_context.User, "UId", "UNames", user.UPartnerId);
            ViewData["UPlatformId"] = new SelectList(_context.Platform, "PId", "PName", user.UPlatformId);
            ViewData["UUsertypeId"] = new SelectList(_context.UserTypes, "UTId", "UTDescription", user.UUsertypeId);
            ViewData["UVid"] = new SelectList(_context.Vehicle, "VId", "Vregistration", user.UVid);
            return View(user);
        }

        // GET: User/Delete/5
        public async Task<IActionResult> Delete(long? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var user = await _context.User
                .Include(u => u.UPartner)
                .Include(u => u.UPlatform)
                .Include(u => u.UUsertype)
                .Include(u => u.UV)
                .FirstOrDefaultAsync(m => m.UId == id);
            if (user == null)
            {
                return NotFound();
            }

            return View(user);
        }

        // POST: User/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(long id)
        {
            var user = await _context.User.FindAsync(id);
            _context.User.Remove(user);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool UserExists(long id)
        {
            return _context.User.Any(e => e.UId == id);
        }
    }
}
