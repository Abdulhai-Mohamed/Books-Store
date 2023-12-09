using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace BooksStore.Migrations
{
    /// <inheritdoc />
    public partial class addUserisPersistentColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isPersistent",
                table: "AspNetUsers",
                type: "bit",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isPersistent",
                table: "AspNetUsers");
        }
    }
}
