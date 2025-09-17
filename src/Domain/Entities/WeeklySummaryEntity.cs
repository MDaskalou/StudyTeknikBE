namespace Domain.Entities
{
    public class WeeklySummaryEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        public Guid StudentId { get; set; }
        public string YearWeek { get; set; } = default!;
        public string Text { get; set; } = string.Empty;
    }
}