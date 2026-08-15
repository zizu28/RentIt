using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentIt.Modules.Payments.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddPaymentMethods : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "BookingId",
                schema: "payments",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.AddColumn<string>(
                name: "Method_EncryptedProviderToken",
                schema: "payments",
                table: "Payments",
                type: "nvarchar(2000)",
                maxLength: 2000,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Method_ExpiryMonth",
                schema: "payments",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Method_ExpiryYear",
                schema: "payments",
                table: "Payments",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Method_Last4",
                schema: "payments",
                table: "Payments",
                type: "nvarchar(4)",
                maxLength: 4,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Method_MethodType",
                schema: "payments",
                table: "Payments",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Method_Provider",
                schema: "payments",
                table: "Payments",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                schema: "payments",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Payments_UserId",
                schema: "payments",
                table: "Payments",
                column: "UserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Payments_UserId",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Method_EncryptedProviderToken",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Method_ExpiryMonth",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Method_ExpiryYear",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Method_Last4",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Method_MethodType",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "Method_Provider",
                schema: "payments",
                table: "Payments");

            migrationBuilder.DropColumn(
                name: "UserId",
                schema: "payments",
                table: "Payments");

            migrationBuilder.AlterColumn<Guid>(
                name: "BookingId",
                schema: "payments",
                table: "Payments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);
        }
    }
}
