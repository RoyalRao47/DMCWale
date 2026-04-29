using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMCWale.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddUserDetailsProfile : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "UserDetails",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    AspNetUserId = table.Column<string>(type: "nvarchar(450)", maxLength: 450, nullable: false),
                    Salutation = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    CountryCode = table.Column<string>(type: "nvarchar(10)", maxLength: 10, nullable: false),
                    City = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Address1 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Address2 = table.Column<string>(type: "nvarchar(250)", maxLength: 250, nullable: false),
                    Signature = table.Column<string>(type: "nvarchar(4000)", maxLength: 4000, nullable: false),
                    ProfileImagePath = table.Column<string>(type: "nvarchar(500)", maxLength: 500, nullable: true),
                    AddDate = table.Column<DateTime>(type: "datetime2", nullable: false, defaultValueSql: "GETUTCDATE()"),
                    ModifyDate = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_UserDetails", x => x.Id);
                    table.ForeignKey(
                        name: "FK_UserDetails_AspNetUsers_AspNetUserId",
                        column: x => x.AspNetUserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_UserDetails_AspNetUserId",
                table: "UserDetails",
                column: "AspNetUserId",
                unique: true);

            migrationBuilder.Sql(
                """
                INSERT INTO AspNetRoleClaims (RoleId, ClaimType, ClaimValue)
                SELECT Id, 'PagePermission', 'Profile.View'
                FROM AspNetRoles roles
                WHERE NOT EXISTS (
                    SELECT 1
                    FROM AspNetRoleClaims claims
                    WHERE claims.RoleId = roles.Id
                        AND claims.ClaimType = 'PagePermission'
                        AND claims.ClaimValue = 'Profile.View'
                )
                """);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(
                """
                DELETE FROM AspNetRoleClaims
                WHERE ClaimType = 'PagePermission'
                    AND ClaimValue = 'Profile.View'
                """);

            migrationBuilder.DropTable(
                name: "UserDetails");
        }
    }
}
