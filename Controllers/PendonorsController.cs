using BloodCare.Data;
using BloodCare.Models;
using BloodCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{
    [Authorize(Roles = "Admin,Petugas")]
    public class PendonorsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;

        public PendonorsController(ApplicationDbContext context, IAuditService auditService)
        {
            _context = context;
            _auditService = auditService;
        }

        // GET: Pendonors
        public async Task<IActionResult> Index(string? search, string? filterGol)
        {
            var query = _context.Pendonors.AsQueryable();

            if (!string.IsNullOrEmpty(search))
                query = query.Where(p => p.NamaLengkap.Contains(search) || p.NoHp.Contains(search));

            if (!string.IsNullOrEmpty(filterGol))
                query = query.Where(p => p.GolonganDarah == filterGol);

            var data = await query.OrderByDescending(p => p.TanggalDaftar).ToListAsync();

            // Stats
            ViewBag.TotalPendonor = await _context.Pendonors.CountAsync();
            ViewBag.StokPerGol = await _context.Pendonors
                .GroupBy(p => p.GolonganDarah)
                .Select(g => new { Gol = g.Key, Total = g.Count() })
                .ToListAsync();

            ViewBag.Search = search;
            ViewBag.FilterGol = filterGol;

            return View(data);
        }

        // GET: Pendonors/Select2Search?term=budi&page=1
        // Endpoint untuk dropdown pencarian Select2 di form "Input Riwayat Donor".
        // Format response mengikuti kontrak Select2 AJAX (results + pagination.more)
        // sehingga tinggal dipakai lewat:
        //
        //   $('#namaPendonor').select2({
        //       ajax: {
        //           url: '@Url.Action("Select2Search", "Pendonors")',
        //           dataType: 'json',
        //           delay: 250,
        //           data: params => ({ term: params.term, page: params.page || 1 }),
        //           processResults: data => data
        //       },
        //       placeholder: 'Ketik nama atau ID pendonor...',
        //       minimumInputLength: 1
        //   });
        //
        // Saat item dipilih, event 'select2:select' membawa e.params.data yang berisi
        // golonganDarah / rhesus / noHp / alamat, sehingga field Golongan Darah & Rhesus
        // di form Riwayat Donor bisa langsung di-autofill dari data pendonor terdaftar.
        [HttpGet]
        public async Task<IActionResult> Select2Search(string? term, int page = 1)
        {
            const int pageSize = 10;
            var query = _context.Pendonors.AsQueryable();

            if (!string.IsNullOrWhiteSpace(term))
            {
                query = query.Where(p =>
                    p.NamaLengkap.Contains(term) ||
                    p.NoHp.Contains(term) ||
                    p.Id.ToString() == term);
            }

            var total = await query.CountAsync();

            var items = await query
                .OrderBy(p => p.NamaLengkap)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new
                {
                    id = p.Id,
                    text = p.NamaLengkap + " — " + p.GolonganDarah + p.Rhesus + " (" + p.NoHp + ")",
                    namaLengkap = p.NamaLengkap,
                    golonganDarah = p.GolonganDarah,
                    rhesus = p.Rhesus,
                    noHp = p.NoHp,
                    alamat = p.Alamat,
                    status = p.Status
                })
                .ToListAsync();

            return Json(new
            {
                results = items,
                pagination = new { more = (page * pageSize) < total }
            });
        }

        // GET: Pendonors/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var pendonor = await _context.Pendonors.FirstOrDefaultAsync(m => m.Id == id);
            if (pendonor == null) return NotFound();
            return View(pendonor);
        }

        // GET: Pendonors/Create
        public IActionResult Create()
        {
            SetDropdowns();
            return View();
        }

        // POST: Pendonors/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("NamaLengkap,Umur,JenisKelamin,GolonganDarah,Rhesus,Alamat,NoHp,Email,TanggalLahir,Keterangan")] Pendonor pendonor)
        {
            if (ModelState.IsValid)
            {
                pendonor.TanggalDaftar = DateTime.Now;
                pendonor.Status = "Aktif";
                _context.Add(pendonor);
                await _context.SaveChangesAsync();

                await _auditService.LogAsync(
                    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    User.Identity?.Name, "Create", "Pendonors", pendonor.Id.ToString());

                TempData["SweetSuccess"] = $"Pendonor {pendonor.NamaLengkap} berhasil ditambahkan!";
                return RedirectToAction(nameof(Index));
            }
            SetDropdowns();
            return View(pendonor);
        }

        // GET: Pendonors/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();
            var pendonor = await _context.Pendonors.FindAsync(id);
            if (pendonor == null) return NotFound();
            SetDropdowns();
            return View(pendonor);
        }

        // POST: Pendonors/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,NamaLengkap,Umur,JenisKelamin,GolonganDarah,Rhesus,Alamat,NoHp,Email,TanggalLahir,TanggalDaftar,DonorTerakhir,Status,Keterangan")] Pendonor pendonor)
        {
            if (id != pendonor.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(pendonor);
                    await _context.SaveChangesAsync();

                    await _auditService.LogAsync(
                        User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                        User.Identity?.Name, "Update", "Pendonors", pendonor.Id.ToString());

                    TempData["SweetSuccess"] = $"Data {pendonor.NamaLengkap} berhasil diperbarui!";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Pendonors.Any(e => e.Id == pendonor.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            SetDropdowns();
            return View(pendonor);
        }

        // GET: Pendonors/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();
            var pendonor = await _context.Pendonors.FirstOrDefaultAsync(m => m.Id == id);
            if (pendonor == null) return NotFound();
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
                await _auditService.LogAsync(
                    User.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value,
                    User.Identity?.Name, "Delete", "Pendonors", id.ToString(),
                    $"Nama: {pendonor.NamaLengkap}");

                _context.Pendonors.Remove(pendonor);
                await _context.SaveChangesAsync();
                TempData["SweetSuccess"] = $"Pendonor {pendonor.NamaLengkap} berhasil dihapus!";
            }
            return RedirectToAction(nameof(Index));
        }

        private void SetDropdowns()
        {
            ViewBag.GolonganDarahList = new List<string> { "A", "B", "AB", "O" };
            ViewBag.RhesusList = new List<string> { "+", "-" };
            ViewBag.JenisKelaminList = new List<string> { "Laki-laki", "Perempuan" };
            ViewBag.StatusList = new List<string> { "Aktif", "Tidak Aktif" };
        }
    }
}
