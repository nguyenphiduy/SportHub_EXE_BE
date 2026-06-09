using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BidaPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddAiProviderSettings : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ai_provider_settings",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    ProviderName = table.Column<string>(type: "character varying(100)", maxLength: 100, nullable: false),
                    EncryptedApiKey = table.Column<string>(type: "text", nullable: false),
                    Model = table.Column<string>(type: "character varying(200)", maxLength: 200, nullable: false, defaultValue: "openrouter/free"),
                    BaseUrl = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: false, defaultValue: "https://openrouter.ai/api/v1"),
                    IsEnabled = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    UpdatedByUserId = table.Column<Guid>(type: "uuid", nullable: true),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ai_provider_settings", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ai_provider_settings_users_UpdatedByUserId",
                        column: x => x.UpdatedByUserId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ai_provider_settings_ProviderName",
                table: "ai_provider_settings",
                column: "ProviderName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ai_provider_settings_UpdatedByUserId",
                table: "ai_provider_settings",
                column: "UpdatedByUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(name: "ai_provider_settings");
        }
    }
}
