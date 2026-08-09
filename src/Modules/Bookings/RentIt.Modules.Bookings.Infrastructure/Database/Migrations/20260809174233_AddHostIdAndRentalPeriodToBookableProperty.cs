using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentIt.Modules.Bookings.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddHostIdAndRentalPeriodToBookableProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HostId",
                schema: "bookings",
                table: "BookableProperties",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "RentalPeriod",
                schema: "bookings",
                table: "BookableProperties",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "HostId",
                schema: "bookings",
                table: "BookableProperties");

            migrationBuilder.DropColumn(
                name: "RentalPeriod",
                schema: "bookings",
                table: "BookableProperties");
        }
    }
}
