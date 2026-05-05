using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMCWale.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentSupplierCodeToUserProfiles : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgentSupplierCode",
                table: "Users",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.Sql("""
                UPDATE U
                SET U.AgentSupplierCode = COALESCE(AU.AgentSupplierCode, 'DMC-' + RIGHT('0000' + CAST(1000 + (U.Id % 9000) AS varchar(10)), 4))
                FROM Users U
                INNER JOIN AspNetUsers AU ON AU.Id = U.AspNetUserId
                WHERE ISNULL(U.AgentSupplierCode, '') = ''
                """);

            migrationBuilder.Sql("""
                UPDATE AU
                SET AU.AgentSupplierCode = U.AgentSupplierCode
                FROM AspNetUsers AU
                INNER JOIN Users U ON U.AspNetUserId = AU.Id
                WHERE AU.AgentSupplierCode IS NULL
                """);

            migrationBuilder.CreateIndex(
                name: "IX_Users_AgentSupplierCode",
                table: "Users",
                column: "AgentSupplierCode",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Users_AgentSupplierCode",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "AgentSupplierCode",
                table: "Users");
        }
    }
}
