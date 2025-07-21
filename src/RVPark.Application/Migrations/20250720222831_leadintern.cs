using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RVPark.Application.Migrations
{
    /// <inheritdoc />
    public partial class leadintern : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "LeadInternId",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LeadInternId",
                table: "Projects");
        }
    }
}
