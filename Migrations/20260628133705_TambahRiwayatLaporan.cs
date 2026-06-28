using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCare.Migrations
{
    /// <inheritdoc />
    public partial class TambahRiwayatLaporan : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Laporan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AdminId = table.Column<int>(type: "int", nullable: false),
                    JenisLaporan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TanggalLaporan = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laporan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Riwayat_Donor",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PendonorId = table.Column<int>(type: "int", nullable: false),
                    PetugasId = table.Column<int>(type: "int", nullable: false),
                    TanggalDonor = table.Column<DateTime>(type: "datetime2", nullable: false),
                    JumlahKantong = table.Column<int>(type: "int", nullable: false),
                    LokasiDonor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GolonganDarah = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rhesus = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Riwayat_Donor", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Laporan");

            migrationBuilder.DropTable(
                name: "Riwayat_Donor");
        }
    }
}
