using Domain.Models.Flashcards;
using Application.Decks.Dtos;

namespace Application.Mapper
{
    public static class DeckMapper
    {
        public static DeckDto ToDto(Deck Deck)
        {
            return new DeckDto
            {
                Id = Deck.Id,
                Title = Deck.Title,
                CourseName = Deck.CourseName,
                SubjectName = Deck.SubjectName,
                CreatedUtc = Deck.CreatedAtUtc,
                CardCount = Deck.FlashCards.Count
            };
        }
    }
}