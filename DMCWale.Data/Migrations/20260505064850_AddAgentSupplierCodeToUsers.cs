using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DMCWale.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddAgentSupplierCodeToUsers : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AgentSupplierCode",
                table: "AspNetUsers",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_AspNetUsers_AgentSupplierCode",
                table: "AspNetUsers",
                column: "AgentSupplierCode",
                unique: true,
                filter: "[AgentSupplierCode] IS NOT NULL");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_AspNetUsers_AgentSupplierCode",
                table: "AspNetUsers");

            migrationBuilder.DropColumn(
                name: "AgentSupplierCode",
                table: "AspNetUsers");
        }
    }
}
