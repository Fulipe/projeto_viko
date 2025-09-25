using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace viko_api.Migrations
{
    /// <inheritdoc />
    public partial class InitialMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Entities",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Image = table.Column<string>(type: "varchar(500)", unicode: false, maxLength: 500, nullable: true),
                    Languages = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Entities__3214EC070ADBE488", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "EventStatus",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Status = table.Column<string>(type: "varchar(50)", unicode: false, maxLength: 50, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EventSta__3214EC07E115BBAA", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Users",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Username = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Password = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Email = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Birthdate = table.Column<DateOnly>(type: "date", nullable: false),
                    Entity_Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Users__3214EC0772F42D82", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Users_Entities",
                        column: x => x.Entity_Id,
                        principalTable: "Entities",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Administrators",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<long>(type: "bigint", nullable: false),
                    Entity_Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Administ__3214EC073EC03841", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Admin_Entity",
                        column: x => x.Entity_Id,
                        principalTable: "Entities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Admin_User",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Students",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<long>(type: "bigint", nullable: false),
                    Entity_Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Students__3214EC07E1286ECC", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Student_Entity",
                        column: x => x.Entity_Id,
                        principalTable: "Entities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Student_User",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Teacher",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    User_Id = table.Column<long>(type: "bigint", nullable: false),
                    Entity_Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Teacher__3214EC0794A11128", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Teacher_Entity",
                        column: x => x.Entity_Id,
                        principalTable: "Entities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Teacher_User",
                        column: x => x.User_Id,
                        principalTable: "Users",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "Events",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Teacher_Id = table.Column<long>(type: "bigint", nullable: false),
                    Start_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Finish_Date = table.Column<DateTime>(type: "datetime", nullable: false),
                    Registration_Deadline = table.Column<DateTime>(type: "datetime", nullable: false),
                    Category = table.Column<string>(type: "varchar(100)", unicode: false, maxLength: 100, nullable: false),
                    Location = table.Column<string>(type: "varchar(255)", unicode: false, maxLength: 255, nullable: false),
                    Description = table.Column<string>(type: "varchar(1000)", unicode: false, maxLength: 1000, nullable: true),
                    Event_Status_Id = table.Column<long>(type: "bigint", nullable: false),
                    Entity_Id = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__Events__3214EC0796865580", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Event_Entity",
                        column: x => x.Entity_Id,
                        principalTable: "Entities",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Event_Status",
                        column: x => x.Event_Status_Id,
                        principalTable: "EventStatus",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Event_Teacher",
                        column: x => x.Teacher_Id,
                        principalTable: "Teacher",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateTable(
                name: "EventRegistrations",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Student_Id = table.Column<long>(type: "bigint", nullable: false),
                    Event_Id = table.Column<long>(type: "bigint", nullable: false),
                    RegistrationDate = table.Column<DateTime>(type: "datetime", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK__EventReg__3214EC07A46E7A77", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Registration_Event",
                        column: x => x.Event_Id,
                        principalTable: "Events",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_Registration_Student",
                        column: x => x.Student_Id,
                        principalTable: "Students",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_Entity_Id",
                table: "Administrators",
                column: "Entity_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Administrators_User_Id",
                table: "Administrators",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_Event_Id",
                table: "EventRegistrations",
                column: "Event_Id");

            migrationBuilder.CreateIndex(
                name: "IX_EventRegistrations_Student_Id",
                table: "EventRegistrations",
                column: "Student_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Entity_Id",
                table: "Events",
                column: "Entity_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Event_Status_Id",
                table: "Events",
                column: "Event_Status_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Events_Teacher_Id",
                table: "Events",
                column: "Teacher_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_Entity_Id",
                table: "Students",
                column: "Entity_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Students_User_Id",
                table: "Students",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_Entity_Id",
                table: "Teacher",
                column: "Entity_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Teacher_User_Id",
                table: "Teacher",
                column: "User_Id");

            migrationBuilder.CreateIndex(
                name: "IX_Users_Entity_Id",
                table: "Users",
                column: "Entity_Id");

            migrationBuilder.CreateIndex(
                name: "UQ__Users__536C85E4D82E5D9E",
                table: "Users",
                column: "Username",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "UQ__Users__A9D1053450251125",
                table: "Users",
                column: "Email",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Administrators");

            migrationBuilder.DropTable(
                name: "EventRegistrations");

            migrationBuilder.DropTable(
                name: "Events");

            migrationBuilder.DropTable(
                name: "Students");

            migrationBuilder.DropTable(
                name: "EventStatus");

            migrationBuilder.DropTable(
                name: "Teacher");

            migrationBuilder.DropTable(
                name: "Users");

            migrationBuilder.DropTable(
                name: "Entities");
        }
    }
}
