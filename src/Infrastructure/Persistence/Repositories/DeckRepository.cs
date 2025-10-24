using Application.Abstractions.IPersistence;
using Application.Decks.IRepository;
using Domain.Entities;
using Domain.Models.Flashcards;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using Application.Common.Results;
using Domain.Common;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class DeckRepository :IDeckRepository
    {
        private readonly IAppDbContext _context;

        public DeckRepository(IAppDbContext context)
        {
            _context = context;
        }

        public async Task AddAsync(Deck deck, CancellationToken ct)
        {
            var deckEntity = new DeckEntity
            {
                Id = deck.Id,
                CreatedAtUtc = deck.CreatedAtUtc,
                UpdatedAtUtc = deck.UpdatedAtUtc,
                Title = deck.Title,
                CourseName = deck.CourseName,
                SubjectName = deck.SubjectName,
                UserId = deck.UserId
            };
            
            await _context.Decks.AddAsync(deckEntity, ct);
            
            await _context.SaveChangesAsync(ct);
        }

        public async Task<List<Deck>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            // 1. Hämta alla "tunna" entiteter för användaren
            var deckEntities = await _context.Decks
                .AsNoTracking() // Viktigt för prestanda vid läsning
                .Where(d => d.UserId == userId)
                // Ladda antalet kort effektivt (om DeckDto behöver CardCount)
                .Select(d => new 
                {
                    Entity = d,
                    CardCount = d.FlashCards.Count() // Räkna korten i databasen
                }) 
                .ToListAsync(ct);

            // 2. Mappa "tunna" entiteter till "rika" domänmodeller
            var domainDecks = deckEntities.Select(wrapper => 
                {
                    // Använd din Load-metod (eller en separat mappningsfunktion)
                    var deck = Deck.Load(
                        wrapper.Entity.Id,
                        wrapper.Entity.CreatedAtUtc,
                        wrapper.Entity.UpdatedAtUtc,
                        wrapper.Entity.Title,
                        wrapper.Entity.CourseName,
                        wrapper.Entity.SubjectName,
                        wrapper.Entity.UserId
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
                .FirstOrDefaultAsync(d => d.Id == id, ct);

            if (deckEntity == null)
            {
                return null;
            }
            
            var domainDeck = Deck.Load(
                deckEntity.Id,
                deckEntity.CreatedAtUtc,
                deckEntity.UpdatedAtUtc,
                deckEntity.Title,
                deckEntity.CourseName,
                deckEntity.SubjectName,
                deckEntity.UserId
            );
                return domainDeck;
        }

        public async Task<Deck?> GetByIdTrackedAsync(Guid deckId, CancellationToken ct = default)
        {
            var deckEntity = await _context.Decks
                .FirstOrDefaultAsync(d => d.Id == deckId, ct);

            if (deckEntity == null)
            {
                return null;
            }
            
            var domainDeck = Deck.Load(
                deckEntity.Id,
                deckEntity.CreatedAtUtc,
                deckEntity.UpdatedAtUtc,
                deckEntity.Title,
                deckEntity.CourseName,
                deckEntity.SubjectName,
                deckEntity.UserId
            );
                return domainDeck;
            
        }

        public async Task UpdateAsync(Deck deck, CancellationToken ct)
        {
            var deckEnity = await _context.Decks
                .FindAsync(new object[] {deck.Id}, ct);

            if (deckEnity != null)
            {
                deckEnity.Title = deck.Title;
                deckEnity.CourseName = deck.CourseName;
                deckEnity.SubjectName = deck.SubjectName;
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
                
            }
            await _context.SaveChangesAsync(ct);
        }
        
    }
}