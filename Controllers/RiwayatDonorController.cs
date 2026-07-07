using BloodCare.Data;
using BloodCare.Models;
using BloodCare.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{

    [Authorize]
    public class RiwayatDonorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly UserManager<ApplicationUser> _userManager;

        public RiwayatDonorController(
            ApplicationDbContext context,
            IAuditService auditService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _auditService = auditService;
            _userManager = userManager;
        }

        // GET: RiwayatDonor
        public async Task<IActionResult> Index()
        {
            // Pendonor  hanya lihat riwayat miliknya sendiri (UC008/UC009)
            if (User.IsInRole("Pendonor") && !User.IsInRole("Petugas") && !User.IsInRole("Admin"))
            {
                var user = await _userManager.GetUserAsync(User);
                var idPendonor = user?.IdPendonor;

                var riwayatSaya = idPendonor.HasValue
                    ? await _context.RiwayatDonors
                        .Where(r => r.PendonorId == idPendonor.Value)
                        .OrderByDescending(r => r.TanggalDonor)
                        .ToListAsync()
                    : new List<RiwayatDonor>();

                return View("IndexPendonor", riwayatSaya);
            }

            // Petugas admin form input + daftar input terkini
            if (!User.IsInRole("Petugas") && !User.IsInRole("Admin"))
                return Forbid();

            var terkini = await _context.RiwayatDonors
                .OrderByDescending(r => r.WaktuInput)
                .Take(10)
                .ToListAsync();

            var hariIni = DateTime.Today;
            ViewBag.TotalKantongHariIni = await _context.RiwayatDonors
                .CountAsync(r => r.TanggalDonor.Date == hariIni);

            // Target harian statis  bisa dipindah ke config kalau perlu
            const int targetHarian = 50;
            int totalHariIni = await _context.RiwayatDonors.CountAsync(r => r.TanggalDonor.Date == hariIni);
            ViewBag.TargetHarianPersen = targetHarian == 0 ? 0 : Math.Min(100, (int)(totalHariIni * 100.0 / targetHarian));

            // Daftar jadwal donor yang masih aktif, untuk dropdown Lokasi Donor
            // pada mode "Pendonor Baru" di form input manual
            ViewBag.DaftarJadwal = await _context.JadwalDonors
                .Where(j => j.Tanggal.Date >= hariIni)
                .OrderBy(j => j.Tanggal)
                .Select(j => new { j.Id, j.Lokasi, j.Tanggal })
                .ToListAsync();

            return View(terkini);
        }

        // GET: RiwayatDonor/Create
        [Authorize(Roles = "Admin,Petugas")]
        public IActionResult Create(int? pendaftaranId)
        {
            var model = new RiwayatDonor
            {
                TanggalDonor = DateTime.Today
            };

            // Kalau datang dari halaman verifikasi kehadiran (UC012), prefill data pendonor & jadwal
            if (pendaftaranId.HasValue)
            {
                var pendaftaran = _context.PendaftaranJadwals.Find(pendaftaranId.Value);
                if (pendaftaran != null)
                {
                    var jadwal = _context.JadwalDonors.Find(pendaftaran.JadwalDonorId);
                    model.NamaPendonor = pendaftaran.NamaPendonor;
                    model.JadwalDonorId = pendaftaran.JadwalDonorId;
                    model.LokasiDonor = jadwal?.Lokasi ?? string.Empty;
                    model.PendaftaranJadwalId = pendaftaran.Id;

                    var akun = _context.Users.FirstOrDefault(u => u.Id == pendaftaran.PendonorUserId);
                    if (akun?.IdPendonor != null)
                    {
                        model.PendonorId = akun.IdPendonor;

                        // Auto-isi Golongan Darah & Rhesus dari data master Pendonor
                        var pendonorData = _context.Pendonors.Find(akun.IdPendonor.Value);
                        if (pendonorData != null)
                        {
                            model.GolonganDarah = pendonorData.GolonganDarah;
                            model.Rhesus = pendonorData.Rhesus;
                        }
                    }
                }
            }

            return View(model);
        }

        // POST: RiwayatDonor/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Petugas")]
        public async Task<IActionResult> Create(
            [Bind("PendonorId,NamaPendonor,JadwalDonorId,PendaftaranJadwalId,TanggalDonor,LokasiDonor,JumlahDarahMl,GolonganDarah,Rhesus,CatatanTambahan")] RiwayatDonor riwayat)
        {
            if (!ModelState.IsValid) return View(riwayat);

            var petugas = await _userManager.GetUserAsync(User);
            riwayat.PetugasId = petugas?.Id;
            riwayat.PetugasNama = petugas?.FullName ?? petugas?.Email;
            riwayat.WaktuInput = DateTime.Now;
            riwayat.Status = "Terverifikasi";

            _context.RiwayatDonors.Add(riwayat);

            //UC005: Update Stock Darah (otomatis, +1 kantong per input)
            var stok = await _context.StokDarahs.FirstOrDefaultAsync(s =>
                s.GolonganDarah == riwayat.GolonganDarah && s.Rhesus == riwayat.Rhesus);

            if (stok == null)
            {
                stok = new StokDarah
                {
                    GolonganDarah = riwayat.GolonganDarah,
                    Rhesus = riwayat.Rhesus,
                    JumlahKantong = 1,
                    TerakhirDiperbarui = DateTime.Now
                };
                _context.StokDarahs.Add(stok);
            }
            else
            {
                stok.JumlahKantong += 1;
                stok.TerakhirDiperbarui = DateTime.Now;
            }

            // Update juga data master Pendonor (DonorTerakhir) kalau linknya ada
            if (riwayat.PendonorId.HasValue)
            {
                var pendonor = await _context.Pendonors.FindAsync(riwayat.PendonorId.Value);
                if (pendonor != null) pendonor.DonorTerakhir = riwayat.TanggalDonor;
            }

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                petugas?.Id, petugas?.UserName, "Create", "RiwayatDonor", riwayat.Id.ToString(),
                $"Update stok {riwayat.GolonganDarah}{riwayat.Rhesus} +1 kantong");

            TempData["SweetSuccess"] = $"Riwayat donor {riwayat.NamaPendonor} berhasil disimpan & stok darah diperbarui!";

            // Kalau berasal dari alur Kehadiran, kembali ke halaman Kehadiran jadwal itu
            if (riwayat.PendaftaranJadwalId.HasValue)
            {
                return RedirectToAction("Kehadiran", "PendaftaranJadwal", new { jadwalId = riwayat.JadwalDonorId });
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: RiwayatDonor/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();
            var riwayat = await _context.RiwayatDonors.FindAsync(id);
            if (riwayat == null) return NotFound();
            return View(riwayat);
        }
    }
}
