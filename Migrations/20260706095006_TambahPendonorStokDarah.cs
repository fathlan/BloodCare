using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCare.Migrations
{
    /// <inheritdoc />
    public partial class TambahPendonorStokDarah : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "TanggalMasuk",
                table: "StokDarahs",
                newName: "TerakhirDiperbarui");

            migrationBuilder.RenameColumn(
                name: "NoHP",
                table: "Pendonors",
                newName: "NoHp");

            migrationBuilder.RenameColumn(
                name: "TanggalDonorTerakhir",
                table: "Pendonors",
                newName: "TanggalDaftar");

            migrationBuilder.RenameColumn(
                name: "Nama",
                table: "Pendonors",
                newName: "Status");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TanggalKadaluarsa",
                table: "StokDarahs",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AddColumn<string>(
                name: "Keterangan",
                table: "StokDarahs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "DonorTerakhir",
                table: "Pendonors",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Pendonors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Keterangan",
                table: "Pendonors",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "NamaLengkap",
                table: "Pendonors",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "TanggalLahir",
                table: "Pendonors",
                type: "datetime2",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Keterangan",
                table: "StokDarahs");

            migrationBuilder.DropColumn(
                name: "DonorTerakhir",
                table: "Pendonors");

            migrationBuilder.DropColumn(
                name: "Email",
                table: "Pendonors");

            migrationBuilder.DropColumn(
                name: "Keterangan",
                table: "Pendonors");

            migrationBuilder.DropColumn(
                name: "NamaLengkap",
                table: "Pendonors");

            migrationBuilder.DropColumn(
                name: "TanggalLahir",
                table: "Pendonors");

            migrationBuilder.RenameColumn(
                name: "TerakhirDiperbarui",
                table: "StokDarahs",
                newName: "TanggalMasuk");

            migrationBuilder.RenameColumn(
                name: "NoHp",
                table: "Pendonors",
                newName: "NoHP");

            migrationBuilder.RenameColumn(
                name: "TanggalDaftar",
                table: "Pendonors",
                newName: "TanggalDonorTerakhir");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Pendonors",
                newName: "Nama");

            migrationBuilder.AlterColumn<DateTime>(
                name: "TanggalKadaluarsa",
                table: "StokDarahs",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);
        }
    }
}
