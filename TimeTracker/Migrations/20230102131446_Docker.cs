using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace TimeTracker.Migrations
{
    public partial class Docker : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "RefreshTokenProvider",
                columns: table => new
                {
                    UserId = table.Column<int>(type: "int", nullable: false),
                    UserRefreshTokenPair = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    RefreshToken = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    RefreshTokenCreated = table.Column<DateTime>(type: "datetime2", nullable: false),
                    RefreshTokenExpires = table.Column<DateTime>(type: "datetime2", nullable: false)
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
                name: "User2",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    UserId = table.Column<int>(type: "int", maxLength: 2147483647, nullable: false),
                    Name = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Surname = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsSystemAdmin = table.Column<bool>(type: "bit", nullable: false),
                    UserAccessTokenPair = table.Column<Guid>(type: "uniqueidentifier", nullable: true),
                    Role = table.Column<int>(type: "int", nullable: false),
                    StartedWorkDayAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    FinishedWorkDayAt = table.Column<DateTime>(type: "datetime", nullable: false),
                    Break = table.Column<int>(type: "int", maxLength: 59, nullable: false),
                    Date = table.Column<DateTime>(type: "datetime2", nullable: false),
                    TotalWorkedPerDay = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UserWorkedPerRequestedPeriod = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_User", x => x.Id);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {

        }
    }
}
