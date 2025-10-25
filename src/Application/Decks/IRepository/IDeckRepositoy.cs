using Domain.Models.Flashcards;

namespace Application.Decks.IRepository
{
    public interface IDeckRepository
    {
        Task AddAsync(Deck deck, CancellationToken ct);
        
        Task <List<Deck>> GetByUserIdAsync(Guid userId,CancellationToken ct = default);
        
        Task<Deck?> GetByIdAsync(Guid id, CancellationToken ct = default);
        
        Task<Deck> GetByIdTrackedAsync(Guid deckId, CancellationToken ct);
        
        Task UpdateAsync(Deck deck, CancellationToken ct);
        
        Task DeleteAsync(Guid userId, CancellationToken ct);
        
    }
}