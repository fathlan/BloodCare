using BloodCare.Data;
using BloodCare.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BloodCare.Controllers
{
    // TIDAK ada [Authorize] di level class
    // supaya Index/Details bisa diakses User Umum tanpa login,
    // sesuai use case "Melihat jadwal donor darah"
    public class JadwalDonorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public JadwalDonorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: JadwalDonor -> PUBLIK (User Umum, Pendonor, Petugas, Admin semua bisa lihat)
        public async Task<IActionResult> Index()
        {
            var data = await _context.JadwalDonors
                .OrderByDescending(j => j.Tanggal)
                .ToListAsync();
            return View(data);
        }

        // GET: JadwalDonor/Details/5 -> PUBLIK
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var jadwal = await _context.JadwalDonors
                .FirstOrDefaultAsync(m => m.Id == id);

            if (jadwal == null) return NotFound();

            return View(jadwal);
        }

        // GET: JadwalDonor/Create -> HANYA ADMIN (sesuai use case "Mengelola jadwal donor")
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: JadwalDonor/Create
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Lokasi,Tanggal,Kuota,Keterangan")] JadwalDonor jadwal)
        {
            if (ModelState.IsValid)
            {
                _context.Add(jadwal);
                await _context.SaveChangesAsync();
                TempData["SweetSuccess"] = "Jadwal donor berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }
            return View(jadwal);
        }

        // GET: JadwalDonor/Edit/5 -> HANYA ADMIN
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var jadwal = await _context.JadwalDonors.FindAsync(id);
            if (jadwal == null) return NotFound();

            return View(jadwal);
        }

        // POST: JadwalDonor/Edit/5
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Lokasi,Tanggal,Kuota,Keterangan")] JadwalDonor jadwal)
        {
            if (id != jadwal.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(jadwal);
                    await _context.SaveChangesAsync();
                    TempData["SweetSuccess"] = "Jadwal donor berhasil diperbarui!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!JadwalDonorExists(jadwal.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(jadwal);
        }

        // GET: JadwalDonor/Delete/5 -> HANYA ADMIN
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var jadwal = await _context.JadwalDonors
                .FirstOrDefaultAsync(m => m.Id == id);

            if (jadwal == null) return NotFound();

            return View(jadwal);
        }

        // POST: JadwalDonor/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var jadwal = await _context.JadwalDonors.FindAsync(id);
            if (jadwal != null)
            {
                _context.JadwalDonors.Remove(jadwal);
                await _context.SaveChangesAsync();
                TempData["SweetSuccess"] = "Jadwal donor berhasil dihapus!";
            }
            return RedirectToAction(nameof(Index));
        }

        private bool JadwalDonorExists(int id)
            => _context.JadwalDonors.Any(e => e.Id == id);
    }
}