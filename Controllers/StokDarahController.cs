using BloodCare.Data;
using BloodCare.Models;
using BloodCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{
    public class StokDarahController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public StokDarahController(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: StokDarah
        [AllowAnonymous]
        public async Task<IActionResult> Index()
        {
            var data = await _context.StokDarahs
                .OrderBy(s => s.GolonganDarah).ThenBy(s => s.Rhesus)
                .ToListAsync();

            ViewBag.TotalKantong = await _context.StokDarahs.SumAsync(s => s.JumlahKantong);
            ViewBag.StokKritis   = await _context.StokDarahs.CountAsync(s => s.JumlahKantong < 10);
            ViewBag.TotalJenis   = await _context.StokDarahs.CountAsync();

            // Per golongan untuk kartu
            ViewBag.StokPerGol = await _context.StokDarahs
                .GroupBy(s => s.GolonganDarah)
                .Select(g => new { Gol = g.Key, Total = g.Sum(s => s.JumlahKantong) })
                .ToListAsync();

            // Admin (dan yang belum login) tetap lihat tampilan tabel kelola stok yang lama.
            // Petugas mendapat tampilan monitoring per gol+rhesus sesuai mockup "Stok Darah".
            if (User.IsInRole("Petugas") && !User.IsInRole("Admin"))
            {
                ViewBag.PermintaanDarurat = await _context.PermintaanDarahs
                    .Where(p => p.Status == "Pending")
                    .OrderByDescending(p => p.JumlahKebutuhan)
                    .Take(3)
                    .ToListAsync();

                return View("IndexMonitor", data);
            }

            return View(data);
        }

        // GET: StokDarah/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var stok = await _context.StokDarahs.FirstOrDefaultAsync(m => m.Id == id);
            if (stok == null) return NotFound();
            return View(stok);
        }
        [Authorize(Roles = "Admin,Petugas")]
        public IActionResult Create()
        {
            SetDropdowns();
            return View();
        }

        // POST: StokDarah/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("GolonganDarah,Rhesus,JumlahKantong,TanggalKadaluarsa,Keterangan")] StokDarah stok)
        {
            if (ModelState.IsValid)
            {
                stok.TerakhirDiperbarui = DateTime.Now;
                _context.Add(stok);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    User.Identity?.Name, "Create", "StokDarah", stok.Id.ToString());

                TempData["SweetSuccess"] = "Stok darah berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }
            SetDropdowns();
            return View(stok);
        }

        // GET: StokDarah/Edit/5
        [Authorize(Roles = "Admin,Petugas")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var stok = await _context.StokDarahs.FindAsync(id);
            if (stok == null) return NotFound();
            SetDropdowns();
            return View(stok);
        }

        // POST: StokDarah/Edit/5
        [Authorize(Roles = "Admin,Petugas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,GolonganDarah,Rhesus,JumlahKantong,TanggalKadaluarsa,Keterangan")] StokDarah stok)
        {
            if (id != stok.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    stok.TerakhirDiperbarui = DateTime.Now;
                    _context.Update(stok);
                    await _context.SaveChangesAsync();

                    await _auditService.LogAsync(
                        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                        User.Identity?.Name, "Update", "StokDarah", stok.Id.ToString());

                    TempData["SweetSuccess"] = "Stok darah berhasil diperbarui!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.StokDarahs.Any(e => e.Id == stok.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            SetDropdowns();
            return View(stok);
        }

        // POST: StokDarah/Delete/5
        [Authorize(Roles = "Admin,Petugas")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stok = await _context.StokDarahs.FindAsync(id);
            if (stok != null)
            {
                await _auditService.LogAsync(
                    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    User.Identity?.Name, "Delete", "StokDarah", id.ToString());

                _context.StokDarahs.Remove(stok);
                await _context.SaveChangesAsync();
                TempData["SweetSuccess"] = "Stok darah berhasil dihapus!";
            }
            return RedirectToAction(nameof(Index));
        }

        private void SetDropdowns()
        {
            ViewBag.GolonganDarahList = new List<string> { "A", "B", "AB", "O" };
            ViewBag.RhesusList = new List<string> { "+", "-" };
        }
    }
}
