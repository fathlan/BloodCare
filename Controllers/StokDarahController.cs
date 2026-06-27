using BloodCare.Data;
using BloodCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{
    public class StokDarahController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StokDarahController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var data = await _context.StokDarahs.ToListAsync();
            return View(data);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(StokDarah stokDarah)
        {
            if (ModelState.IsValid)
            {
                stokDarah.TanggalMasuk = DateTime.Now;
                _context.Add(stokDarah);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(stokDarah);
        }

        public async Task<IActionResult> Edit(int id)
        {
            var stokDarah = await _context.StokDarahs.FindAsync(id);

            if (stokDarah == null)
            {
                return NotFound();
            }

            return View(stokDarah);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, StokDarah stokDarah)
        {
            if (id != stokDarah.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                stokDarah.TanggalMasuk = DateTime.Now;
                _context.Update(stokDarah);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(stokDarah);
        }

        public async Task<IActionResult> Details(int id)
        {
            var stokDarah = await _context.StokDarahs.FindAsync(id);

            if (stokDarah == null)
            {
                return NotFound();
            }

            return View(stokDarah);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var stokDarah = await _context.StokDarahs.FindAsync(id);

            if (stokDarah == null)
            {
                return NotFound();
            }

            return View(stokDarah);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stokDarah = await _context.StokDarahs.FindAsync(id);

            if (stokDarah != null)
            {
                _context.StokDarahs.Remove(stokDarah);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}