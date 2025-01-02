using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SchoolManagement.Migrations
{
    /// <inheritdoc />
    public partial class acxzxcxxzckzxc : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "SessionDate",
                table: "Sessions",
                newName: "Date");

            migrationBuilder.RenameColumn(
                name: "Room",
                table: "Sessions",
                newName: "SessionName");

            migrationBuilder.RenameColumn(
                name: "CheckTime",
                table: "Attendances",
                newName: "AttendanceTime");

            migrationBuilder.AddColumn<TimeSpan>(
                name: "EndTime",
                table: "Sessions",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));

            migrationBuilder.AddColumn<TimeSpan>(
                name: "StartTime",
                table: "Sessions",
                type: "time",
                nullable: false,
                defaultValue: new TimeSpan(0, 0, 0, 0, 0));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EndTime",
                table: "Sessions");

            migrationBuilder.DropColumn(
                name: "StartTime",
                table: "Sessions");

            migrationBuilder.RenameColumn(
                name: "SessionName",
                table: "Sessions",
                newName: "Room");

            migrationBuilder.RenameColumn(
                name: "Date",
                table: "Sessions",
                newName: "SessionDate");

            migrationBuilder.RenameColumn(
                name: "AttendanceTime",
                table: "Attendances",
                newName: "CheckTime");
        }
    }
}
