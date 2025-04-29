using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateForeignKey : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "BusRouteIdRoute",
                table: "RouteStops",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_BusRouteIdRoute",
                table: "RouteStops",
                column: "BusRouteIdRoute");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStops_BusRoutes_BusRouteIdRoute",
                table: "RouteStops",
                column: "BusRouteIdRoute",
                principalTable: "BusRoutes",
                principalColumn: "IdRoute");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_RouteStops_BusRoutes_BusRouteIdRoute",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_RouteStops_BusRouteIdRoute",
                table: "RouteStops");

            migrationBuilder.DropColumn(
                name: "BusRouteIdRoute",
                table: "RouteStops");
        }
    }
}
