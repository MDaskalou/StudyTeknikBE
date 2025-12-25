using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class FixDeckDeleteBehavior : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlashCards_Decks_DeckId",
                table: "FlashCards");

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCards_Decks_DeckId",
                table: "FlashCards",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FlashCards_Decks_DeckId",
                table: "FlashCards");

            migrationBuilder.AddForeignKey(
                name: "FK_FlashCards_Decks_DeckId",
                table: "FlashCards",
                column: "DeckId",
                principalTable: "Decks",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
