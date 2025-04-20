using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateBrandTwo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegistFormId",
                table: "Brands",
                type: "int",
                nullable: true);

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
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_RegistForms_RegistFormId",
                table: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Brands_RegistFormId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "RegistFormId",
                table: "Brands");
        }
    }
}
