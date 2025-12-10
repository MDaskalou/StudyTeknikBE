using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class RenameUserIdToStudentId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentProfiles_Users_UserId",
                table: "StudentProfiles");

            migrationBuilder.RenameColumn(
                name: "UserId",
                table: "StudentProfiles",
                newName: "StudentId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentProfiles_UserId",
                table: "StudentProfiles",
                newName: "IX_StudentProfiles_StudentId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProfiles_Users_StudentId",
                table: "StudentProfiles",
                column: "StudentId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_StudentProfiles_Users_StudentId",
                table: "StudentProfiles");

            migrationBuilder.RenameColumn(
                name: "StudentId",
                table: "StudentProfiles",
                newName: "UserId");

            migrationBuilder.RenameIndex(
                name: "IX_StudentProfiles_StudentId",
                table: "StudentProfiles",
                newName: "IX_StudentProfiles_UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_StudentProfiles_Users_UserId",
                table: "StudentProfiles",
                column: "UserId",
                principalSchema: "dbo",
                principalTable: "Users",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
