using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class RemoveCoachAndRouteFromTicket : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_BusRoutes_IdRoute",
                table: "Tickets");

            migrationBuilder.DropForeignKey(
                name: "FK_Tickets_Coaches_IdCoach",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_IdCoach",
                table: "Tickets");

            migrationBuilder.DropIndex(
                name: "IX_Tickets_IdRoute",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IdCoach",
                table: "Tickets");

            migrationBuilder.DropColumn(
                name: "IdRoute",
                table: "Tickets");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdCoach",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdRoute",
                table: "Tickets",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdCoach",
                table: "Tickets",
                column: "IdCoach");

            migrationBuilder.CreateIndex(
                name: "IX_Tickets_IdRoute",
                table: "Tickets",
                column: "IdRoute");

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_BusRoutes_IdRoute",
                table: "Tickets",
                column: "IdRoute",
                principalTable: "BusRoutes",
                principalColumn: "IdRoute",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Tickets_Coaches_IdCoach",
                table: "Tickets",
                column: "IdCoach",
                principalTable: "Coaches",
                principalColumn: "IdCoach",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
