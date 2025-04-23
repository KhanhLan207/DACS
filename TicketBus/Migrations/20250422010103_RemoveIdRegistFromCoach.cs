using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIdRegistFromCoach : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_RegistForms_IdRegist",
                table: "Coaches");

            migrationBuilder.RenameColumn(
                name: "IdRegist",
                table: "Coaches",
                newName: "RegistFormIdRegist");

            migrationBuilder.RenameIndex(
                name: "IX_Coaches_IdRegist",
                table: "Coaches",
                newName: "IX_Coaches_RegistFormIdRegist");

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_RegistForms_RegistFormIdRegist",
                table: "Coaches",
                column: "RegistFormIdRegist",
                principalTable: "RegistForms",
                principalColumn: "IdRegist");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Coaches_RegistForms_RegistFormIdRegist",
                table: "Coaches");

            migrationBuilder.RenameColumn(
                name: "RegistFormIdRegist",
                table: "Coaches",
                newName: "IdRegist");

            migrationBuilder.RenameIndex(
                name: "IX_Coaches_RegistFormIdRegist",
                table: "Coaches",
                newName: "IX_Coaches_IdRegist");

            migrationBuilder.AddForeignKey(
                name: "FK_Coaches_RegistForms_IdRegist",
                table: "Coaches",
                column: "IdRegist",
                principalTable: "RegistForms",
                principalColumn: "IdRegist",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
