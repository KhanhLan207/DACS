using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateVehicleTypesData : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 1,
                columns: new[] { "NameType", "SeatCount" },
                values: new object[] { "Giường nằm CLC 34 chỗ", 34 });

            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 2,
                columns: new[] { "NameType", "SeatCount" },
                values: new object[] { "Giường nằm CLC 40 chỗ", 40 });

            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 3,
                columns: new[] { "NameType", "SeatCount" },
                values: new object[] { "Giường nằm CLC VIP 20 chỗ", 20 });

            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 4,
                columns: new[] { "NameType", "SeatCount" },
                values: new object[] { "Giường nằm massage 34 chỗ", 34 });

            migrationBuilder.InsertData(
                table: "VehicleTypes",
                columns: new[] { "IdType", "NameType", "SeatCount", "State", "TypeCode" },
                values: new object[,]
                {
                    { 5, "Giường nằm massage 40 chỗ", 40, 0, "VT005" },
                    { 6, "Giường nằm đôi VIP 22 chỗ", 22, 0, "VT006" },
                    { 7, "Ghé Nằm CLC 34 chỗ", 34, 0, "VT007" },
                    { 8, "Ghé Nằm CLC 40 chỗ", 40, 0, "VT008" },
                    { 9, "Ghé Nằm VIP 20 chỗ", 20, 0, "VT009" },
                    { 10, "Ghé Nằm massage 34 chỗ", 34, 0, "VT010" },
                    { 11, "Ghế ngồi CLC 45 chỗ", 45, 0, "VT011" },
                    { 12, "Ghế ngồi CLC 50 chỗ", 50, 0, "VT012" },
                    { 13, "Ghế ngồi VIP 32 chỗ", 32, 0, "VT013" },
                    { 14, "Ghế ngồi Limousine 28 chỗ", 28, 0, "VT014" },
                    { 15, "Limousine DCar VIP 9 chỗ", 9, 0, "VT015" },
                    { 16, "Limousine President 11 chỗ", 11, 0, "VT016" },
                    { 17, "Limousine Fuso Rosa 17 chỗ", 17, 0, "VT017" },
                    { 18, "Limousine Skybus 19 chỗ", 19, 0, "VT018" },
                    { 19, "Limousine Jet VIP 22 chỗ", 22, 0, "VT019" },
                    { 20, "Limousine Auto Kingdom 26 chỗ", 26, 0, "VT020" },
                    { 21, "Xe khách giường nằm 34 chỗ", 34, 0, "VT021" },
                    { 22, "Xe khách giường nằm 40 chỗ", 40, 0, "VT022" },
                    { 23, "Xe khách giường đôi 20 chỗ", 20, 0, "VT023" },
                    { 24, "Xe khách giường đôi 34 chỗ", 34, 0, "VT024" },
                    { 25, "Xe ghế ngồi 12 chỗ Transit", 12, 0, "VT025" },
                    { 26, "Xe ghế ngồi 29 chỗ County", 29, 0, "VT026" },
                    { 27, "Xe ghế ngồi 34 chỗ Thaco Garden", 34, 1, "VT027" },
                    { 28, "Xe ghế ngồi 50 chỗ Giáp Bát Express", 50, 1, "VT028" },
                    { 29, "Xe giường nằm massage 38 chỗ", 38, 0, "VT029" },
                    { 30, "Xe giường nằm CLC 32 chỗ", 32, 0, "VT030" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 5);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 10);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 11);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 12);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 13);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 14);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 15);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 16);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 17);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 18);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 19);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 20);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 21);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 22);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 23);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 24);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 25);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 26);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 27);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 28);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 29);

            migrationBuilder.DeleteData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 30);

            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 1,
                columns: new[] { "NameType", "SeatCount" },
                values: new object[] { "Xe khách 16 chỗ", 16 });

            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 2,
                columns: new[] { "NameType", "SeatCount" },
                values: new object[] { "Xe khách 29 chỗ", 29 });

            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 3,
                columns: new[] { "NameType", "SeatCount" },
                values: new object[] { "Xe khách 45 chỗ", 45 });

            migrationBuilder.UpdateData(
                table: "VehicleTypes",
                keyColumn: "IdType",
                keyValue: 4,
                columns: new[] { "NameType", "SeatCount" },
                values: new object[] { "Xe limousine 9 chỗ", 9 });
        }
    }
}
