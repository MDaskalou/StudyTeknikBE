namespace MaisonCalliard.Infrastructure.Options;

public sealed class ResendOptions
{
    public const string SectionName = "Resend";

    public string ApiKey { get; set; } = string.Empty;
    public bool Enabled { get; set; }
}
