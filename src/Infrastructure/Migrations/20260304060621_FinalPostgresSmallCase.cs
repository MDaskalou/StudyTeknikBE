using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FinalPostgresSmallCase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_studentProfiles_StudentProfileId",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "FK_Decks_users_UserId",
                table: "Decks");

            migrationBuilder.DropForeignKey(
                name: "FK_FlashCards_Decks_DeckId",
                table: "FlashCards");

            migrationBuilder.DropForeignKey(
                name: "FK_studentProfiles_users_StudentId",
                table: "studentProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyGoals_courses_CourseId",
                table: "StudyGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyGoals_users_UserId",
                table: "StudyGoals");

            migrationBuilder.DropForeignKey(
                name: "FK_StudyPlanTasks_StudyGoals_StudyGoalId",
                table: "StudyPlanTasks");

            migrationBuilder.DropForeignKey(
                name: "FK_studySessions_courses_CourseId",
                table: "studySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_studySessions_users_UserId",
                table: "studySessions");

            migrationBuilder.DropForeignKey(
                name: "FK_studySessionSteps_studySessions_StudySessionId",
                table: "studySessionSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_weeklySummaries",
                table: "weeklySummaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_studySessionSteps",
                table: "studySessionSteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_studySessions",
                table: "studySessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyPlanTasks",
                table: "StudyPlanTasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_StudyGoals",
                table: "StudyGoals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_studentProfiles",
                table: "studentProfiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mentorAssigments",
                table: "mentorAssigments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_FlashCards",
                table: "FlashCards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_diaryEntries",
                table: "diaryEntries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Decks",
                table: "Decks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_auditLogs",
                table: "auditLogs");

            migrationBuilder.RenameTable(
                name: "weeklySummaries",
                newName: "weeklysummaries");

            migrationBuilder.RenameTable(
                name: "studySessionSteps",
                newName: "studysessionsteps");

            migrationBuilder.RenameTable(
                name: "studySessions",
                newName: "studysessions");

            migrationBuilder.RenameTable(
                name: "StudyPlanTasks",
                newName: "studyplantasks");

            migrationBuilder.RenameTable(
                name: "StudyGoals",
                newName: "studygoals");

            migrationBuilder.RenameTable(
                name: "studentProfiles",
                newName: "studentprofiles");

            migrationBuilder.RenameTable(
                name: "mentorAssigments",
                newName: "mentorassigments");

            migrationBuilder.RenameTable(
                name: "FlashCards",
                newName: "flashcards");

            migrationBuilder.RenameTable(
                name: "Enrollments",
                newName: "enrollments");

            migrationBuilder.RenameTable(
                name: "diaryEntries",
                newName: "diaryentries");

            migrationBuilder.RenameTable(
                name: "Decks",
                newName: "decks");

            migrationBuilder.RenameTable(
                name: "auditLogs",
                newName: "auditlogs");

            migrationBuilder.RenameIndex(
                name: "IX_weeklySummaries_StudentId_YearWeek",
                table: "weeklysummaries",
                newName: "IX_weeklysummaries_StudentId_YearWeek");

            migrationBuilder.RenameIndex(
                name: "IX_studySessionSteps_StudySessionId",
                table: "studysessionsteps",
                newName: "IX_studysessionsteps_StudySessionId");

            migrationBuilder.RenameIndex(
                name: "IX_studySessionSteps_OrderIndex",
                table: "studysessionsteps",
                newName: "IX_studysessionsteps_OrderIndex");

            migrationBuilder.RenameIndex(
                name: "IX_studySessions_UserId",
                table: "studysessions",
                newName: "IX_studysessions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_studySessions_Status",
                table: "studysessions",
                newName: "IX_studysessions_Status");

            migrationBuilder.RenameIndex(
                name: "IX_studySessions_CourseId",
                table: "studysessions",
                newName: "IX_studysessions_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyPlanTasks_StudyGoalId",
                table: "studyplantasks",
                newName: "IX_studyplantasks_StudyGoalId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyGoals_UserId",
                table: "studygoals",
                newName: "IX_studygoals_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudyGoals_CourseId",
                table: "studygoals",
                newName: "IX_studygoals_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_studentProfiles_StudentId",
                table: "studentprofiles",
                newName: "IX_studentprofiles_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_mentorAssigments_MentorId_StudentId",
                table: "mentorassigments",
                newName: "IX_mentorassigments_MentorId_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_FlashCards_NextReviewAtUtc",
                table: "flashcards",
                newName: "IX_flashcards_NextReviewAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_FlashCards_DeckId",
                table: "flashcards",
                newName: "IX_flashcards_DeckId");

            migrationBuilder.RenameIndex(
                name: "IX_Enrollments_StudentId_ClassId",
                table: "enrollments",
                newName: "IX_enrollments_StudentId_ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_diaryEntries_StudentId_EntryDate",
                table: "diaryentries",
                newName: "IX_diaryentries_StudentId_EntryDate");

            migrationBuilder.RenameIndex(
                name: "IX_Decks_UserId",
                table: "decks",
                newName: "IX_decks_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_weeklysummaries",
                table: "weeklysummaries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_studysessionsteps",
                table: "studysessionsteps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_studysessions",
                table: "studysessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_studyplantasks",
                table: "studyplantasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_studygoals",
                table: "studygoals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_studentprofiles",
                table: "studentprofiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mentorassigments",
                table: "mentorassigments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_flashcards",
                table: "flashcards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_enrollments",
                table: "enrollments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_diaryentries",
                table: "diaryentries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_decks",
                table: "decks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_auditlogs",
                table: "auditlogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_studentprofiles_StudentProfileId",
                table: "courses",
                column: "StudentProfileId",
                principalTable: "studentprofiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_decks_users_UserId",
                table: "decks",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_flashcards_decks_DeckId",
                table: "flashcards",
                column: "DeckId",
                principalTable: "decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_studentprofiles_users_StudentId",
                table: "studentprofiles",
                column: "StudentId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_studygoals_courses_CourseId",
                table: "studygoals",
                column: "CourseId",
                principalTable: "courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_studygoals_users_UserId",
                table: "studygoals",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_studyplantasks_studygoals_StudyGoalId",
                table: "studyplantasks",
                column: "StudyGoalId",
                principalTable: "studygoals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_studysessions_courses_CourseId",
                table: "studysessions",
                column: "CourseId",
                principalTable: "courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_studysessions_users_UserId",
                table: "studysessions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_studysessionsteps_studysessions_StudySessionId",
                table: "studysessionsteps",
                column: "StudySessionId",
                principalTable: "studysessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_courses_studentprofiles_StudentProfileId",
                table: "courses");

            migrationBuilder.DropForeignKey(
                name: "FK_decks_users_UserId",
                table: "decks");

            migrationBuilder.DropForeignKey(
                name: "FK_flashcards_decks_DeckId",
                table: "flashcards");

            migrationBuilder.DropForeignKey(
                name: "FK_studentprofiles_users_StudentId",
                table: "studentprofiles");

            migrationBuilder.DropForeignKey(
                name: "FK_studygoals_courses_CourseId",
                table: "studygoals");

            migrationBuilder.DropForeignKey(
                name: "FK_studygoals_users_UserId",
                table: "studygoals");

            migrationBuilder.DropForeignKey(
                name: "FK_studyplantasks_studygoals_StudyGoalId",
                table: "studyplantasks");

            migrationBuilder.DropForeignKey(
                name: "FK_studysessions_courses_CourseId",
                table: "studysessions");

            migrationBuilder.DropForeignKey(
                name: "FK_studysessions_users_UserId",
                table: "studysessions");

            migrationBuilder.DropForeignKey(
                name: "FK_studysessionsteps_studysessions_StudySessionId",
                table: "studysessionsteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_weeklysummaries",
                table: "weeklysummaries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_studysessionsteps",
                table: "studysessionsteps");

            migrationBuilder.DropPrimaryKey(
                name: "PK_studysessions",
                table: "studysessions");

            migrationBuilder.DropPrimaryKey(
                name: "PK_studyplantasks",
                table: "studyplantasks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_studygoals",
                table: "studygoals");

            migrationBuilder.DropPrimaryKey(
                name: "PK_studentprofiles",
                table: "studentprofiles");

            migrationBuilder.DropPrimaryKey(
                name: "PK_mentorassigments",
                table: "mentorassigments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_flashcards",
                table: "flashcards");

            migrationBuilder.DropPrimaryKey(
                name: "PK_enrollments",
                table: "enrollments");

            migrationBuilder.DropPrimaryKey(
                name: "PK_diaryentries",
                table: "diaryentries");

            migrationBuilder.DropPrimaryKey(
                name: "PK_decks",
                table: "decks");

            migrationBuilder.DropPrimaryKey(
                name: "PK_auditlogs",
                table: "auditlogs");

            migrationBuilder.RenameTable(
                name: "weeklysummaries",
                newName: "weeklySummaries");

            migrationBuilder.RenameTable(
                name: "studysessionsteps",
                newName: "studySessionSteps");

            migrationBuilder.RenameTable(
                name: "studysessions",
                newName: "studySessions");

            migrationBuilder.RenameTable(
                name: "studyplantasks",
                newName: "StudyPlanTasks");

            migrationBuilder.RenameTable(
                name: "studygoals",
                newName: "StudyGoals");

            migrationBuilder.RenameTable(
                name: "studentprofiles",
                newName: "studentProfiles");

            migrationBuilder.RenameTable(
                name: "mentorassigments",
                newName: "mentorAssigments");

            migrationBuilder.RenameTable(
                name: "flashcards",
                newName: "FlashCards");

            migrationBuilder.RenameTable(
                name: "enrollments",
                newName: "Enrollments");

            migrationBuilder.RenameTable(
                name: "diaryentries",
                newName: "diaryEntries");

            migrationBuilder.RenameTable(
                name: "decks",
                newName: "Decks");

            migrationBuilder.RenameTable(
                name: "auditlogs",
                newName: "auditLogs");

            migrationBuilder.RenameIndex(
                name: "IX_weeklysummaries_StudentId_YearWeek",
                table: "weeklySummaries",
                newName: "IX_weeklySummaries_StudentId_YearWeek");

            migrationBuilder.RenameIndex(
                name: "IX_studysessionsteps_StudySessionId",
                table: "studySessionSteps",
                newName: "IX_studySessionSteps_StudySessionId");

            migrationBuilder.RenameIndex(
                name: "IX_studysessionsteps_OrderIndex",
                table: "studySessionSteps",
                newName: "IX_studySessionSteps_OrderIndex");

            migrationBuilder.RenameIndex(
                name: "IX_studysessions_UserId",
                table: "studySessions",
                newName: "IX_studySessions_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_studysessions_Status",
                table: "studySessions",
                newName: "IX_studySessions_Status");

            migrationBuilder.RenameIndex(
                name: "IX_studysessions_CourseId",
                table: "studySessions",
                newName: "IX_studySessions_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_studyplantasks_StudyGoalId",
                table: "StudyPlanTasks",
                newName: "IX_StudyPlanTasks_StudyGoalId");

            migrationBuilder.RenameIndex(
                name: "IX_studygoals_UserId",
                table: "StudyGoals",
                newName: "IX_StudyGoals_UserId");

            migrationBuilder.RenameIndex(
                name: "IX_studygoals_CourseId",
                table: "StudyGoals",
                newName: "IX_StudyGoals_CourseId");

            migrationBuilder.RenameIndex(
                name: "IX_studentprofiles_StudentId",
                table: "studentProfiles",
                newName: "IX_studentProfiles_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_mentorassigments_MentorId_StudentId",
                table: "mentorAssigments",
                newName: "IX_mentorAssigments_MentorId_StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_flashcards_NextReviewAtUtc",
                table: "FlashCards",
                newName: "IX_FlashCards_NextReviewAtUtc");

            migrationBuilder.RenameIndex(
                name: "IX_flashcards_DeckId",
                table: "FlashCards",
                newName: "IX_FlashCards_DeckId");

            migrationBuilder.RenameIndex(
                name: "IX_enrollments_StudentId_ClassId",
                table: "Enrollments",
                newName: "IX_Enrollments_StudentId_ClassId");

            migrationBuilder.RenameIndex(
                name: "IX_diaryentries_StudentId_EntryDate",
                table: "diaryEntries",
                newName: "IX_diaryEntries_StudentId_EntryDate");

            migrationBuilder.RenameIndex(
                name: "IX_decks_UserId",
                table: "Decks",
                newName: "IX_Decks_UserId");

            migrationBuilder.AddPrimaryKey(
                name: "PK_weeklySummaries",
                table: "weeklySummaries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_studySessionSteps",
                table: "studySessionSteps",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_studySessions",
                table: "studySessions",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyPlanTasks",
                table: "StudyPlanTasks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_StudyGoals",
                table: "StudyGoals",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_studentProfiles",
                table: "studentProfiles",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_mentorAssigments",
                table: "mentorAssigments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_FlashCards",
                table: "FlashCards",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Enrollments",
                table: "Enrollments",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_diaryEntries",
                table: "diaryEntries",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Decks",
                table: "Decks",
                column: "Id");

            migrationBuilder.AddPrimaryKey(
                name: "PK_auditLogs",
                table: "auditLogs",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_courses_studentProfiles_StudentProfileId",
                table: "courses",
                column: "StudentProfileId",
                principalTable: "studentProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Decks_users_UserId",
                table: "Decks",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCards_Decks_DeckId",
                table: "FlashCards",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_studentProfiles_users_StudentId",
                table: "studentProfiles",
                column: "StudentId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyGoals_courses_CourseId",
                table: "StudyGoals",
                column: "CourseId",
                principalTable: "courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_StudyGoals_users_UserId",
                table: "StudyGoals",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_StudyPlanTasks_StudyGoals_StudyGoalId",
                table: "StudyPlanTasks",
                column: "StudyGoalId",
                principalTable: "StudyGoals",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_studySessions_courses_CourseId",
                table: "studySessions",
                column: "CourseId",
                principalTable: "courses",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_studySessions_users_UserId",
                table: "studySessions",
                column: "UserId",
                principalTable: "users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_studySessionSteps_studySessions_StudySessionId",
                table: "studySessionSteps",
                column: "StudySessionId",
                principalTable: "studySessions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
