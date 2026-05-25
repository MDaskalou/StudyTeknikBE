namespace MaisonCalliard.Application.News.Dtos;

public sealed class UpdateNewsItemRequest
{
    public string TitleSe { get; set; } = string.Empty;
    public string TitleEn { get; set; } = string.Empty;
    public string SubtitleSe { get; set; } = string.Empty;
    public string SubtitleEn { get; set; } = string.Empty;
    public string? Link { get; set; }
}
