using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDB : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_RegistForms_RegistFormIdRegist",
                table: "Coaches");

            migrationBuilder.DropIndex(
                name: "IX_Coaches_RegistFormIdRegist",
                table: "Coaches");

            migrationBuilder.DropColumn(
                name: "RegistFormIdRegist",
                table: "Coaches");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "RegistFormIdRegist",
                table: "Coaches",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Coaches_RegistFormIdRegist",
                table: "Coaches",
                column: "RegistFormIdRegist");

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_RegistForms_RegistFormIdRegist",
                table: "Coaches",
                column: "RegistFormIdRegist",
                principalTable: "RegistForms",
                principalColumn: "IdRegist");
        }
    }
}
