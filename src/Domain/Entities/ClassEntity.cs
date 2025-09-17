namespace Domain.Entities
{
    public class ClassEntity
    {
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        //Egenskaper
        public string SchoolName { get; set; } = default!;
        public int Year { get; set; }
        public string ClassName { get; set; } = default!;
    }
}