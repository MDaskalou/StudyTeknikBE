using Domain.Models.Flashcards;

namespace Application.Decks.IRepository
{
    public interface IDeckRepository
    {
        Task AddAsync(Deck deck, CancellationToken ct);
        
        Task <List<Deck>> GetByUserIdAsync(Guid userId,CancellationToken ct = default);
        
    }
}