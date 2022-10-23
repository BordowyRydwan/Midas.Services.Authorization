using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    public partial class ExtractUserDataToUserService : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "UserFamilyRoles");

            migrationBuilder.DropTable(
                name: "Families");

            migrationBuilder.DropTable(
                name: "FamilyRoles");

            migrationBuilder.DropColumn(
                name: "BirthDate",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "FirstName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "LastName",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "RegisterDate",
                table: "Users");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "BirthDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "FirstName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "LastName",
                table: "Users",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisterDate",
                table: "Users",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "Families",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Families", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "FamilyRoles",
                columns: table => new
                {
                    Id = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FamilyRoles", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "UserFamilyRoles",
                columns: table => new
                {
                    UserId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    FamilyId = table.Column<decimal>(type: "decimal(20,0)", nullable: false),
                    FamilyRoleId = table.Column<decimal>(type: "decimal(20,0)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserFamilyRoles", x => new { x.UserId, x.FamilyId });
                    table.ForeignKey(
                        name: "FK_UserFamilyRoles_Families_FamilyId",
                        column: x => x.FamilyId,
                        principalTable: "Families",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFamilyRoles_FamilyRoles_FamilyRoleId",
                        column: x => x.FamilyRoleId,
                        principalTable: "FamilyRoles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_UserFamilyRoles_Users_UserId",
                        column: x => x.UserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "FamilyRoles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 1m, "Main administrator" });

            migrationBuilder.InsertData(
                table: "FamilyRoles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 2m, "Parent" });

            migrationBuilder.InsertData(
                table: "FamilyRoles",
                columns: new[] { "Id", "Name" },
                values: new object[] { 3m, "Child" });

            migrationBuilder.CreateIndex(
                name: "IX_UserFamilyRoles_FamilyId",
                table: "UserFamilyRoles",
                column: "FamilyId");

            migrationBuilder.CreateIndex(
                name: "IX_UserFamilyRoles_FamilyRoleId",
                table: "UserFamilyRoles",
                column: "FamilyRoleId");
        }
    }
}
