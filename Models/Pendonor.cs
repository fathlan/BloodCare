using System.ComponentModel.DataAnnotations;

namespace BloodCare.Models
{
    public class Pendonor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama lengkap wajib diisi")]
        [Display(Name = "Nama Lengkap")]
        public string NamaLengkap { get; set; } = string.Empty;

        [Required(ErrorMessage = "Umur wajib diisi")]
        [Range(17, 65, ErrorMessage = "Umur pendonor harus antara 17-65 tahun")]
        public int Umur { get; set; }

        [Required(ErrorMessage = "Jenis kelamin wajib dipilih")]
        [Display(Name = "Jenis Kelamin")]
        public string JenisKelamin { get; set; } = string.Empty;

        [Required(ErrorMessage = "Golongan darah wajib dipilih")]
        [Display(Name = "Golongan Darah")]
        public string GolonganDarah { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rhesus wajib dipilih")]
        public string Rhesus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Alamat wajib diisi")]
        public string Alamat { get; set; } = string.Empty;

        [Required(ErrorMessage = "Nomor HP wajib diisi")]
        [Display(Name = "No. HP")]
        [Phone(ErrorMessage = "Format nomor HP tidak valid")]
        public string NoHp { get; set; } = string.Empty;

        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string? Email { get; set; }

        [Display(Name = "Tanggal Lahir")]
        public DateTime? TanggalLahir { get; set; }

        [Display(Name = "Tanggal Daftar")]
        public DateTime TanggalDaftar { get; set; } = DateTime.Now;

        [Display(Name = "Donor Terakhir")]
        public DateTime? DonorTerakhir { get; set; }

        public string Status { get; set; } = "Aktif";

        public string? Keterangan { get; set; }
    }
}
