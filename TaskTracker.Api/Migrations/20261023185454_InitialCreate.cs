using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace TaskTracker.Api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Projects",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", maxLength: 100, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Projects", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Tasks",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    ProjectId = table.Column<int>(type: "INTEGER", nullable: false),
                    Title = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Description = table.Column<string>(type: "TEXT", maxLength: 500, nullable: true),
                    Status = table.Column<string>(type: "TEXT", maxLength: 20, nullable: false),
                    DueDate = table.Column<DateTime>(type: "TEXT", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Tasks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Tasks_Projects_ProjectId",
                        column: x => x.ProjectId,
                        principalTable: "Projects",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Projects",
                columns: new[] { "Id", "CreatedAt", "Description", "Name" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 1, 9, 0, 0, 0, DateTimeKind.Utc), "Refresh the company website with a modern look.", "Website Redesign" },
                    { 2, new DateTime(2026, 10, 5, 9, 0, 0, 0, DateTimeKind.Utc), "Prepare and release version 1.0 of the mobile app.", "Mobile App Launch" }
                });

            migrationBuilder.InsertData(
                table: "Tasks",
                columns: new[] { "Id", "CreatedAt", "Description", "DueDate", "ProjectId", "Status", "Title" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 10, 1, 10, 0, 0, 0, DateTimeKind.Utc), "Sketch the home, about and contact pages.", new DateTime(2026, 10, 10, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Done", "Create wireframes" },
                    { 2, new DateTime(2026, 10, 2, 10, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 10, 28, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "InProgress", "Design homepage mockup" },
                    { 3, new DateTime(2026, 10, 3, 10, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 11, 5, 0, 0, 0, 0, DateTimeKind.Unspecified), 1, "Todo", "Build contact form" },
                    { 4, new DateTime(2026, 10, 5, 11, 0, 0, 0, DateTimeKind.Utc), null, new DateTime(2026, 11, 15, 0, 0, 0, 0, DateTimeKind.Unspecified), 2, "InProgress", "Write app store description" },
                    { 5, new DateTime(2026, 10, 6, 11, 0, 0, 0, DateTimeKind.Utc), "Send a reminder when a task is due.", null, 2, "Todo", "Set up push notifications" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Tasks_ProjectId",
                table: "Tasks",
                column: "ProjectId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Tasks");

            migrationBuilder.DropTable(
                name: "Projects");
        }
    }
}
