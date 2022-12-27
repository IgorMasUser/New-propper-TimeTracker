using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTracker.Migrations
{
    public partial class PropperDataForAzureEF : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AzureIdentityProvider",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserIdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    AzurAuthenticationKey = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Issuer = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Audience = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "Roles",
                columns: table => new
                {
                    UserRoleId = table.Column<int>(type: "int", nullable: false),
                    RoleName = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    isActive = table.Column<bool>(type: "bit", nullable: false),
                    RolesCreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });

            migrationBuilder.CreateTable(
                name: "User",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", maxLength: 2147483647, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Email = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    IsSystemAdmin = table.Column<bool>(type: "bit", nullable: false),
                    Role = table.Column<int>(type: "int", nullable: false),
                    UserIdentityId = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    StartedWorkDayAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    FinishedWorkDayAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Break = table.Column<int>(type: "int", maxLength: 59, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalWorkedPerDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserWorkedPerRequestedPeriod = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AzureIdentityProvider");

            migrationBuilder.DropTable(
                name: "Roles");

            migrationBuilder.DropTable(
                name: "User");
        }
    }
}
