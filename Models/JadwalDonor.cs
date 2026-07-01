using System;
using System.ComponentModel.DataAnnotations;

namespace BloodCare.Models
{
    public class JadwalDonor
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Lokasi wajib diisi")]
        public string Lokasi { get; set; }

        [Required(ErrorMessage = "Tanggal wajib diisi")]
        [DataType(DataType.Date)]
        public DateTime Tanggal { get; set; }

        [Required(ErrorMessage = "Kuota wajib diisi")]
        [Range(1, 10000, ErrorMessage = "Kuota harus antara 1 - 10000 orang")]
        public int Kuota { get; set; }

        public string? Keterangan { get; set; }
    }
}
