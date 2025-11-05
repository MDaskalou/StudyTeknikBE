namespace Domain.Entities
{
    public class SubjectEntity
    {
        public Guid id { get; set; }
        public string Name { get; set; } 
        public Guid UserId { get; set; }
        DateTime CreatedAtUtc { get; set; }
    }
}