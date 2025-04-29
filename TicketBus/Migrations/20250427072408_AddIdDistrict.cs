using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class AddIdDistrict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdDistrict",
                table: "BusRoutes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BusRoutes_IdDistrict",
                table: "BusRoutes",
                column: "IdDistrict");

            migrationBuilder.AddForeignKey(
                name: "FK_BusRoutes_Districts_IdDistrict",
                table: "BusRoutes",
                column: "IdDistrict",
                principalTable: "Districts",
                principalColumn: "IdDistrict");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusRoutes_Districts_IdDistrict",
                table: "BusRoutes");

            migrationBuilder.DropIndex(
                name: "IX_BusRoutes_IdDistrict",
                table: "BusRoutes");

            migrationBuilder.DropColumn(
                name: "IdDistrict",
                table: "BusRoutes");
        }
    }
}
