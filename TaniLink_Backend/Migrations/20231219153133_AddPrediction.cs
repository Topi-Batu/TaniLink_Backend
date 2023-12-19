using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TaniLink_Backend.Migrations
{
    /// <inheritdoc />
    public partial class AddPrediction : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Predictions",
                columns: table => new
                {
                    Id = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Date = table.Column<DateOnly>(type: "date", nullable: false),
                    Price = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    CommodityId = table.Column<string>(type: "nvarchar(450)", nullable: true),
                    CreatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    UpdatedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    DeletedAt = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Predictions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Predictions_Commodities_CommodityId",
                        column: x => x.CommodityId,
                        principalTable: "Commodities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "AreaPrediction",
                columns: table => new
                {
                    AreasId = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    PredictionsId = table.Column<string>(type: "nvarchar(450)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AreaPrediction", x => new { x.AreasId, x.PredictionsId });
                    table.ForeignKey(
                        name: "FK_AreaPrediction_Areas_AreasId",
                        column: x => x.AreasId,
                        principalTable: "Areas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_AreaPrediction_Predictions_PredictionsId",
                        column: x => x.PredictionsId,
                        principalTable: "Predictions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_AreaPrediction_PredictionsId",
                table: "AreaPrediction",
                column: "PredictionsId");

            migrationBuilder.CreateIndex(
                name: "IX_Predictions_CommodityId",
                table: "Predictions",
                column: "CommodityId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AreaPrediction");

            migrationBuilder.DropTable(
                name: "Predictions");
        }
    }
}
