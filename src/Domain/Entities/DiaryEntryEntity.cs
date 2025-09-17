namespace Domain.Entities
{
    public sealed class DiaryEntryEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        public Guid StudentId { get; set; }
        public DateOnly EntryDate { get; set; }
        public string Text { get; set; } = string.Empty;

        
    }
}