using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTracker.Migrations
{
    public partial class SimplifiedDB : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AzureIdentityProvider");

            migrationBuilder.DropColumn(
                name: "UserIdentityId",
                table: "User");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "UserIdentityId",
                table: "User",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "AzureIdentityProvider",
                columns: table => new
                {
                    Audience = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    AzurAuthenticationKey = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Issuer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserIdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                });
        }
    }
}
