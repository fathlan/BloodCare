using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCare.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "JadwalDonors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Lokasi = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Tanggal = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Kuota = table.Column<int>(type: "int", nullable: false),
                    Keterangan = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_JadwalDonors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Pendonors",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nama = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Umur = table.Column<int>(type: "int", nullable: false),
                    JenisKelamin = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GolonganDarah = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rhesus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Alamat = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NoHP = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TanggalDonorTerakhir = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pendonors", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "PermintaanDarahs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    NamaPasien = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RumahSakit = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    GolonganDarah = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rhesus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JumlahKebutuhan = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    TanggalPermintaan = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PermintaanDarahs", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "StokDarahs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    GolonganDarah = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rhesus = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    JumlahKantong = table.Column<int>(type: "int", nullable: false),
                    TanggalMasuk = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TanggalKadaluarsa = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StokDarahs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "JadwalDonors");

            migrationBuilder.DropTable(
                name: "Pendonors");

            migrationBuilder.DropTable(
                name: "PermintaanDarahs");

            migrationBuilder.DropTable(
                name: "StokDarahs");
        }
    }
}
