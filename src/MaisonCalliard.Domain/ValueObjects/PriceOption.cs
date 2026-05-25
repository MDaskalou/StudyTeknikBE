namespace MaisonCalliard.Domain.ValueObjects;

public sealed class PriceOption
{
    public string Label { get; set; } = string.Empty;
    public decimal Price { get; set; }
}
