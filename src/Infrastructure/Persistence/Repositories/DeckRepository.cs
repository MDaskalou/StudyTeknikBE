using Application.Abstractions.IPersistence;
using Application.Decks.IRepository;
using Domain.Entities;
using Domain.Models.Flashcards;

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
        }
    }
}