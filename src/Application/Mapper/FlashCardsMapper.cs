
using Application.FlashCards.Dtos;
using Domain.Entities;         
using Domain.Models.Flashcards; 

namespace Application.Mapper 
{
    public static class FlashCardsMapper 
    {
        
        public static FlashCard ToDomain(this FlashCardEntity entity)
        {
            // Använder din statiska 'Load'-metod
            return FlashCard.Load(
                entity.Id,
                entity.CreatedAtUtc,
                entity.UpdatedAtUtc,
                entity.FrontText,
                entity.BackText,
                entity.DeckId,
                entity.NextReviewAtUtc,
                entity.Interval,
                entity.EaseFactor
            );
        }

       
        public static FlashCardEntity ToEntity(this FlashCard domain)
        {
            return new FlashCardEntity
            {
                // Nu bör alla 'set'-anrop fungera
                Id = domain.Id,
                CreatedAtUtc = domain.CreatedAtUtc,
                UpdatedAtUtc = domain.UpdatedAtUtc,
                FrontText = domain.FrontText,
                BackText = domain.BackText,
                DeckId = domain.DeckId,
                NextReviewAtUtc = domain.NextReviewAtUtc,
                Interval = domain.Interval,
                EaseFactor = domain.EaseFactor
            };
        }

        public static FlashCardDto ToDto(this FlashCard domain)
        {
            // Din 'record'-konstruktor för DTO:n
            return new FlashCardDto(
                domain.Id,
                domain.FrontText,
                domain.BackText,
                domain.CreatedAtUtc
            );
        }
    }
}