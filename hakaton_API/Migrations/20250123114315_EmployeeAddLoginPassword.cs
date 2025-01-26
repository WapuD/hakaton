using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace hakaton_API.Migrations
{
    /// <inheritdoc />
    public partial class EmployeeAddLoginPassword : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Login",
                table: "Employee",
                type: "text",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Password",
                table: "Employee",
                type: "text",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Login",
                table: "Employee");

            migrationBuilder.DropColumn(
                name: "Password",
                table: "Employee");
        }
    }
}
