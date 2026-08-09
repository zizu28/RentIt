using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentIt.Modules.Payments.Infrastructure.Database.Migrations
{
    /// <inheritdoc />
    public partial class AddEncryptedProviderTokenToPayment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "EncryptedProviderToken",
                schema: "payments",
                table: "Payments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "EncryptedProviderToken",
                schema: "payments",
                table: "Payments");
        }
    }
}
