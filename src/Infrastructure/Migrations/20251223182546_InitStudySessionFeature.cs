using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitStudySessionFeature : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySessions_Courses_CourseId",
                table: "StudySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudySessions_Users_UserId",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "BreakFeedback",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "WorkFeedback",
                table: "StudySessions");

            migrationBuilder.RenameColumn(
                name: "WorkDurationMinutes",
                table: "StudySessions",
                newName: "Status");

            migrationBuilder.RenameColumn(
                name: "TaskDescription",
                table: "StudySessions",
                newName: "SessionGoal");

            migrationBuilder.RenameColumn(
                name: "StartDate",
                table: "StudySessions",
                newName: "UpdatedAtUtc");

            migrationBuilder.AddColumn<int>(
                name: "ActualMinutes",
                table: "StudySessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAtUtc",
                table: "StudySessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "EndDateUtc",
                table: "StudySessions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "EnergyEnd",
                table: "StudySessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "EnergyStart",
                table: "StudySessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "PlannedMinutes",
                table: "StudySessions",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartDateUtc",
                table: "StudySessions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.CreateTable(
                name: "StudySessionSteps",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    StudySessionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    OrderIndex = table.Column<int>(type: "int", nullable: false),
                    StepType = table.Column<int>(type: "int", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(300)", maxLength: 300, nullable: false),
                    DurationMinutes = table.Column<int>(type: "int", nullable: false),
                    IsCompleted = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAtUtc = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_StudySessionSteps", x => x.Id);
                    table.ForeignKey(
                        name: "FK_StudySessionSteps_StudySessions_StudySessionId",
                        column: x => x.StudySessionId,
                        principalTable: "StudySessions",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_StudySessions_Status",
                table: "StudySessions",
                column: "Status");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessionSteps_OrderIndex",
                table: "StudySessionSteps",
                column: "OrderIndex");

            migrationBuilder.CreateIndex(
                name: "IX_StudySessionSteps_StudySessionId",
                table: "StudySessionSteps",
                column: "StudySessionId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySessions_Courses_CourseId",
                table: "StudySessions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_StudySessions_Users_UserId",
                table: "StudySessions",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudySessions_Courses_CourseId",
                table: "StudySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_StudySessions_Users_UserId",
                table: "StudySessions");

            migrationBuilder.DropTable(
                name: "StudySessionSteps");

            migrationBuilder.DropIndex(
                name: "IX_StudySessions_Status",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "ActualMinutes",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "CreatedAtUtc",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "EndDateUtc",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "EnergyEnd",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "EnergyStart",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "PlannedMinutes",
                table: "StudySessions");

            migrationBuilder.DropColumn(
                name: "StartDateUtc",
                table: "StudySessions");

            migrationBuilder.RenameColumn(
                name: "UpdatedAtUtc",
                table: "StudySessions",
                newName: "StartDate");

            migrationBuilder.RenameColumn(
                name: "Status",
                table: "StudySessions",
                newName: "WorkDurationMinutes");

            migrationBuilder.RenameColumn(
                name: "SessionGoal",
                table: "StudySessions",
                newName: "TaskDescription");

            migrationBuilder.AddColumn<string>(
                name: "BreakFeedback",
                table: "StudySessions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "WorkFeedback",
                table: "StudySessions",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySessions_Courses_CourseId",
                table: "StudySessions",
                column: "CourseId",
                principalTable: "Courses",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudySessions_Users_UserId",
                table: "StudySessions",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id");
        }
    }
}
