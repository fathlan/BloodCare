using BloodCare.Data;
using BloodCare.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BloodCare.Controllers
{
    [Authorize(Roles = "Admin,Petugas")]
    public class NotifikasiController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<ApplicationUser> _userManager;

        public NotifikasiController(ApplicationDbContext context, UserManager<ApplicationUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // Halaman penuh "Lihat Semua Notifikasi" (tetap dipertahankan sebagai fallback)
        public async Task<IActionResult> Index()
        {
            var sorted = await BuildNotifikasiList();

            ViewBag.TotalNotif = sorted.Count;
            ViewBag.TotalUrgent = sorted.Count(n => n.IsUrgent);
            ViewBag.TotalBelumDibaca = sorted.Count(n => !n.IsRead);

            return View(sorted);
        }

        // GET: Notifikasi/Dropdown -> dipanggil AJAX saat bel notifikasi diklik, tanpa pindah halaman
        [HttpGet]
        public async Task<IActionResult> Dropdown()
        {
            var sorted = await BuildNotifikasiList();
            return PartialView("_NotifikasiDropdown", sorted.Take(8).ToList());
        }

        // GET: Notifikasi/UnreadCount -> dipakai untuk angka badge di bel notifikasi
        [HttpGet]
        public async Task<IActionResult> UnreadCount()
        {
            var sorted = await BuildNotifikasiList();
            return Json(new { count = sorted.Count(n => !n.IsRead) });
        }

        // POST: Notifikasi/MarkAsRead -> tandai satu notifikasi sudah dibaca (AJAX, tanpa reload)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsRead(string id)
        {
            if (string.IsNullOrWhiteSpace(id)) return BadRequest();

            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            bool sudahAda = await _context.NotifikasiDibacas
                .AnyAsync(n => n.UserId == user.Id && n.NotifId == id);

            if (!sudahAda)
            {
                _context.NotifikasiDibacas.Add(new NotifikasiDibaca
                {
                    UserId = user.Id,
                    NotifId = id,
                    WaktuDibaca = DateTime.Now
                });
                await _context.SaveChangesAsync();
            }

            return Json(new { success = true });
        }

        // POST: Notifikasi/MarkAllAsRead -> tandai semua notifikasi yang sedang tampil sebagai sudah dibaca
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAllAsRead()
        {
            var user = await _userManager.GetUserAsync(User);
            if (user == null) return Unauthorized();

            var semua = await BuildNotifikasiList();
            var sudahDibacaIds = (await _context.NotifikasiDibacas
                .Where(n => n.UserId == user.Id)
                .Select(n => n.NotifId)
                .ToListAsync())
                .ToHashSet();

            var belumDibaca = semua.Where(n => !sudahDibacaIds.Contains(n.Id));

            foreach (var n in belumDibaca)
            {
                _context.NotifikasiDibacas.Add(new NotifikasiDibaca
                {
                    UserId = user.Id,
                    NotifId = n.Id,
                    WaktuDibaca = DateTime.Now
                });
            }

            await _context.SaveChangesAsync();
            return Json(new { success = true });
        }

        // ============================================================
        // Bangun daftar notifikasi sintetis dari data live + status baca per user
        // ============================================================
        private async Task<List<NotifItem>> BuildNotifikasiList()
        {
            var notifs = new List<NotifItem>();

            // 1. Stok darah kritis (< 10 kantong)
            var stokKritis = await _context.StokDarahs
                .Where(s => s.JumlahKantong < 10)
                .ToListAsync();

            foreach (var s in stokKritis)
            {
                notifs.Add(new NotifItem
                {
                    Id = $"stok-{s.Id}",
                    Judul = $"Stok Darah {s.GolonganDarah}{s.Rhesus} Menipis",
                    Pesan = $"Persediaan darah golongan {s.GolonganDarah} rhesus {s.Rhesus} " +
                            $"tersisa {s.JumlahKantong} kantong. Mohon segera update jadwal donor mendesak.",
                    Waktu = s.TerakhirDiperbarui,
                    Tipe = "stok",
                    IsUrgent = s.JumlahKantong < 5
                });
            }

            // 2. Permintaan darah darurat (>= 5 kantong & pending)
            var permintaanDarurat = await _context.PermintaanDarahs
                .Where(p => p.JumlahKebutuhan >= 5 && p.Status == "Pending")
                .OrderByDescending(p => p.TanggalPermintaan)
                .ToListAsync();

            foreach (var p in permintaanDarurat)
            {
                notifs.Add(new NotifItem
                {
                    Id = $"permintaan-{p.Id}",
                    Judul = "Permintaan Darah Darurat!",
                    Pesan = $"Pasien {p.NamaPasien} di {p.RumahSakit} memerlukan {p.JumlahKebutuhan} " +
                            $"kantong darah golongan {p.GolonganDarah}{p.Rhesus} segera.",
                    Waktu = p.TanggalPermintaan,
                    Tipe = "permintaan",
                    IsUrgent = true
                });
            }

            // 3. Jadwal donor akan datang dalam 3 hari
            var jadwalDekat = await _context.JadwalDonors
                .Where(j => j.Tanggal >= DateTime.Today && j.Tanggal <= DateTime.Today.AddDays(3))
                .ToListAsync();

            foreach (var j in jadwalDekat)
            {
                notifs.Add(new NotifItem
                {
                    Id = $"jadwal-{j.Id}",
                    Judul = "Jadwal Donor Keliling Baru",
                    Pesan = $"Jadwal kegiatan donor di {j.Lokasi} pada tanggal " +
                            $"{j.Tanggal:dd MMMM yyyy}. Kuota: {j.Kuota} orang.",
                    Waktu = j.Tanggal,
                    Tipe = "jadwal",
                    IsUrgent = false
                });
            }

            // --- Tempelkan status baca per user yang login ---
            var user = await _userManager.GetUserAsync(User);
            if (user != null)
            {
                var sudahDibacaIds = (await _context.NotifikasiDibacas
                    .Where(n => n.UserId == user.Id)
                    .Select(n => n.NotifId)
                    .ToListAsync())
                    .ToHashSet();

                foreach (var n in notifs)
                    n.IsRead = sudahDibacaIds.Contains(n.Id);
            }

            return notifs
                .OrderByDescending(n => n.IsUrgent)
                .ThenByDescending(n => n.Waktu)
                .ToList();
        }
    }

    // Model notifikasi sintetis (menggantikan dynamic/anonymous supaya bisa di-serialize & dipakai partial view)
    public class NotifItem
    {
        public string Id { get; set; } = string.Empty;
        public string Judul { get; set; } = string.Empty;
        public string Pesan { get; set; } = string.Empty;
        public DateTime Waktu { get; set; }
        public string Tipe { get; set; } = string.Empty;
        public bool IsUrgent { get; set; }
        public bool IsRead { get; set; }
    }
}
