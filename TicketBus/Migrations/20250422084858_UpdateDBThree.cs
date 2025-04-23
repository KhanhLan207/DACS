using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDBThree : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_AspNetUsers_UserId",
                table: "Brands");

            migrationBuilder.DropForeignKey(
                name: "FK_Brands_RegistForms_RegistFormId",
                table: "Brands");

            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_Brands_IdBrand",
                table: "Coaches");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_AspNetUsers_UserId",
                table: "Brands",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_RegistForms_RegistFormId",
                table: "Brands",
                column: "RegistFormId",
                principalTable: "RegistForms",
                principalColumn: "IdRegist",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_Brands_IdBrand",
                table: "Coaches",
                column: "IdBrand",
                principalTable: "Brands",
                principalColumn: "IdBrand",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_AspNetUsers_UserId",
                table: "Brands");

            migrationBuilder.DropForeignKey(
                name: "FK_Brands_RegistForms_RegistFormId",
                table: "Brands");

            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_Brands_IdBrand",
                table: "Coaches");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_AspNetUsers_UserId",
                table: "Brands",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_RegistForms_RegistFormId",
                table: "Brands",
                column: "RegistFormId",
                principalTable: "RegistForms",
                principalColumn: "IdRegist");

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_Brands_IdBrand",
                table: "Coaches",
                column: "IdBrand",
                principalTable: "Brands",
                principalColumn: "IdBrand");
        }
    }
}
