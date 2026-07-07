using System.ComponentModel.DataAnnotations;

namespace BloodCare.Models
{
    public class RiwayatDonor
    {
        public int Id { get; set; }

        // Kalau pendonor dipilih dari data terdaftar (Select2), Id-nya disimpan di sini.
        // Nullable karena bisa juga pendonor walk-in yang belum terdaftar (isi manual).
        [Display(Name = "ID Pendonor")]
        public int? PendonorId { get; set; }

        [Required(ErrorMessage = "Nama pendonor wajib diisi")]
        [Display(Name = "Nama Pendonor")]
        public string NamaPendonor { get; set; } = string.Empty;

        // Jejak asal: kalau riwayat donor ini datang dari alur verifikasi kehadiran
        // (PendaftaranJadwalController -> Kehadiran), Id pendaftarannya disimpan di sini.
        // Nullable karena riwayat donor walk-in (manual, tanpa daftar jadwal) tidak punya ini.
        [Display(Name = "ID Pendaftaran Jadwal")]
        public int? PendaftaranJadwalId { get; set; }

        [Display(Name = "ID Jadwal Donor")]
        public int? JadwalDonorId { get; set; }

        [Required(ErrorMessage = "Tanggal donor wajib diisi")]
        [DataType(DataType.Date)]
        [Display(Name = "Tanggal Donor")]
        public DateTime TanggalDonor { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Lokasi donor wajib diisi")]
        [Display(Name = "Lokasi Donor")]
        public string LokasiDonor { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jumlah darah wajib diisi")]
        [Range(50, 1000, ErrorMessage = "Jumlah darah harus antara 50 - 1000 ml")]
        [Display(Name = "Jumlah Darah (ml)")]
        public int JumlahDarahMl { get; set; }

        [Required(ErrorMessage = "Golongan darah wajib dipilih")]
        [Display(Name = "Golongan Darah")]
        public string GolonganDarah { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rhesus wajib dipilih")]
        public string Rhesus { get; set; } = string.Empty;

        [Display(Name = "Catatan Tambahan")]
        public string? CatatanTambahan { get; set; }

        // Terverifikasi = sudah dicek Admin/Petugas senior, Proses = baru diinput
        public string Status { get; set; } = "Proses";

        // Siapa Petugas yang menginput
        [Display(Name = "Diinput Oleh")]
        public string? PetugasId { get; set; }

        [Display(Name = "Nama Petugas")]
        public string? PetugasNama { get; set; }

        [Display(Name = "Waktu Input")]
        public DateTime WaktuInput { get; set; } = DateTime.Now;
    }
}