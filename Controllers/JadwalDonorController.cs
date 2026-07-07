using BloodCare.Data;
using BloodCare.Models;
using BloodCare.ViewModels;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;

namespace BloodCare.Controllers
{

    public class JadwalDonorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public JadwalDonorController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: JadwalDonor publik
        public async Task<IActionResult> Index()
        {
            var data = await _context.JadwalDonors
                .OrderByDescending(j => j.Tanggal)
                .ToListAsync();

            var jadwalIds = data.Select(j => j.Id).ToList();

            // Hitung jumlah pendaftar per jadwal dari tabel PendaftaranJadwals
            var jumlahPerJadwal = await _context.PendaftaranJadwals
                .Where(p => jadwalIds.Contains(p.JadwalDonorId))
                .GroupBy(p => p.JadwalDonorId)
                .Select(g => new { JadwalDonorId = g.Key, Total = g.Count() })
                .ToDictionaryAsync(x => x.JadwalDonorId, x => x.Total);

            // Kalau yang login adalah Pendonor, cek jadwal mana saja yang sudah dia daftar
            var sudahDaftarIds = new HashSet<int>();
            if (User.Identity!.IsAuthenticated && User.IsInRole("Pendonor"))
            {
                var user = await _userManager.GetUserAsync(User);
                if (user != null)
                {
                    var idsTerdaftar = await _context.PendaftaranJadwals
                        .Where(p => jadwalIds.Contains(p.JadwalDonorId) && p.PendonorUserId == user.Id)
                        .Select(p => p.JadwalDonorId)
                        .ToListAsync();
                    sudahDaftarIds = idsTerdaftar.ToHashSet();
                }
            }

            var viewModel = data.Select(j => new JadwalDonorViewModel
            {
                Jadwal = j,
                JumlahTerdaftar = jumlahPerJadwal.TryGetValue(j.Id, out var total) ? total : 0,
                SudahDaftar = sudahDaftarIds.Contains(j.Id)
            }).ToList();

            // Admin melihat tabel kelola (Create/Edit/Delete).
            // Selain Admin (Petugas, Pendonor, User Umum) melihat kartu ringkas sesuai mockup.
            if (!User.IsInRole("Admin"))
            {
                return View("IndexCards", viewModel);
            }

            return View(viewModel);
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
