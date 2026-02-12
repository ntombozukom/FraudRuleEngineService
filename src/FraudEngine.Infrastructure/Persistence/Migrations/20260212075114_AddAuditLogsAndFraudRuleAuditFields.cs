using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FraudEngine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddAuditLogsAndFraudRuleAuditFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "CreatedBy",
                table: "FraudRuleConfigurations",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastModifiedBy",
                table: "FraudRuleConfigurations",
                type: "nvarchar(256)",
                maxLength: 256,
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityType = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityId = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Action = table.Column<int>(type: "int", nullable: false),
                    PropertyName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: true),
                    OldValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: true),
                    ModifiedBy = table.Column<string>(type: "nvarchar(256)", maxLength: 256, nullable: false),
                    ModifiedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.Id);
                });

            migrationBuilder.UpdateData(
                table: "FraudRuleConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("b4e8d1f5-3a72-4c0e-9d6b-7f5a2c8e1b4d"),
                columns: new[] { "CreatedBy", "LastModifiedBy" },
                values: new object[] { "System", null });

            migrationBuilder.UpdateData(
                table: "FraudRuleConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("c7b9a4e2-8f31-4d6a-b5c8-2e9f0a1b3d4e"),
                columns: new[] { "CreatedBy", "LastModifiedBy" },
                values: new object[] { "System", null });

            migrationBuilder.UpdateData(
                table: "FraudRuleConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("d9c3b7a6-2e84-4f1d-a5b0-6c9e3d1f8a2b"),
                columns: new[] { "CreatedBy", "LastModifiedBy" },
                values: new object[] { "System", null });

            migrationBuilder.UpdateData(
                table: "FraudRuleConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("e3d7f2a1-6c48-4b9e-a0d5-8f2c1e7b9a3d"),
                columns: new[] { "CreatedBy", "LastModifiedBy" },
                values: new object[] { "System", null });

            migrationBuilder.UpdateData(
                table: "FraudRuleConfigurations",
                keyColumn: "Id",
                keyValue: new Guid("f8a2c6e4-9b15-4d7f-8e3a-1c0d5b9f2a6e"),
                columns: new[] { "CreatedBy", "LastModifiedBy" },
                values: new object[] { "System", null });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityId",
                table: "AuditLogs",
                column: "EntityId");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType",
                table: "AuditLogs",
                column: "EntityType");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_EntityType_EntityId",
                table: "AuditLogs",
                columns: new[] { "EntityType", "EntityId" });

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ModifiedAt",
                table: "AuditLogs",
                column: "ModifiedAt");

            migrationBuilder.CreateIndex(
                name: "IX_AuditLogs_ModifiedBy",
                table: "AuditLogs",
                column: "ModifiedBy");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropColumn(
                name: "CreatedBy",
                table: "FraudRuleConfigurations");

            migrationBuilder.DropColumn(
                name: "LastModifiedBy",
                table: "FraudRuleConfigurations");
        }
    }
}
