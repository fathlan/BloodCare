using System.ComponentModel.DataAnnotations;

namespace BloodCare.Models
{
    public class StokDarah
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Golongan darah wajib dipilih")]
        [Display(Name = "Golongan Darah")]
        public string GolonganDarah { get; set; } = string.Empty;

        [Required(ErrorMessage = "Rhesus wajib dipilih")]
        public string Rhesus { get; set; } = string.Empty;

        [Required(ErrorMessage = "Jumlah kantong wajib diisi")]
        [Range(0, 10000, ErrorMessage = "Jumlah harus antara 0 - 10000")]
        [Display(Name = "Jumlah Kantong")]
        public int JumlahKantong { get; set; }

        [Display(Name = "Batas Kadaluarsa")]
        public DateTime? TanggalKadaluarsa { get; set; }

        [Display(Name = "Terakhir Diperbarui")]
        public DateTime TerakhirDiperbarui { get; set; } = DateTime.Now;

        public string? Keterangan { get; set; }
    }
}
