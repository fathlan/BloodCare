using System.ComponentModel.DataAnnotations;

namespace BloodCare.Models
{
    // Pendaftaran Pendonor ke satu Jadwal Donor (UC007), lalu dicek kehadirannya
    // oleh Petugas di lokasi (UC012) sebelum riwayat donornya diinput.
    public class PendaftaranJadwal
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "ID Jadwal Donor")]
        public int JadwalDonorId { get; set; }

        // Id akun ApplicationUser (AspNetUsers) yang mendaftar, supaya "Riwayat Donor Saya"
        // & "Jadwal Saya" bisa difilter per akun yang login.
        [Required]
        [Display(Name = "ID Akun Pendonor")]
        public string PendonorUserId { get; set; } = string.Empty;

        [Display(Name = "Nama Pendonor")]
        public string NamaPendonor { get; set; } = string.Empty;

        [Display(Name = "Tanggal Daftar")]
        public DateTime TanggalDaftar { get; set; } = DateTime.Now;

        // Menunggu -> Hadir / Tidak Hadir (diisi Petugas saat kegiatan berlangsung, UC012)
        public string StatusKehadiran { get; set; } = "Menunggu";

        [Display(Name = "Waktu Verifikasi")]
        public DateTime? WaktuVerifikasi { get; set; }

        // Petugas mana yang memverifikasi kehadiran
        [Display(Name = "Diverifikasi Oleh")]
        public string? PetugasId { get; set; }
    }
}
