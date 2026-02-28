using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PaperTestChecker.Migrations
{
    /// <inheritdoc />
    public partial class AddTeacherFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string[]>(
                name: "Options",
                table: "QuestionResults",
                type: "text[]",
                nullable: false,
                defaultValue: new string[0]);

            migrationBuilder.CreateTable(
                name: "GeneratedTests",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    CreatedByUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    ForStudentUserId = table.Column<Guid>(type: "uuid", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedTests", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedTests_Users_CreatedByUserId",
                        column: x => x.CreatedByUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_GeneratedTests_Users_ForStudentUserId",
                        column: x => x.ForStudentUserId,
                        principalTable: "Users",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "GeneratedTestItems",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    GeneratedTestId = table.Column<Guid>(type: "uuid", nullable: false),
                    QuestionNumber = table.Column<int>(type: "integer", nullable: false),
                    QuestionText = table.Column<string>(type: "text", nullable: false),
                    Options = table.Column<string[]>(type: "text[]", nullable: false),
                    CorrectAnswer = table.Column<string>(type: "text", nullable: false),
                    SourceQuestionResultId = table.Column<Guid>(type: "uuid", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GeneratedTestItems", x => x.Id);
                    table.ForeignKey(
                        name: "FK_GeneratedTestItems_GeneratedTests_GeneratedTestId",
                        column: x => x.GeneratedTestId,
                        principalTable: "GeneratedTests",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedTestItems_GeneratedTestId",
                table: "GeneratedTestItems",
                column: "GeneratedTestId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedTests_CreatedByUserId",
                table: "GeneratedTests",
                column: "CreatedByUserId");

            migrationBuilder.CreateIndex(
                name: "IX_GeneratedTests_ForStudentUserId",
                table: "GeneratedTests",
                column: "ForStudentUserId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GeneratedTestItems");

            migrationBuilder.DropTable(
                name: "GeneratedTests");

            migrationBuilder.DropColumn(
                name: "Options",
                table: "QuestionResults");
        }
    }
}
