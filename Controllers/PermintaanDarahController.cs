using BloodCare.Data;
using BloodCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BloodCare.Controllers
{

    [Authorize(Roles = "Admin")]
    public class PermintaanDarahController : Controller
    {
        private readonly ApplicationDbContext _context;

        public PermintaanDarahController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: PermintaanDarah
        public async Task<IActionResult> Index()
        {
            var data = await _context.PermintaanDarahs
                .OrderByDescending(p => p.TanggalPermintaan)
                .ToListAsync();
            return View(data);
        }

        // GET: PermintaanDarah/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var permintaan = await _context.PermintaanDarahs
                .FirstOrDefaultAsync(m => m.Id == id);

            if (permintaan == null) return NotFound();

            return View(permintaan);
        }

        // GET: PermintaanDarah/Create
        public IActionResult Create()
        {
            SetDropdowns();
            return View();
        }

        // POST: PermintaanDarah/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NamaPasien,RumahSakit,GolonganDarah,Rhesus,JumlahKebutuhan")] PermintaanDarah permintaan)
        {
            if (ModelState.IsValid)
            {
                permintaan.TanggalPermintaan = DateTime.Now;
                permintaan.Status = "Pending";
                _context.Add(permintaan);
                await _context.SaveChangesAsync();
                TempData["SweetSuccess"] = "Permintaan darah berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }

            SetDropdowns();
            return View(permintaan);
        }

        // GET: PermintaanDarah/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var permintaan = await _context.PermintaanDarahs.FindAsync(id);
            if (permintaan == null) return NotFound();

            SetDropdowns();
            return View(permintaan);
        }

        // POST: PermintaanDarah/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NamaPasien,RumahSakit,GolonganDarah,Rhesus,JumlahKebutuhan,Status,TanggalPermintaan")] PermintaanDarah permintaan)
        {
            if (id != permintaan.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(permintaan);
                    await _context.SaveChangesAsync();
                    TempData["SweetSuccess"] = "Permintaan darah berhasil diperbarui!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PermintaanDarahExists(permintaan.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }

            SetDropdowns();
            return View(permintaan);
        }

        // GET: PermintaanDarah/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var permintaan = await _context.PermintaanDarahs
                .FirstOrDefaultAsync(m => m.Id == id);

            if (permintaan == null) return NotFound();

            return View(permintaan);
        }

        // POST: PermintaanDarah/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var permintaan = await _context.PermintaanDarahs.FindAsync(id);
            if (permintaan != null)
            {
                _context.PermintaanDarahs.Remove(permintaan);
                await _context.SaveChangesAsync();
                TempData["SweetSuccess"] = "Permintaan darah berhasil dihapus!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool PermintaanDarahExists(int id)
            => _context.PermintaanDarahs.Any(e => e.Id == id);

        private void SetDropdowns()
        {
            ViewBag.GolonganDarahList = new List<string> { "A", "B", "AB", "O" };
            ViewBag.RhesusList = new List<string> { "+", "-" };
            ViewBag.StatusList = new List<string> { "Pending", "Diproses", "Selesai", "Ditolak" };
        }
    }
}