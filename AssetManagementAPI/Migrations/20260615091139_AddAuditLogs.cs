using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace AssetManagementAPI.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogs : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetAllocation",
                table: "AssetAllocation");

            migrationBuilder.RenameTable(
                name: "AssetAllocation",
                newName: "AssetAllocations");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetAllocations",
                table: "AssetAllocations",
                column: "Id");

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Action = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Details = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropPrimaryKey(
                name: "PK_AssetAllocations",
                table: "AssetAllocations");

            migrationBuilder.RenameTable(
                name: "AssetAllocations",
                newName: "AssetAllocation");

            migrationBuilder.AddPrimaryKey(
                name: "PK_AssetAllocation",
                table: "AssetAllocation",
                column: "Id");
        }
    }
}
