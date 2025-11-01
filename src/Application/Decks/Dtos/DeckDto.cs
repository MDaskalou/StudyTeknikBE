using Application.FlashCards.Dtos;

namespace Application.Decks.Dtos
{
    public sealed record DeckDto
    {
        public Guid Id { get; set; }
        public string Title{get;set;}
        public string CourseName{get;set;}
        public string SubjectName{get;set;}
        public DateTime CreatedUtc{get;set;}
        public int CardCount {get;set;}
        
        public List<FlashCardDto> FlashCards{get;set;}

    }
}