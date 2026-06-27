using System;
using System.ComponentModel.DataAnnotations;

namespace BloodCare.Models
{
    public class StokDarah
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Golongan Darah")]
        public string GolonganDarah { get; set; } = string.Empty;

        [Required]
        public string Rhesus { get; set; } = string.Empty;

        [Required]
        [Range(1, 1000)]
        [Display(Name = "Jumlah Kantong")]
        public int JumlahKantong { get; set; }

        [Required]
        [Display(Name = "Tanggal Masuk")]
        public DateTime TanggalMasuk { get; set; }

        [Required]
        [Display(Name = "Tanggal Kadaluarsa")]
        public DateTime TanggalKadaluarsa { get; set; }
    }
}