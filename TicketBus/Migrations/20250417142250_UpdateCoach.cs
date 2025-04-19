using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateCoach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "IdBrand",
                table: "Coaches",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_IdBrand",
                table: "Coaches",
                column: "IdBrand");

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_Brands_IdBrand",
                table: "Coaches",
                column: "IdBrand",
                principalTable: "Brands",
                principalColumn: "IdBrand");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_Brands_IdBrand",
                table: "Coaches");

            migrationBuilder.DropIndex(
                name: "IX_Coaches_IdBrand",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "IdBrand",
                table: "Coaches");
        }
    }
}
