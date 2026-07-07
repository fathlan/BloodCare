using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BloodCare.Migrations
{
    /// <inheritdoc />
    public partial class TambahNotifikasiDibaca : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "NotifikasiDibacas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    NotifId = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    WaktuDibaca = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_NotifikasiDibacas", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "NotifikasiDibacas");
        }
    }
}
