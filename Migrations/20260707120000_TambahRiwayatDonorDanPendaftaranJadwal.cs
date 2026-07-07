using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCare.Migrations
{
    /// <inheritdoc />
    public partial class TambahRiwayatDonorDanPendaftaranJadwal : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "PendaftaranJadwals",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    JadwalDonorId = table.Column<int>(type: "int", nullable: false),
                    PendonorUserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NamaPendonor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TanggalDaftar = table.Column<DateTime>(type: "datetime2", nullable: false),
                    StatusKehadiran = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WaktuVerifikasi = table.Column<DateTime>(type: "datetime2", nullable: true),
                    PetugasId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PendaftaranJadwals", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "RiwayatDonors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    PendonorId = table.Column<int>(type: "int", nullable: true),
                    NamaPendonor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JadwalDonorId = table.Column<int>(type: "int", nullable: true),
                    TanggalDonor = table.Column<DateTime>(type: "datetime2", nullable: false),
                    LokasiDonor = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JumlahDarahMl = table.Column<int>(type: "int", nullable: false),
                    GolonganDarah = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rhesus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CatatanTambahan = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PetugasId = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    PetugasNama = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    WaktuInput = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_RiwayatDonors", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PendaftaranJadwals");

            migrationBuilder.DropTable(
                name: "RiwayatDonors");
        }
    }
}
