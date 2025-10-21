
namespace Domain.Entities
{
    public sealed class DeckEntity
    {
        public Guid Id {get;set;}
        public DateTime CreatedAtUtc {get;set;}
        public DateTime UpdatedAtUtc {get;set;}

        public string Title { get; set; }
        public string CourseName {get;set;}
        
        public string SubjectName {get;set;}
        
        public Guid UserId {get;set;}
        public UserEntity User {get;set;}
        
        public List<FlashCardEntity> FlashCards {get;set;}
        
        
    }
}