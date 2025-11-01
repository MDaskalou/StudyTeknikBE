using Domain.Models.Flashcards;
using Application.Decks.Dtos;
using Domain.Entities;

namespace Application.Mapper
{
    public static class DeckMapper
    {
        // === Din befintliga metod (med en liten fix) ===
        public static DeckDto ToDto(Deck deck) // <-- Bytte 'Deck' till 'deck'
        {
            return new DeckDto
            {
                Id = deck.Id,
                Title = deck.Title,
                CourseName = deck.CourseName,
                SubjectName = deck.SubjectName,
                CreatedUtc = deck.CreatedAtUtc,
                CardCount = deck.FlashCards.Count,
                
                FlashCards = deck.FlashCards
                    .Select(fc => fc.ToDto())
                    .ToList()
                
            };
        }

        public static DeckEntity ToEntity(this Deck domain)
        {
            var entity = new DeckEntity
            {
                Id = domain.Id,
                CreatedAtUtc = domain.CreatedAtUtc,
                UpdatedAtUtc = domain.UpdatedAtUtc,
                Title = domain.Title,
                CourseName = domain.CourseName,
                SubjectName = domain.SubjectName,
                UserId = domain.UserId,
                FlashCards = new List<FlashCardEntity>()
            };

            foreach (var cardDomain in domain.FlashCards)
            {
                entity.FlashCards.Add(cardDomain.ToEntity());
            }

            return entity;
        }
    }
}