using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentIt.Modules.Properties.Infrastructure.Database.Migrations;

/// <inheritdoc />
public partial class AddedPropertiesCollections : Migration
{
    /// <inheritdoc />
    protected override void Up(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.AddColumn<string>(
            name: "Amenities",
            schema: "properties",
            table: "Properties",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");

        migrationBuilder.AddColumn<string>(
            name: "Images",
            schema: "properties",
            table: "Properties",
            type: "nvarchar(max)",
            nullable: false,
            defaultValue: "");
    }

    /// <inheritdoc />
    protected override void Down(MigrationBuilder migrationBuilder)
    {
        migrationBuilder.DropColumn(
            name: "Amenities",
            schema: "properties",
            table: "Properties");

        migrationBuilder.DropColumn(
            name: "Images",
            schema: "properties",
            table: "Properties");
    }
}
