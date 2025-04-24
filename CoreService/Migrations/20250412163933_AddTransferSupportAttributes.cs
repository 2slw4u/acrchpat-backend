using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace CoreService.Migrations
{
    /// <inheritdoc />
    public partial class AddTransferSupportAttributes : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<double>(
                name: "DestinationAmount",
                table: "Transactions",
                type: "double precision",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "DestinationCurrency",
                table: "Transactions",
                type: "integer",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "DestinationAmount",
                table: "Transactions");

            migrationBuilder.DropColumn(
                name: "DestinationCurrency",
                table: "Transactions");
        }
    }
}
