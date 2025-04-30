using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TicketBus.Migrations
{
    /// <inheritdoc />
    public partial class UpdateScheduleDetailsTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleDetails",
                table: "ScheduleDetails");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DepartTime",
                table: "ScheduleDetails",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ArriveTime",
                table: "ScheduleDetails",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0),
                oldClrType: typeof(TimeSpan),
                oldType: "time",
                oldNullable: true);

            migrationBuilder.AlterColumn<int>(
                name: "IdRoute",
                table: "ScheduleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "IdCoach",
                table: "ScheduleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .OldAnnotation("Relational:ColumnOrder", 0);

            migrationBuilder.AddColumn<int>(
                name: "IdSchedule",
                table: "ScheduleDetails",
                type: "int",
                nullable: false,
                defaultValue: 0)
                .Annotation("SqlServer:Identity", "1, 1");

            migrationBuilder.AddColumn<DateTime>(
                name: "Date",
                table: "ScheduleDetails",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "State",
                table: "ScheduleDetails",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleDetails",
                table: "ScheduleDetails",
                column: "IdSchedule");

            migrationBuilder.CreateIndex(
                name: "IX_ScheduleDetails_IdCoach",
                table: "ScheduleDetails",
                column: "IdCoach");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_ScheduleDetails",
                table: "ScheduleDetails");

            migrationBuilder.DropIndex(
                name: "IX_ScheduleDetails_IdCoach",
                table: "ScheduleDetails");

            migrationBuilder.DropColumn(
                name: "IdSchedule",
                table: "ScheduleDetails");

            migrationBuilder.DropColumn(
                name: "Date",
                table: "ScheduleDetails");

            migrationBuilder.DropColumn(
                name: "State",
                table: "ScheduleDetails");

            migrationBuilder.AlterColumn<int>(
                name: "IdRoute",
                table: "ScheduleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 1);

            migrationBuilder.AlterColumn<int>(
                name: "IdCoach",
                table: "ScheduleDetails",
                type: "int",
                nullable: false,
                oldClrType: typeof(int),
                oldType: "int")
                .Annotation("Relational:ColumnOrder", 0);

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "DepartTime",
                table: "ScheduleDetails",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AlterColumn<TimeSpan>(
                name: "ArriveTime",
                table: "ScheduleDetails",
                type: "time",
                nullable: true,
                oldClrType: typeof(TimeSpan),
                oldType: "time");

            migrationBuilder.AddPrimaryKey(
                name: "PK_ScheduleDetails",
                table: "ScheduleDetails",
                columns: new[] { "IdCoach", "IdRoute" });
        }
    }
}
