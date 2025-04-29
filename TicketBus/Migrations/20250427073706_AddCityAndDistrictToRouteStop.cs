using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class AddCityAndDistrictToRouteStop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCity",
                table: "RouteStops",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdDistrict",
                table: "RouteStops",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_IdCity",
                table: "RouteStops",
                column: "IdCity");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_IdDistrict",
                table: "RouteStops",
                column: "IdDistrict");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStops_Cities_IdCity",
                table: "RouteStops",
                column: "IdCity",
                principalTable: "Cities",
                principalColumn: "IdCity");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStops_Districts_IdDistrict",
                table: "RouteStops",
                column: "IdDistrict",
                principalTable: "Districts",
                principalColumn: "IdDistrict");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteStops_Cities_IdCity",
                table: "RouteStops");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteStops_Districts_IdDistrict",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_RouteStops_IdCity",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_RouteStops_IdDistrict",
                table: "RouteStops");

            migrationBuilder.DropColumn(
                name: "IdCity",
                table: "RouteStops");

            migrationBuilder.DropColumn(
                name: "IdDistrict",
                table: "RouteStops");
        }
    }
}
