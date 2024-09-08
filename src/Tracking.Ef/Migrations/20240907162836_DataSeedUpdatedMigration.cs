using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Tracking.Ef.Migrations
{
    /// <inheritdoc />
    public partial class DataSeedUpdatedMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Name",
                value: "Inactive account");

            migrationBuilder.InsertData(
                table: "Accounts",
                columns: new[] { "Id", "IsActive", "Name" },
                values: new object[] { 3L, true, "Another active account" });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 3L);

            migrationBuilder.UpdateData(
                table: "Accounts",
                keyColumn: "Id",
                keyValue: 2L,
                column: "Name",
                value: "Active account");
        }
    }
}
