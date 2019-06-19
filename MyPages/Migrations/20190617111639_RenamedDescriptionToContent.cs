using Microsoft.EntityFrameworkCore.Migrations;

namespace MyPages.Migrations
{
    public partial class RenamedDescriptionToContent : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Folders",
                newName: "Content");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "Content",
                table: "Folders",
                newName: "Description");
        }
    }
}
