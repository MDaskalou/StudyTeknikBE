namespace MaisonCalliard.Domain.Entities;

public sealed class AppSettings
{
    public Guid Id { get; set; }
    public int LeadTimeDays { get; set; } = 3;
}
