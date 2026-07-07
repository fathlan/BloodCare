using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCare.Migrations
{
    /// <inheritdoc />
    public partial class AddPendaftaranJadwalIdToRiwayatDonor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "PendaftaranJadwalId",
                table: "RiwayatDonors",
                type: "int",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PendaftaranJadwalId",
                table: "RiwayatDonors");
        }
    }
}
