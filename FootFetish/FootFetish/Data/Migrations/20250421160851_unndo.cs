using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FootFetish.Data.Migrations
{
    /// <inheritdoc />
    public partial class unndo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CategoryIds",
                table: "Products",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]",
                oldClrType: typeof(string),
                oldType: "nvarchar(max)",
                oldNullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "CategoryIds",
                table: "Products",
                type: "nvarchar(max)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");
        }
    }
}
