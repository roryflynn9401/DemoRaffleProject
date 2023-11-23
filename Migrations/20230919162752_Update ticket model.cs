using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SportsRaffles.Migrations
{
    /// <inheritdoc />
    public partial class Updateticketmodel : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Cost",
                table: "Ticket");

            migrationBuilder.AddColumn<float>(
                name: "TicketCost",
                table: "Competitions",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TicketCost",
                table: "Competitions");

            migrationBuilder.AddColumn<float>(
                name: "Cost",
                table: "Ticket",
                type: "float",
                nullable: false,
                defaultValue: 0f);
        }
    }
}
