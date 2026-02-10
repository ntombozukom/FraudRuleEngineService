using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace FraudEngine.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "FraudRuleConfigurations",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsEnabled = table.Column<bool>(type: "bit", nullable: false),
                    Parameters = table.Column<string>(type: "nvarchar(1000)", maxLength: 1000, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudRuleConfigurations", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Transactions",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    TransactionReference = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    AccountNumber = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    Amount = table.Column<decimal>(type: "decimal(18,2)", nullable: false),
                    Currency = table.Column<string>(type: "nvarchar(3)", maxLength: 3, nullable: false),
                    MerchantName = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Category = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    TransactionDate = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Location = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: false),
                    Country = table.Column<string>(type: "nvarchar(5)", maxLength: 5, nullable: false),
                    Channel = table.Column<string>(type: "nvarchar(13)", maxLength: 13, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Transactions", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FraudAlerts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AlertReference = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    TransactionId = table.Column<int>(type: "int", nullable: false),
                    RuleName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Severity = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    ReviewedBy = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    ReviewedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FraudAlerts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FraudAlerts_Transactions_TransactionId",
                        column: x => x.TransactionId,
                        principalTable: "Transactions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FraudRuleConfigurations",
                columns: new[] { "Id", "CreatedAt", "Description", "IsEnabled", "Parameters", "RuleName", "UpdatedAt" },
                values: new object[,]
                {
                    { new Guid("c7b9a4e2-8f31-4d6a-b5c8-2e9f0a1b3d4e"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flags transactions exceeding a configurable amount threshold.", true, "{\"ThresholdAmount\": 50000}", "HighValueTransaction", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("e3d7f2a1-6c48-4b9e-a0d5-8f2c1e7b9a3d"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flags accounts with multiple transactions within a short time window.", true, "{\"MaxTransactions\": 3, \"TimeWindowMinutes\": 5}", "RapidSuccessiveTransactions", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("f8a2c6e4-9b15-4d7f-8e3a-1c0d5b9f2a6e"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flags transactions originating from outside the home country.", true, "{\"HomeCountry\": \"ZA\"}", "CrossBorderTransaction", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("b4e8d1f5-3a72-4c0e-9d6b-7f5a2c8e1b4d"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flags transactions occurring during unusual hours (23:00-05:00).", true, "{\"StartHour\": 23, \"EndHour\": 5}", "AfterHoursTransaction", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) },
                    { new Guid("d9c3b7a6-2e84-4f1d-a5b0-6c9e3d1f8a2b"), new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc), "Flags transactions in spending categories unusual for the account.", true, "{\"MinHistoryCount\": 10, \"UnusualThresholdPercent\": 5}", "UnusualCategory", new DateTime(2025, 1, 1, 0, 0, 0, 0, DateTimeKind.Utc) }
                });

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_AlertReference",
                table: "FraudAlerts",
                column: "AlertReference",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_CreatedAt",
                table: "FraudAlerts",
                column: "CreatedAt");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_Severity",
                table: "FraudAlerts",
                column: "Severity");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_Status",
                table: "FraudAlerts",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_FraudAlerts_TransactionId",
                table: "FraudAlerts",
                column: "TransactionId");

            migrationBuilder.CreateIndex(
                name: "IX_FraudRuleConfigurations_RuleName",
                table: "FraudRuleConfigurations",
                column: "RuleName",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_AccountNumber",
                table: "Transactions",
                column: "AccountNumber");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionDate",
                table: "Transactions",
                column: "TransactionDate");

            migrationBuilder.CreateIndex(
                name: "IX_Transactions_TransactionReference",
                table: "Transactions",
                column: "TransactionReference",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FraudAlerts");

            migrationBuilder.DropTable(
                name: "FraudRuleConfigurations");

            migrationBuilder.DropTable(
                name: "Transactions");
        }
    }
}
