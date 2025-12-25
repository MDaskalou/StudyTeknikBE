namespace Application.AI.Dtos
{
    public record GenerateFlashCardsFromTextRequestDto(
        string PdfContent,
        Guid DeckId
    );
}

