using Domain.Models.Users;

namespace Domain.Entities
{
    public sealed class FlashCardEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        public string FrontText { get; set; }
        public string BackText { get; set; }
        
        
        public DateTime NextReviewAtUtc { get; set; }
        
        public Guid DeckId { get; set; }
        public DeckEntity Deck { get; set; } = null;
        
        public int Interval {get; set;}
        public float EaseFactor {get; set;}
    }

    
}