using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RentIt.Modules.Analytics.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddHostIdToPropertyMetrics : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "HostId",
                schema: "analytics",
                table: "PropertyMetrics",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<int>(
                name: "TotalCancellations",
                schema: "analytics",
                table: "PropertyMetrics",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<decimal>(
                name: "TotalRevenue",
                schema: "analytics",
                table: "PropertyMetrics",
                type: "decimal(18,2)",
                precision: 18,
                scale: 2,
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.CreateTable(
                name: "HostMetrics",
                schema: "analytics",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    HostId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TotalProperties = table.Column<int>(type: "int", nullable: false),
                    TotalBookings = table.Column<int>(type: "int", nullable: false),
                    TotalRevenue = table.Column<decimal>(type: "decimal(18,2)", precision: 18, scale: 2, nullable: false),
                    TotalReviews = table.Column<int>(type: "int", nullable: false),
                    AverageRating = table.Column<double>(type: "float", nullable: false),
                    RowVersion = table.Column<byte[]>(type: "rowversion", rowVersion: true, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HostMetrics", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PropertyMetrics_HostId",
                schema: "analytics",
                table: "PropertyMetrics",
                column: "HostId");

            migrationBuilder.CreateIndex(
                name: "IX_PropertyMetrics_PropertyId",
                schema: "analytics",
                table: "PropertyMetrics",
                column: "PropertyId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_HostMetrics_HostId",
                schema: "analytics",
                table: "HostMetrics",
                column: "HostId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "HostMetrics",
                schema: "analytics");

            migrationBuilder.DropIndex(
                name: "IX_PropertyMetrics_HostId",
                schema: "analytics",
                table: "PropertyMetrics");

            migrationBuilder.DropIndex(
                name: "IX_PropertyMetrics_PropertyId",
                schema: "analytics",
                table: "PropertyMetrics");

            migrationBuilder.DropColumn(
                name: "HostId",
                schema: "analytics",
                table: "PropertyMetrics");

            migrationBuilder.DropColumn(
                name: "TotalCancellations",
                schema: "analytics",
                table: "PropertyMetrics");

            migrationBuilder.DropColumn(
                name: "TotalRevenue",
                schema: "analytics",
                table: "PropertyMetrics");
        }
    }
}
