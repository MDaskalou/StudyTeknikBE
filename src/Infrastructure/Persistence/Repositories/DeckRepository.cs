using Application.Abstractions.IPersistence;
using Application.Decks.IRepository;
using Domain.Entities;
using Domain.Models.Flashcards;
using Microsoft.EntityFrameworkCore;
using System.Linq;

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
                        // Notera: Vi laddar INTE hela User eller FlashCards-listan här
                        // eftersom domänmodellen ska vara fokuserad.
                    );
                    
                    // Om du behöver CardCount (t.ex. för mappning till DTO) 
                    // kan du behöva lägga till det i Load eller mappa separat.
                    // För nu antar vi att DeckMapper kan hantera det.
                    
                    return deck;
                })
                .ToList();

            return domainDecks;
        }
        
    }
}