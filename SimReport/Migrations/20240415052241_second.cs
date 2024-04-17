using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SimReport.Migrations
{
    /// <inheritdoc />
    public partial class second : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Companies_CompanyId",
                table: "Cards");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Companies_CompanyId",
                table: "Cards",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Cards_Companies_CompanyId",
                table: "Cards");

            migrationBuilder.AddForeignKey(
                name: "FK_Cards_Companies_CompanyId",
                table: "Cards",
                column: "CompanyId",
                principalTable: "Companies",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
