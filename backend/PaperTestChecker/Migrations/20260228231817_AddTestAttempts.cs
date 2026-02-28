using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaperTestChecker.Migrations
{
    /// <inheritdoc />
    public partial class AddTestAttempts : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "TestAttempts",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneratedTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    StudentUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    Score = table.Column<int>(type: "integer", nullable: false),
                    MaxScore = table.Column<int>(type: "integer", nullable: false),
                    CompletedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAttempts", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttempts_GeneratedTests_GeneratedTestId",
                        column: x => x.GeneratedTestId,
                        principalTable: "GeneratedTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_TestAttempts_Users_StudentUserId",
                        column: x => x.StudentUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "TestAttemptAnswers",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TestAttemptId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionNumber = table.Column<int>(type: "integer", nullable: false),
                    SelectedAnswer = table.Column<string>(type: "text", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "text", nullable: false),
                    IsCorrect = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TestAttemptAnswers", x => x.Id);
                    table.ForeignKey(
                        name: "FK_TestAttemptAnswers_TestAttempts_TestAttemptId",
                        column: x => x.TestAttemptId,
                        principalTable: "TestAttempts",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_TestAttemptAnswers_TestAttemptId",
                table: "TestAttemptAnswers",
                column: "TestAttemptId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_GeneratedTestId",
                table: "TestAttempts",
                column: "GeneratedTestId");

            migrationBuilder.CreateIndex(
                name: "IX_TestAttempts_StudentUserId",
                table: "TestAttempts",
                column: "StudentUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "TestAttemptAnswers");

            migrationBuilder.DropTable(
                name: "TestAttempts");
        }
    }
}
