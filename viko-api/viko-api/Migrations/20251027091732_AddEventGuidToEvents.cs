using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace viko_api.Migrations
{
    /// <inheritdoc />
    public partial class AddEventGuidToEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<Guid>(
                name: "EventGuid",
                table: "Events",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_Events_EventGuid",
                table: "Events",
                column: "EventGuid",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Events_EventGuid",
                table: "Events");

            migrationBuilder.DropColumn(
                name: "EventGuid",
                table: "Events");
        }
    }
}
