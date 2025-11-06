namespace Domain.Entities
{
    public class SubjectEntity
    {
        public Guid Id { get; set; }
        public string Name { get; set; } 
        public Guid UserId { get; set; }
        public DateTime CreatedAtUtc { get; set; }
    }
}