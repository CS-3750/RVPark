using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace RVPark.Application.Migrations
{
    /// <inheritdoc />
    public partial class FileVersioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "FileSizeBytes",
                table: "Files",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<bool>(
                name: "IsLatestVersion",
                table: "Files",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "ParentFileId",
                table: "Files",
                type: "int",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Version",
                table: "Files",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<string>(
                name: "VersionDescription",
                table: "Files",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Files_ParentFileId",
                table: "Files",
                column: "ParentFileId");

            migrationBuilder.AddForeignKey(
                name: "FK_Files_Files_ParentFileId",
                table: "Files",
                column: "ParentFileId",
                principalTable: "Files",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Files_Files_ParentFileId",
                table: "Files");

            migrationBuilder.DropIndex(
                name: "IX_Files_ParentFileId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "FileSizeBytes",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "IsLatestVersion",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "ParentFileId",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "Version",
                table: "Files");

            migrationBuilder.DropColumn(
                name: "VersionDescription",
                table: "Files");
        }
    }
}
