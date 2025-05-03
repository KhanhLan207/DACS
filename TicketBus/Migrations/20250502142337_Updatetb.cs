using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class Updatetb : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_RegistForms_RegistFormId",
                table: "Brands");

            migrationBuilder.DropForeignKey(
                name: "FK_DropOffs_RegistForms_IdRegist",
                table: "DropOffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Pickups_RegistForms_IdRegist",
                table: "Pickups");

            migrationBuilder.DropForeignKey(
                name: "FK_RouteStops_BusRoutes_BusRouteIdRoute",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_RouteStops_BusRouteIdRoute",
                table: "RouteStops");

            migrationBuilder.DropIndex(
                name: "IX_RegistForms_IdBrand",
                table: "RegistForms");

            migrationBuilder.DropIndex(
                name: "IX_Pickups_IdRegist",
                table: "Pickups");

            migrationBuilder.DropIndex(
                name: "IX_DropOffs_IdRegist",
                table: "DropOffs");

            migrationBuilder.DropIndex(
                name: "IX_Brands_RegistFormId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "BusRouteIdRoute",
                table: "RouteStops");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Pickups");

            migrationBuilder.DropColumn(
                name: "IdRegist",
                table: "Pickups");

            migrationBuilder.DropColumn(
                name: "PickupCode",
                table: "Pickups");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "DropOffs");

            migrationBuilder.DropColumn(
                name: "DropOffCode",
                table: "DropOffs");

            migrationBuilder.DropColumn(
                name: "IdRegist",
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

            migrationBuilder.AddColumn<int>(
                name: "IdBrand",
                table: "Pickups",
                type: "int",
                nullable: false,
                defaultValue: 0);

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

            migrationBuilder.AddColumn<int>(
                name: "IdBrand",
                table: "DropOffs",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Brands",
                type: "nvarchar(450)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "NameBrand",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "BrandCode",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_RegistForms_IdBrand",
                table: "RegistForms",
                column: "IdBrand",
                unique: true,
                filter: "[IdBrand] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Pickups_IdBrand",
                table: "Pickups",
                column: "IdBrand");

            migrationBuilder.CreateIndex(
                name: "IX_DropOffs_IdBrand",
                table: "DropOffs",
                column: "IdBrand");

            migrationBuilder.AddForeignKey(
                name: "FK_DropOffs_Brands_IdBrand",
                table: "DropOffs",
                column: "IdBrand",
                principalTable: "Brands",
                principalColumn: "IdBrand",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Pickups_Brands_IdBrand",
                table: "Pickups",
                column: "IdBrand",
                principalTable: "Brands",
                principalColumn: "IdBrand",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DropOffs_Brands_IdBrand",
                table: "DropOffs");

            migrationBuilder.DropForeignKey(
                name: "FK_Pickups_Brands_IdBrand",
                table: "Pickups");

            migrationBuilder.DropIndex(
                name: "IX_RegistForms_IdBrand",
                table: "RegistForms");

            migrationBuilder.DropIndex(
                name: "IX_Pickups_IdBrand",
                table: "Pickups");

            migrationBuilder.DropIndex(
                name: "IX_DropOffs_IdBrand",
                table: "DropOffs");

            migrationBuilder.DropColumn(
                name: "IdBrand",
                table: "Pickups");

            migrationBuilder.DropColumn(
                name: "IdBrand",
                table: "DropOffs");

            migrationBuilder.AddColumn<int>(
                name: "BusRouteIdRoute",
                table: "RouteStops",
                type: "int",
                nullable: true);

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

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Pickups",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdRegist",
                table: "Pickups",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "PickupCode",
                table: "Pickups",
                type: "nvarchar(max)",
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

            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "DropOffs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "DropOffCode",
                table: "DropOffs",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "IdRegist",
                table: "DropOffs",
                type: "int",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "UserId",
                table: "Brands",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "PhoneNumber",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "NameBrand",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Image",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "BrandCode",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<string>(
                name: "Address",
                table: "Brands",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.CreateIndex(
                name: "IX_RouteStops_BusRouteIdRoute",
                table: "RouteStops",
                column: "BusRouteIdRoute");

            migrationBuilder.CreateIndex(
                name: "IX_RegistForms_IdBrand",
                table: "RegistForms",
                column: "IdBrand");

            migrationBuilder.CreateIndex(
                name: "IX_Pickups_IdRegist",
                table: "Pickups",
                column: "IdRegist");

            migrationBuilder.CreateIndex(
                name: "IX_DropOffs_IdRegist",
                table: "DropOffs",
                column: "IdRegist");

            migrationBuilder.CreateIndex(
                name: "IX_Brands_RegistFormId",
                table: "Brands",
                column: "RegistFormId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_RegistForms_RegistFormId",
                table: "Brands",
                column: "RegistFormId",
                principalTable: "RegistForms",
                principalColumn: "IdRegist");

            migrationBuilder.AddForeignKey(
                name: "FK_DropOffs_RegistForms_IdRegist",
                table: "DropOffs",
                column: "IdRegist",
                principalTable: "RegistForms",
                principalColumn: "IdRegist");

            migrationBuilder.AddForeignKey(
                name: "FK_Pickups_RegistForms_IdRegist",
                table: "Pickups",
                column: "IdRegist",
                principalTable: "RegistForms",
                principalColumn: "IdRegist");

            migrationBuilder.AddForeignKey(
                name: "FK_RouteStops_BusRoutes_BusRouteIdRoute",
                table: "RouteStops",
                column: "BusRouteIdRoute",
                principalTable: "BusRoutes",
                principalColumn: "IdRoute");
        }
    }
}
