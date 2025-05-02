using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateRouteStop : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusRoutes_Districts_IdDistrict",
                table: "BusRoutes");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteStops_Districts_IdDistrict",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_RouteStops_IdDistrict",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_BusRoutes_IdDistrict",
                table: "BusRoutes");

            migrationBuilder.DropColumn(
                name: "IdDistrict",
                table: "RouteStops");

            migrationBuilder.DropColumn(
                name: "IdDistrict",
                table: "BusRoutes");

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "BusRoutes",
                type: "datetime2",
                nullable: true,
                oldClrType: typeof(DateTime),
                oldType: "datetime2");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "BusRoutes",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdDistrict",
                table: "RouteStops",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<DateTime>(
                name: "StartDate",
                table: "BusRoutes",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified),
                oldClrType: typeof(DateTime),
                oldType: "datetime2",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "BusRoutes",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "IdDistrict",
                table: "BusRoutes",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_IdDistrict",
                table: "RouteStops",
                column: "IdDistrict");

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

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStops_Districts_IdDistrict",
                table: "RouteStops",
                column: "IdDistrict",
                principalTable: "Districts",
                principalColumn: "IdDistrict");
        }
    }
}
