namespace Application.FlashCards.Dtos
{
    public record FlashCardDto(
        
        Guid Id,
        string FrontText,
        string BackText,
        DateTime CreatedAtUtc
        );
}