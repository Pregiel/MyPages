using Microsoft.EntityFrameworkCore.Migrations;

namespace MyPages.Migrations
{
    public partial class AddedOrdinalNumber : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "OrdinalNumber",
                table: "Pages",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OrdinalNumber",
                table: "Pages");
        }
    }
}
