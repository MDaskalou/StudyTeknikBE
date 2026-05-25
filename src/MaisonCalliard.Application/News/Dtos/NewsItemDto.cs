namespace MaisonCalliard.Application.News.Dtos;

public sealed class NewsItemDto
{
    public Guid Id { get; set; }
    public LocalizedTextDto Title { get; set; } = new();
    public LocalizedTextDto Subtitle { get; set; } = new();
    public string ImageUrl { get; set; } = string.Empty;
    public string? Link { get; set; }
}

public sealed class LocalizedTextDto
{
    public string Se { get; set; } = string.Empty;
    public string En { get; set; } = string.Empty;
}
