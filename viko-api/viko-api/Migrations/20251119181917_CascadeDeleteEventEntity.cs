using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace viko_api.Migrations
{
    /// <inheritdoc />
    public partial class CascadeDeleteEventEntity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registration_Event",
                table: "EventRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Entity",
                table: "Events");

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_Event",
                table: "EventRegistrations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Entity",
                table: "Events",
                column: "Entity_Id",
                principalTable: "Entities",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Registration_Event",
                table: "EventRegistrations");

            migrationBuilder.DropForeignKey(
                name: "FK_Event_Entity",
                table: "Events");

            migrationBuilder.AddForeignKey(
                name: "FK_Registration_Event",
                table: "EventRegistrations",
                column: "EventId",
                principalTable: "Events",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Event_Entity",
                table: "Events",
                column: "Entity_Id",
                principalTable: "Entities",
                principalColumn: "Id");
        }
    }
}
