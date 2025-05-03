using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDD : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "PickupName",
                table: "Pickups",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<int>(
                name: "IdCity",
                table: "Pickups",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddColumn<int>(
                name: "IdRoute",
                table: "Pickups",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdCity",
                table: "DropOffs",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AlterColumn<string>(
                name: "DropOffName",
                table: "DropOffs",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<int>(
                name: "IdRoute",
                table: "DropOffs",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Pickups_IdRoute",
                table: "Pickups",
                column: "IdRoute");

            migrationBuilder.CreateIndex(
                name: "IX_DropOffs_IdRoute",
                table: "DropOffs",
                column: "IdRoute");

            migrationBuilder.AddForeignKey(
                name: "FK_DropOffs_BusRoutes_IdRoute",
                table: "DropOffs",
                column: "IdRoute",
                principalTable: "BusRoutes",
                principalColumn: "IdRoute");

            migrationBuilder.AddForeignKey(
                name: "FK_Pickups_BusRoutes_IdRoute",
                table: "Pickups",
                column: "IdRoute",
                principalTable: "BusRoutes",
                principalColumn: "IdRoute");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DropOffs_BusRoutes_IdRoute",
                table: "DropOffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Pickups_BusRoutes_IdRoute",
                table: "Pickups");

            migrationBuilder.DropIndex(
                name: "IX_Pickups_IdRoute",
                table: "Pickups");

            migrationBuilder.DropIndex(
                name: "IX_DropOffs_IdRoute",
                table: "DropOffs");

            migrationBuilder.DropColumn(
                name: "IdRoute",
                table: "Pickups");

            migrationBuilder.DropColumn(
                name: "IdRoute",
                table: "DropOffs");

            migrationBuilder.AlterColumn<string>(
                name: "PickupName",
                table: "Pickups",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdCity",
                table: "Pickups",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdCity",
                table: "DropOffs",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "DropOffName",
                table: "DropOffs",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }
    }
}
