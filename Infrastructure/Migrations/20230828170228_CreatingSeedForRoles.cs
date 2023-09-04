using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class CreatingSeedForRoles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            //migrationBuilder.DropForeignKey(
            //    name: "FK_Users_Roles_RoleId1",
            //    table: "Users");

            //migrationBuilder.DropIndex(
            //    name: "IX_Users_RoleId1",
            //    table: "Users");

            //migrationBuilder.DropColumn(
            //    name: "RoleId1",
            //    table: "Users");

            migrationBuilder.InsertData(
                table: "Roles",
                columns: new[] { "Id", "Description" },
                values: new object[,]
                {
                    { 1, "Admin" },
                    { 2, "Default" },
                    { 3, "Real Estate Broker" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 1);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 2);

            migrationBuilder.DeleteData(
                table: "Roles",
                keyColumn: "Id",
                keyValue: 3);

            //migrationBuilder.AddColumn<int>(
            //    name: "RoleId1",
            //    table: "Users",
            //    type: "int",
            //    nullable: true);

            //migrationBuilder.CreateIndex(
            //    name: "IX_Users_RoleId1",
            //    table: "Users",
            //    column: "RoleId1");

            //migrationBuilder.AddForeignKey(
            //    name: "FK_Users_Roles_RoleId1",
            //    table: "Users",
            //    column: "RoleId1",
            //    principalTable: "Roles",
            //    principalColumn: "Id");
        }
    }
}
