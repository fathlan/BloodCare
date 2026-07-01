using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BloodCare.Data;
using BloodCare.Models;
using Microsoft.AspNetCore.Authorization;

namespace BloodCare.Controllers
{
    public class PendonorsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PendonorsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Pendonors
        public async Task<IActionResult> Index()
        {
            return View(await _context.Pendonors.ToListAsync());
        }

        // GET: Pendonors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pendonor = await _context.Pendonors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pendonor == null)
            {
                return NotFound();
            }

            return View(pendonor);
        }

        // GET: Pendonors/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Pendonors/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Nama,Umur,JenisKelamin,GolonganDarah,Rhesus,Alamat,NoHP,TanggalDonorTerakhir")] Pendonor pendonor)
        {
            if (ModelState.IsValid)
            {
                _context.Add(pendonor);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(pendonor);
        }

        // GET: Pendonors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pendonor = await _context.Pendonors.FindAsync(id);
            if (pendonor == null)
            {
                return NotFound();
            }
            return View(pendonor);
        }

        // POST: Pendonors/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Nama,Umur,JenisKelamin,GolonganDarah,Rhesus,Alamat,NoHP,TanggalDonorTerakhir")] Pendonor pendonor)
        {
            if (id != pendonor.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pendonor);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PendonorExists(pendonor.Id))
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
            return View(pendonor);
        }

        // GET: Pendonors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var pendonor = await _context.Pendonors
                .FirstOrDefaultAsync(m => m.Id == id);
            if (pendonor == null)
            {
                return NotFound();
            }

            return View(pendonor);
        }

        // POST: Pendonors/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var pendonor = await _context.Pendonors.FindAsync(id);
            if (pendonor != null)
            {
                _context.Pendonors.Remove(pendonor);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PendonorExists(int id)
        {
            return _context.Pendonors.Any(e => e.Id == id);
        }
    }
}
