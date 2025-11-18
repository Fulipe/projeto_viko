using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace viko_api.Migrations
{
    /// <inheritdoc />
    public partial class AddFieldIsViewed : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isViewed",
                table: "Events",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isViewed",
                table: "Events");
        }
    }
}
