using Microsoft.EntityFrameworkCore.Migrations;

namespace MyPages.Migrations
{
    public partial class AddedPublicAccessToPage : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "PublicAccess",
                table: "Pages",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "PublicAccess",
                table: "Pages");
        }
    }
}
