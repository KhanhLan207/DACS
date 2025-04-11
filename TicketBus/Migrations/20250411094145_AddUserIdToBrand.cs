using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class AddUserIdToBrand : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Brands",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_UserId",
                table: "Brands",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_AspNetUsers_UserId",
                table: "Brands",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_AspNetUsers_UserId",
                table: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Brands_UserId",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Brands");
        }
    }
}
