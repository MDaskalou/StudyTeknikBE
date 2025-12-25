namespace Application.AI.Dtos
{
    public record FlashCardGeneratedDto(
        string FrontText,
        string BackText
    );

    public record GenerateFlashCardsResponseDto(
        int Count,
        List<FlashCardGeneratedDto> Cards
    );
}

