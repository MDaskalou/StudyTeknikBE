using Domain.Models.Diary;

namespace Application.Abstractions.IPersistence
{
    public interface IAIService
    {
        Task<string> RewriteDiaryEntryAsync(string originalText, CancellationToken cancellationToken);
    }
}