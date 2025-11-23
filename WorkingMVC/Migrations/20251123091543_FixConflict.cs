using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WorkingMVC.Migrations
{
    /// <inheritdoc />
    public partial class FixConflict : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Name",
                table: "tblProductImages");

            migrationBuilder.DropColumn(
                name: "Priority",
                table: "tblProductImages");

            migrationBuilder.AddColumn<string>(
                name: "ImageDescription",
                table: "tblProductImages",
                type: "character varying(255)",
                maxLength: 255,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "ImageUrl",
                table: "tblProductImages",
                type: "character varying(500)",
                maxLength: 500,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<bool>(
                name: "IsMain",
                table: "tblProductImages",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ImageDescription",
                table: "tblProductImages");

            migrationBuilder.DropColumn(
                name: "ImageUrl",
                table: "tblProductImages");

            migrationBuilder.DropColumn(
                name: "IsMain",
                table: "tblProductImages");

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "tblProductImages",
                type: "character varying(250)",
                maxLength: 250,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<short>(
                name: "Priority",
                table: "tblProductImages",
                type: "smallint",
                nullable: false,
                defaultValue: (short)0);
        }
    }
}
