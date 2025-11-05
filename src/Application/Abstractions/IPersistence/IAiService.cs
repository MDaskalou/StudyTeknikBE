using Application.AI.Dtos;
using Domain.Models.Diary;

namespace Application.Abstractions.IPersistence
{
    public interface IAiService
    {
        public record AiGeneratedCard(string FrontText, string BackText);

        Task<string> RewriteDiaryEntryAsync(string originalText, CancellationToken cancellationToken);
        
        Task<List<AiGeneratedCardDto>> GenerateFlashcardsFromDocumentAsync(
            Stream fileStream, 
            string fileName, 
            CancellationToken cancellationToken);
    }
}