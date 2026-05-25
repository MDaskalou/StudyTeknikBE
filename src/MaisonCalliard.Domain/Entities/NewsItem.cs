using MaisonCalliard.Domain.ValueObjects;

namespace MaisonCalliard.Domain.Entities;

public sealed class NewsItem
{
    public Guid Id { get; set; }
    public LocalizedText Title { get; set; } = new();
    public LocalizedText Subtitle { get; set; } = new();
    public string ImageUrl { get; set; } = string.Empty;
    public string? Link { get; set; }
}
