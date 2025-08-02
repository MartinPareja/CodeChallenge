using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CarRental.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class RemovedEnumsAddedIsCancelled : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TotalCost",
                table: "Rentals");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Cars");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "Rentals",
                newName: "IsCancelled");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsCancelled",
                table: "Rentals",
                newName: "Status");

            migrationBuilder.AddColumn<decimal>(
                name: "TotalCost",
                table: "Rentals",
                type: "decimal(18,2)",
                nullable: false,
                defaultValue: 0m);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Cars",
                type: "INTEGER",
                nullable: false,
                defaultValue: 0);
        }
    }
}
