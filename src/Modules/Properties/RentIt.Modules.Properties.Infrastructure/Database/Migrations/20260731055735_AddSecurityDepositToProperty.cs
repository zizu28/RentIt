using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentIt.Modules.Properties.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddSecurityDepositToProperty : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<decimal>(
                name: "SecurityDeposit_Amount",
                schema: "properties",
                table: "Properties",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<string>(
                name: "SecurityDeposit_Currency",
                schema: "properties",
                table: "Properties",
                type: "nvarchar(3)",
                maxLength: 3,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SecurityDeposit_Amount",
                schema: "properties",
                table: "Properties");

            migrationBuilder.DropColumn(
                name: "SecurityDeposit_Currency",
                schema: "properties",
                table: "Properties");
        }
    }
}
