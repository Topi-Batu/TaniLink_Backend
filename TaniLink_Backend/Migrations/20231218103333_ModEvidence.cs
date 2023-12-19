using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaniLink_Backend.Migrations
{
    /// <inheritdoc />
    public partial class ModEvidence : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "XenditInvoiceId",
                table: "Invoices");

            migrationBuilder.AddColumn<string>(
                name: "PaymentEvidence",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PaymentEvidence",
                table: "Invoices");

            migrationBuilder.AddColumn<string>(
                name: "XenditInvoiceId",
                table: "Invoices",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
