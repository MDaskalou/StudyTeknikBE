using Application.Abstractions.IPersistence;
using Application.Decks.IRepository;
using Domain.Entities;
using Domain.Models.Flashcards;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Application.Common.Results; // Oanvänd, men OK
using Domain.Common; // Oanvänd, men OK
using Application.Mapper; // <-- VIKTIGT: Importera dina mappers

namespace Infrastructure.Persistence.Repositories
{
    public sealed class DeckRepository : IDeckRepository
    {
        private readonly IAppDbContext _context;

        public DeckRepository(IAppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Deck deck, CancellationToken ct)
        {
            // Använd din mapper för att konvertera från domän till entitet
            var deckEntity = deck.ToEntity(); 
            
            await _context.Decks.AddAsync(deckEntity, ct);
            
            await _context.SaveChangesAsync(ct);
            
        }

        public async Task<List<Deck>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var deckEntities = await _context.Decks
                .AsNoTracking()
                .Where(d => d.UserId == userId)
                .Include(d => d.FlashCards) // <-- FIX 1: Hämta de tillhörande korten
                .ToListAsync(ct);

            // Mappa om entitetslistan till en domänlista
            var domainDecks = deckEntities.Select(deckEntity => 
                {
                    // FIX 2: Mappa om kort-entiteterna till domänmodeller
                    var flashcardModels = deckEntity.FlashCards
                        .Select(fc => fc.ToDomain()) // Använder FlashCardsMapper.ToDomain
                        .ToList();

                    // FIX 3: Anropa Load med 8 argument
                    var deck = Deck.Load(
                        deckEntity.Id,
                        deckEntity.CreatedAtUtc,
                        deckEntity.UpdatedAtUtc,
                        deckEntity.Title,
                        deckEntity.CourseName,
                        deckEntity.SubjectName,
                        deckEntity.UserId,
                        flashcardModels 
                    );
                    
                    return deck;
                })
                .ToList();

            return domainDecks;
        }

        public async Task<Deck?> GetByIdAsync(Guid id, CancellationToken ct = default) 
        {
            var deckEntity = await _context.Decks
                .AsNoTracking()
                .Include(d => d.FlashCards) 
                .FirstOrDefaultAsync(d => d.Id == id, ct);

            if (deckEntity == null)
            {
                return null;
            }
            
            // FIX 2: Mappa om korten
            var flashcardModels = deckEntity.FlashCards
                .Select(fc => fc.ToDomain())
                .ToList();

            // FIX 3: Anropa Load med 8 argument
            var domainDeck = Deck.Load(
                deckEntity.Id,
                deckEntity.CreatedAtUtc,
                deckEntity.UpdatedAtUtc,
                deckEntity.Title,
                deckEntity.CourseName,
                deckEntity.SubjectName,
                deckEntity.UserId,
                flashcardModels // <-- Det 8:e argumentet
            );
            return domainDeck;
        }

        public async Task<Deck?> GetByIdTrackedAsync(Guid deckId, CancellationToken ct = default)
        {
            var deckEntity = await _context.Decks
                .Include(d => d.FlashCards) // <-- FIX 1: Hämta de tillhörande korten
                .FirstOrDefaultAsync(d => d.Id == deckId, ct);

            if (deckEntity == null)
            {
                return null;
            }
            
            // FIX 2: Mappa om korten
            var flashcardModels = deckEntity.FlashCards
                .Select(fc => fc.ToDomain())
                .ToList();

            // FIX 3: Anropa Load med 8 argument (och korrigerad syntax)
            var domainDeck = Deck.Load(
                deckEntity.Id,
                deckEntity.CreatedAtUtc,
                deckEntity.UpdatedAtUtc,
                deckEntity.Title,
                deckEntity.CourseName,
                deckEntity.SubjectName,
                deckEntity.UserId,
                flashcardModels // <-- Det 8:e argumentet
            );
            return domainDeck;
        }

        public async Task UpdateAsync(Deck deck, CancellationToken ct)
        {
           var deckEntity = await _context.Decks
               .Include(d => d.FlashCards)
               .FirstOrDefaultAsync(d => d.Id == deck.Id, ct);

           if (deckEntity != null)
           {
               deckEntity.Title = deck.Title;
               deckEntity.CourseName = deck.CourseName;
               deckEntity.SubjectName = deck.SubjectName;
               deckEntity.UpdatedAtUtc = DateTime.UtcNow;


               var cardsToRemove = deckEntity.FlashCards
                   .Where(fe => !deck.FlashCards.Any(domainCard => domainCard.Id == fe.Id))
                   .ToList();
               _context.FlashCards.RemoveRange(cardsToRemove);

               foreach (var domianCard in deck.FlashCards)
               {
                   var existingCard = deckEntity.FlashCards.FirstOrDefault(fe => fe.Id == domianCard.Id);

                   if (existingCard == null)
                   {
                       // Nytt kort -> Lägg till
                       deckEntity.FlashCards.Add(domianCard.ToEntity());                   }
                   else
                   {
                       existingCard.UpdatedAtUtc = domianCard.UpdatedAtUtc; // FIX 5: Använd domänvärden
                       existingCard.FrontText = domianCard.FrontText;
                       existingCard.BackText = domianCard.BackText;
                       existingCard.NextReviewAtUtc = domianCard.NextReviewAtUtc;
                       existingCard.Interval = domianCard.Interval;
                       existingCard.EaseFactor = domianCard.EaseFactor;
                   }
               }

               await _context.SaveChangesAsync(ct);
           }

        }

        public async Task DeleteAsync(Guid deckId, CancellationToken ct)
        {
            var deckEntity = await _context.Decks
                .FindAsync(new object[] {deckId}, ct);

            if (deckEntity != null)
            {
                _context.Decks.Remove(deckEntity);
                await _context.SaveChangesAsync(ct);
            }
            
        }
    }
}