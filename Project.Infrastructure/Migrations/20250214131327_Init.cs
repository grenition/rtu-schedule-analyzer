using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Project.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Inconveniences",
                columns: table => new
                {
                    Id = table.Column<string>(type: "text", nullable: false),
                    SearchKey = table.Column<string>(type: "text", nullable: true),
                    SearchTime = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    FromLessonId = table.Column<string>(type: "text", nullable: true),
                    ToLessonId = table.Column<string>(type: "text", nullable: true),
                    Type = table.Column<string>(type: "text", nullable: true),
                    Date = table.Column<DateOnly>(type: "date", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Inconveniences", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Inconveniences");
        }
    }
}
