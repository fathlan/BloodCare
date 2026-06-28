using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using BloodCare.Models;
using BloodCare.Data;

namespace BloodCare.Controllers
{
    public class RiwayatController : Controller
    {
        private readonly ApplicationDbContext _context;

        public RiwayatController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var riwayat = await _context.RiwayatDonors.ToListAsync();
            return View(riwayat);
        }

        public IActionResult Create()
        {
            // Kirim daftar pendonor ke View untuk dijadikan Dropdown (Biar petugas gampang milih)
            ViewBag.PendonorList = new SelectList(_context.Pendonors, "Id", "Nama");
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(RiwayatDonor riwayatDonor)
        {
            // Ambil data pendonor berdasarkan pilihan di dropdown
            var pendonor = await _context.Pendonors.FindAsync(riwayatDonor.PendonorId);

            if (pendonor != null)
            {
                // 1. Lengkapi data riwayat
                riwayatDonor.TanggalDonor = DateTime.Now;
                riwayatDonor.PetugasId = 1; // Hardcode id_petugas sementara
                riwayatDonor.GolonganDarah = pendonor.GolonganDarah;
                riwayatDonor.Rhesus = pendonor.Rhesus;

                // 2. LOGIKA AUTO-UPDATE STOK DARAH 
                var stokDarah = await _context.StokDarahs
                    .FirstOrDefaultAsync(s => s.GolonganDarah == pendonor.GolonganDarah && s.Rhesus == pendonor.Rhesus);

                if (stokDarah != null)
                {
                    // Jika stok darah udah ada, tambah jumlah kantongnya
                    stokDarah.JumlahKantong += riwayatDonor.JumlahKantong;
                    _context.Update(stokDarah);
                }
                else
                {
                    // Kalau golongan darah/rhesus ini belum pernah ada di stok, bikin record baru!
                    var stokBaru = new StokDarah
                    {
                        GolonganDarah = pendonor.GolonganDarah,
                        Rhesus = pendonor.Rhesus,
                        JumlahKantong = riwayatDonor.JumlahKantong,
                        TanggalMasuk = DateTime.Now,
                        TanggalKadaluarsa = DateTime.Now.AddDays(35) // Misal darah tahan 35 hari
                    };
                    _context.StokDarahs.Add(stokBaru);
                }

                // 3. Simpan Riwayat
                _context.RiwayatDonors.Add(riwayatDonor);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Riwayat berhasil disimpan & Stok Darah terupdate otomatis!";
                return RedirectToAction(nameof(Index));
            }

            ViewBag.PendonorList = new SelectList(_context.Pendonors, "Id", "Nama", riwayatDonor.PendonorId);
            return View(riwayatDonor);
        }
    }
}