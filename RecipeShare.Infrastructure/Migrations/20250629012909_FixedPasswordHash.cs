using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RecipeShare.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixedPasswordHash : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2y$10$8SAsAgOuPG294fzcbEFD4eIvkXrlPyg8H6gB6bLuwcZ.AYoNbxvye");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "$2y$10$8SAsAgOuPG294fzcbEFD4eIvkXrlPyg8H6gB6bLuwcZ.AYoNbxvye");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "password123!");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 2,
                column: "PasswordHash",
                value: "password123!");
        }
    }
}
