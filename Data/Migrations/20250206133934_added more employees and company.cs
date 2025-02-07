using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class addedmoreemployeesandcompany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "contact@companyA.com");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Email",
                value: "contact@companyB.com");

            migrationBuilder.InsertData(
                table: "Customers",
                columns: new[] { "Id", "Email", "Name", "PhoneNumber" },
                values: new object[] { 3, "contact@companyC.com", "Company c", "987988976" });

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastName",
                value: "Andersson");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastName",
                value: "Johansson");

            migrationBuilder.InsertData(
                table: "Employees",
                columns: new[] { "Id", "Email", "FirstName", "LastName", "RoleId" },
                values: new object[] { 3, "Maria@company.com", "Maria", "Karlsson", 1 });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.DeleteData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 3);

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 1,
                column: "Email",
                value: "contact@companya.com");

            migrationBuilder.UpdateData(
                table: "Customers",
                keyColumn: "Id",
                keyValue: 2,
                column: "Email",
                value: "contact@companyb.com");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 1,
                column: "LastName",
                value: "Svensson");

            migrationBuilder.UpdateData(
                table: "Employees",
                keyColumn: "Id",
                keyValue: 2,
                column: "LastName",
                value: "Andersson");
        }
    }
}
