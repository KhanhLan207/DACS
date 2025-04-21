using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDocuments : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Document",
                table: "Coaches");

            migrationBuilder.AddColumn<string>(
                name: "Documents",
                table: "Coaches",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Documents",
                table: "Coaches");

            migrationBuilder.AddColumn<string>(
                name: "Document",
                table: "Coaches",
                type: "nvarchar(max)",
                nullable: true);
        }
    }
}
