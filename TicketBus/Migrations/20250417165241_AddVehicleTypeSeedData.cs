using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class AddVehicleTypeSeedData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "VehicleTypes",
                columns: new[] { "IdType", "NameType", "SeatCount", "State", "TypeCode" },
                values: new object[,]
                {
                    { 1, "Xe khách 16 chỗ", 16, 0, "VT001" },
                    { 2, "Xe khách 29 chỗ", 29, 0, "VT002" },
                    { 3, "Xe khách 45 chỗ", 45, 0, "VT003" },
                    { 4, "Xe limousine 9 chỗ", 9, 0, "VT004" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 4);
        }
    }
}
