using System.ComponentModel.DataAnnotations;

namespace BloodCare.ViewModels
{
    public class RegisterViewModel
    {
        [Required(ErrorMessage = "Nama lengkap wajib diisi")]
        [Display(Name = "Nama Lengkap")]
        public string FullName { get; set; } = string.Empty;

        [Required(ErrorMessage = "Email wajib diisi")]
        [EmailAddress(ErrorMessage = "Format email tidak valid")]
        public string Email { get; set; } = string.Empty;

        [Required(ErrorMessage = "Password wajib diisi")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password minimal 6 karakter")]
        [DataType(DataType.Password)]
        public string Password { get; set; } = string.Empty;

        [Required(ErrorMessage = "Konfirmasi password wajib diisi")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Password dan konfirmasi tidak sama")]
        [Display(Name = "Konfirmasi Password")]
        public string ConfirmPassword { get; set; } = string.Empty;

        // ==== Data pendukung Pendonor (UC001) ====
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

        [Display(Name = "Tanggal Lahir")]
        [DataType(DataType.Date)]
        public DateTime? TanggalLahir { get; set; }
    }
}
