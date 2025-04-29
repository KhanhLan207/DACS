using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBtwice : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "RouteStops",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DepartureTimesJson",
                table: "BusRoutes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Frequency",
                table: "BusRoutes",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDate",
                table: "BusRoutes",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<TimeSpan>(
                name: "TravelTime",
                table: "BusRoutes",
                type: "time",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Address",
                table: "RouteStops");

            migrationBuilder.DropColumn(
                name: "DepartureTimesJson",
                table: "BusRoutes");

            migrationBuilder.DropColumn(
                name: "Frequency",
                table: "BusRoutes");

            migrationBuilder.DropColumn(
                name: "StartDate",
                table: "BusRoutes");

            migrationBuilder.DropColumn(
                name: "TravelTime",
                table: "BusRoutes");
        }
    }
}
