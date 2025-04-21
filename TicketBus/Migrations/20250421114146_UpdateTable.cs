using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Coaches");

            migrationBuilder.AddColumn<string>(
                name: "Images",
                table: "Coaches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Images",
                table: "Coaches");

            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Coaches",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
