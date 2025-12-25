namespace Application.AI.Dtos
{
    public record GenerateFlashCardsRequestDto(
        string PdfContent,  // Texten extraherad från PDF
        Guid DeckId         // Vilket deck ska korten läggas till i
    );
}

