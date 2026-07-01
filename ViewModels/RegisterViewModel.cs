using System.ComponentModel.DataAnnotations;

namespace BloodCare.ViewModels
{
    // Register di sini otomatis jadi role "Pendonor"
    // Admin & Petugas dibuat manual oleh Admin, tidak lewat form register publik
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
    }
}
