using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentIt.Modules.Properties.Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class InitialPropertiesMigration : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.EnsureSchema(
            name: "properties");

        migrationBuilder.CreateTable(
            name: "Properties",
            schema: "properties",
            columns: table => new
            {
                Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                HostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                Name = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Description = table.Column<string>(type: "nvarchar(2000)", maxLength: 2000, nullable: false),
                Address_Street = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                Address_City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Address_Region = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Address_Country = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                Address_PostalCode = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                Type = table.Column<int>(type: "int", nullable: false),
                Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                RentalPeriod = table.Column<int>(type: "int", nullable: false),
                Price_Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                Price_Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                Bedrooms = table.Column<int>(type: "int", nullable: false),
                Bathrooms = table.Column<int>(type: "int", nullable: false),
                RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
            },
            constraints: table =>
            {
                table.PrimaryKey("PK_Properties", x => x.Id);
            });
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropTable(
            name: "Properties",
            schema: "properties");
    }
}
