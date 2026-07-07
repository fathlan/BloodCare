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
    public class PendaftaranJadwalController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IAuditService _auditService;
        private readonly UserManager<ApplicationUser> _userManager;

        public PendaftaranJadwalController(
            ApplicationDbContext context,
            IAuditService auditService,
            UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _auditService = auditService;
            _userManager = userManager;
        }

        // PENDONOR: daftar ke satu jadwal donor (UC007)
        [Authorize(Roles = "Pendonor")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Daftar(int jadwalDonorId)
        {
            var jadwal = await _context.JadwalDonors.FindAsync(jadwalDonorId);
            if (jadwal == null)
            {
                TempData["SweetError"] = "Jadwal donor tidak ditemukan.";
                return RedirectToAction("Index", "JadwalDonor");
            }

            var user = await _userManager.GetUserAsync(User);

            // Skenario alternatif: sudah pernah daftar ke jadwal yang sama
            bool sudahDaftar = await _context.PendaftaranJadwals.AnyAsync(p =>
                p.JadwalDonorId == jadwalDonorId && p.PendonorUserId == user!.Id);

            if (sudahDaftar)
            {
                TempData["SweetError"] = "Anda sudah terdaftar pada jadwal donor ini.";
                return RedirectToAction("Index", "JadwalDonor");
            }

            // Kuota penuh
            int totalTerdaftar = await _context.PendaftaranJadwals.CountAsync(p => p.JadwalDonorId == jadwalDonorId);
            if (totalTerdaftar >= jadwal.Kuota)
            {
                TempData["SweetError"] = "Mohon maaf, kuota jadwal donor ini sudah penuh.";
                return RedirectToAction("Index", "JadwalDonor");
            }

            var pendaftaran = new PendaftaranJadwal
            {
                JadwalDonorId = jadwalDonorId,
                PendonorUserId = user!.Id,
                NamaPendonor = user.FullName ?? user.Email ?? "Pendonor",
                TanggalDaftar = DateTime.Now,
                StatusKehadiran = "Menunggu"
            };

            _context.PendaftaranJadwals.Add(pendaftaran);
            await _context.SaveChangesAsync();

            await _auditService.LogAsync(user.Id, user.UserName, "Create", "PendaftaranJadwal", pendaftaran.Id.ToString());

            TempData["SweetSuccess"] = $"Berhasil mendaftar ke kegiatan donor di {jadwal.Lokasi}!";
            return RedirectToAction("MyIndex");
        }

        // GET: PendaftaranJadwal/MyIndex -> daftar jadwal yang sudah didaftar Pendonor (UC008)
        [Authorize(Roles = "Pendonor")]
        public async Task<IActionResult> MyIndex()
        {
            var user = await _userManager.GetUserAsync(User);

            var data = await (from p in _context.PendaftaranJadwals
                               join j in _context.JadwalDonors on p.JadwalDonorId equals j.Id
                               where p.PendonorUserId == user!.Id
                               orderby j.Tanggal descending
                               select new PendaftaranJadwalViewItem
                               {
                                   PendaftaranId = p.Id,
                                   Lokasi = j.Lokasi,
                                   Tanggal = j.Tanggal,
                                   StatusKehadiran = p.StatusKehadiran
                               }).ToListAsync();

            return View(data);
        }

        // PETUGAS: cek kehadiran peserta jadwal (UC012)
        [Authorize(Roles = "Admin,Petugas")]
        public async Task<IActionResult> Kehadiran(int? jadwalId)
        {
            var jadwalMendatang = await _context.JadwalDonors
                .OrderByDescending(j => j.Tanggal)
                .ToListAsync();
            ViewBag.DaftarJadwal = jadwalMendatang;

            if (jadwalId == null)
            {
                jadwalId = jadwalMendatang.FirstOrDefault(j => j.Tanggal.Date == DateTime.Today)?.Id
                           ?? jadwalMendatang.FirstOrDefault()?.Id;
            }

            ViewBag.JadwalDipilih = jadwalId;

            if (jadwalId == null)
                return View(new List<PendaftaranJadwal>());

            var peserta = await _context.PendaftaranJadwals
                .Where(p => p.JadwalDonorId == jadwalId)
                .OrderBy(p => p.NamaPendonor)
                .ToListAsync();

            // Cek siapa saja yang riwayat donornya sudah pernah diinput
            // (supaya tombol Hadir/Tidak Hadir disembunyikan di view kalau sudah diinput)
            var pesertaIds = peserta.Select(p => p.Id).ToList();
            ViewBag.SudahInputIds = await _context.RiwayatDonors
                .Where(r => r.PendaftaranJadwalId != null && pesertaIds.Contains(r.PendaftaranJadwalId.Value))
                .Select(r => r.PendaftaranJadwalId!.Value)
                .ToListAsync();

            var jadwal = await _context.JadwalDonors.FindAsync(jadwalId);
            ViewBag.JadwalInfo = jadwal;

            return View(peserta);
        }

        // POST: PendaftaranJadwal/SetKehadiran -> tandai Hadir / Tidak Hadir
        [Authorize(Roles = "Admin,Petugas")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SetKehadiran(int id, string status)
        {
            var pendaftaran = await _context.PendaftaranJadwals.FindAsync(id);
            if (pendaftaran == null) return NotFound();

            var petugas = await _userManager.GetUserAsync(User);

            pendaftaran.StatusKehadiran = status; // "Hadir" atau "Tidak Hadir"
            pendaftaran.WaktuVerifikasi = DateTime.Now;
            pendaftaran.PetugasId = petugas?.Id;

            await _context.SaveChangesAsync();

            await _auditService.LogAsync(
                petugas?.Id, petugas?.UserName, "Update", "PendaftaranJadwal", id.ToString(),
                $"Status kehadiran -> {status}");

            return RedirectToAction(nameof(Kehadiran), new { jadwalId = pendaftaran.JadwalDonorId });
        }
    }

    // Kecil, cuma untuk gabungan data tampilan "Jadwal Saya" milik Pendonor
    public class PendaftaranJadwalViewItem
    {
        public int PendaftaranId { get; set; }
        public string Lokasi { get; set; } = string.Empty;
        public DateTime Tanggal { get; set; }
        public string StatusKehadiran { get; set; } = string.Empty;
    }
}
