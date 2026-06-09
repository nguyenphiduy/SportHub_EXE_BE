using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BidaPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiAnalysisHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "StaffUserId",
                table: "work_shifts",
                type: "uuid",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uuid");

            migrationBuilder.CreateTable(
                name: "ai_analysis_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    AnalyzedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Summary = table.Column<string>(type: "text", nullable: false),
                    Trend = table.Column<string>(type: "text", nullable: false),
                    Recommendation = table.Column<string>(type: "text", nullable: false),
                    EstimatedNextPeriodRevenue = table.Column<decimal>(type: "numeric(18,2)", nullable: false),
                    AnalyzedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_analysis_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ai_analysis_history_users_AnalyzedByUserId",
                        column: x => x.AnalyzedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_ai_analysis_history_venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ai_analysis_history_AnalyzedAt",
                table: "ai_analysis_history",
                column: "AnalyzedAt");

            migrationBuilder.CreateIndex(
                name: "IX_ai_analysis_history_AnalyzedByUserId",
                table: "ai_analysis_history",
                column: "AnalyzedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_ai_analysis_history_VenueId",
                table: "ai_analysis_history",
                column: "VenueId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ai_analysis_history");

            migrationBuilder.AlterColumn<Guid>(
                name: "StaffUserId",
                table: "work_shifts",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uuid",
                oldNullable: true);
        }
    }
}
