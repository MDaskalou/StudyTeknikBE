namespace MaisonCalliard.Application.Settings.Dtos;

public sealed class LeadTimeDto
{
    public int LeadTimeDays { get; set; }
}

public sealed class UpdateLeadTimeRequest
{
    public int LeadTimeDays { get; set; }
}
