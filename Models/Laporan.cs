using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace BloodCare.Models
{
    [Table("Laporan")]
    public class Laporan
    {
        [Key]
        public int Id { get; set; }
        public int AdminId { get; set; } // Sementara hardcode
        public string JenisLaporan { get; set; }

        [DataType(DataType.Date)]
        public DateTime TanggalLaporan { get; set; }
        public string Keterangan { get; set; }
    }
}