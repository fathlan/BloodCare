using System;
using System.ComponentModel.DataAnnotations;

namespace BloodCare.Models
{
    public class PermintaanDarah
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Nama pasien wajib diisi")]
        [Display(Name = "Nama Pasien")]
        public string NamaPasien { get; set; }

        [Required(ErrorMessage = "Nama rumah sakit wajib diisi")]
        [Display(Name = "Rumah Sakit")]
        public string RumahSakit { get; set; }

        [Required(ErrorMessage = "Golongan darah wajib dipilih")]
        [Display(Name = "Golongan Darah")]
        public string GolonganDarah { get; set; }

        [Required(ErrorMessage = "Rhesus wajib dipilih")]
        public string Rhesus { get; set; }

        [Required(ErrorMessage = "Jumlah kebutuhan wajib diisi")]
        [Range(1, 100, ErrorMessage = "Jumlah harus antara 1 - 100 kantong")]
        [Display(Name = "Jumlah Kebutuhan (kantong)")]
        public int JumlahKebutuhan { get; set; }

        public string Status { get; set; } = "Pending";

        public DateTime TanggalPermintaan { get; set; } = DateTime.Now;
    }
}
