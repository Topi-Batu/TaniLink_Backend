using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaniLink_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddCommodityImage : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Image",
                table: "Commodities",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Image",
                table: "Commodities");
        }
    }
}
