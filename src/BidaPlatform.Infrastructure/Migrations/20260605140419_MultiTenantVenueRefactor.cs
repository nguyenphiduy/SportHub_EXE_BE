using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BidaPlatform.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class MultiTenantVenueRefactor : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "VenueId",
                table: "users",
                type: "uuid",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "VenueId",
                table: "iot_devices",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VenueId",
                table: "billiard_tables",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "VenueId",
                table: "billiard_sessions",
                type: "uuid",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "venues",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: false),
                    Address = table.Column<string>(type: "character varying(500)", maxLength: 500, nullable: true),
                    Phone = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: true),
                    OwnerName = table.Column<string>(type: "character varying(255)", maxLength: 255, nullable: true),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false, defaultValue: true),
                    PrimaryManagerId = table.Column<Guid>(type: "uuid", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venues", x => x.Id);
                    table.ForeignKey(
                        name: "FK_venues_users_PrimaryManagerId",
                        column: x => x.PrimaryManagerId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "venue_subscriptions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    VenueId = table.Column<Guid>(type: "uuid", nullable: false),
                    Plan = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "character varying(50)", maxLength: 50, nullable: false),
                    StartDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    EndDate = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    AutoRenew = table.Column<bool>(type: "boolean", nullable: false),
                    ApprovedBySuperAdminId = table.Column<Guid>(type: "uuid", nullable: true),
                    ApprovedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_venue_subscriptions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_venue_subscriptions_users_ApprovedBySuperAdminId",
                        column: x => x.ApprovedBySuperAdminId,
                        principalTable: "users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                    table.ForeignKey(
                        name: "FK_venue_subscriptions_venues_VenueId",
                        column: x => x.VenueId,
                        principalTable: "venues",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_users_VenueId",
                table: "users",
                column: "VenueId");

            migrationBuilder.AddCheckConstraint(
                name: "CK_users_role_venue_scope",
                table: "users",
                sql: "(\"Role\" = 'SuperAdmin' AND \"VenueId\" IS NULL) OR (\"Role\" IN ('Manager','Staff') AND \"VenueId\" IS NOT NULL)");

            migrationBuilder.CreateIndex(
                name: "IX_iot_devices_VenueId",
                table: "iot_devices",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_billiard_tables_VenueId_Name",
                table: "billiard_tables",
                columns: new[] { "VenueId", "Name" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_billiard_sessions_VenueId",
                table: "billiard_sessions",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_venue_subscriptions_ApprovedBySuperAdminId",
                table: "venue_subscriptions",
                column: "ApprovedBySuperAdminId");

            migrationBuilder.CreateIndex(
                name: "IX_venue_subscriptions_VenueId",
                table: "venue_subscriptions",
                column: "VenueId");

            migrationBuilder.CreateIndex(
                name: "IX_venues_PrimaryManagerId",
                table: "venues",
                column: "PrimaryManagerId");

            migrationBuilder.AddForeignKey(
                name: "FK_billiard_sessions_venues_VenueId",
                table: "billiard_sessions",
                column: "VenueId",
                principalTable: "venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_billiard_tables_venues_VenueId",
                table: "billiard_tables",
                column: "VenueId",
                principalTable: "venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_iot_devices_venues_VenueId",
                table: "iot_devices",
                column: "VenueId",
                principalTable: "venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_users_venues_VenueId",
                table: "users",
                column: "VenueId",
                principalTable: "venues",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_billiard_sessions_venues_VenueId",
                table: "billiard_sessions");

            migrationBuilder.DropForeignKey(
                name: "FK_billiard_tables_venues_VenueId",
                table: "billiard_tables");

            migrationBuilder.DropForeignKey(
                name: "FK_iot_devices_venues_VenueId",
                table: "iot_devices");

            migrationBuilder.DropForeignKey(
                name: "FK_users_venues_VenueId",
                table: "users");

            migrationBuilder.DropTable(
                name: "venue_subscriptions");

            migrationBuilder.DropTable(
                name: "venues");

            migrationBuilder.DropIndex(
                name: "IX_users_VenueId",
                table: "users");

            migrationBuilder.DropCheckConstraint(
                name: "CK_users_role_venue_scope",
                table: "users");

            migrationBuilder.DropIndex(
                name: "IX_iot_devices_VenueId",
                table: "iot_devices");

            migrationBuilder.DropIndex(
                name: "IX_billiard_tables_VenueId_Name",
                table: "billiard_tables");

            migrationBuilder.DropIndex(
                name: "IX_billiard_sessions_VenueId",
                table: "billiard_sessions");

            migrationBuilder.DropColumn(
                name: "VenueId",
                table: "users");

            migrationBuilder.DropColumn(
                name: "VenueId",
                table: "iot_devices");

            migrationBuilder.DropColumn(
                name: "VenueId",
                table: "billiard_tables");

            migrationBuilder.DropColumn(
                name: "VenueId",
                table: "billiard_sessions");
        }
    }
}
