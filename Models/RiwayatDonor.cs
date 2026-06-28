using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodCare.Models
{
    [Table("Riwayat_Donor")]
    public class RiwayatDonor
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Display(Name = "Pendonor")]
        public int PendonorId { get; set; }

        public int PetugasId { get; set; } // Sementara hardcode sampai ada fitur login

        [Required]
        [DataType(DataType.Date)]
        [Display(Name = "Tanggal Donor")]
        public DateTime TanggalDonor { get; set; }

        [Required]
        [Display(Name = "Jumlah Kantong")]
        public int JumlahKantong { get; set; }

        [Required]
        [Display(Name = "Lokasi Donor")]
        public string LokasiDonor { get; set; }

        public string Keterangan { get; set; }

        // Disimpan untuk laporan / arsip
        public string GolonganDarah { get; set; }
        public string Rhesus { get; set; }
    }
}