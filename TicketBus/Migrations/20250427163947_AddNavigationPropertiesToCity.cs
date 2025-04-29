using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class AddNavigationPropertiesToCity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusRoutes_Cities_IdEndCity",
                table: "BusRoutes");

            migrationBuilder.AlterColumn<int>(
                name: "IdStartCity",
                table: "BusRoutes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<int>(
                name: "IdEndCity",
                table: "BusRoutes",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_BusRoutes_Cities_IdEndCity",
                table: "BusRoutes",
                column: "IdEndCity",
                principalTable: "Cities",
                principalColumn: "IdCity");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BusRoutes_Cities_IdEndCity",
                table: "BusRoutes");

            migrationBuilder.AlterColumn<int>(
                name: "IdStartCity",
                table: "BusRoutes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdEndCity",
                table: "BusRoutes",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_BusRoutes_Cities_IdEndCity",
                table: "BusRoutes",
                column: "IdEndCity",
                principalTable: "Cities",
                principalColumn: "IdCity",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
