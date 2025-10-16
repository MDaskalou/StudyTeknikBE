// Infrastructure/Service/AIService.cs

using Application.Abstractions.IPersistence; // Se till att denna pekar på ditt interface
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using GenerativeAI;

public class AIService : IAIService
{
    private readonly GenerativeModel _model;

    public AIService(IConfiguration configuration)
    {
        var apiKey = configuration["GoogleAI:ApiKey"];
        
        // Skapa modellen. Mycket enklare!
        _model = new GenerativeModel(
            model: "gemini-pro-latest",
            apiKey: apiKey
        );
    }

    public async Task<string> RewriteDiaryEntryAsync(string originalText, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(originalText))
        {
            return originalText;
        }

        var prompt = $"""
        Du är en hjälpsam AI-assistent för en student. Ditt uppdrag är att renskriva och förbättra ett dagboksinlägg om studier.
        Förbättra grammatik, meningsstruktur och tydlighet. Se till att texten blir mer sammanhängande och välformulerad.
        Behåll den ursprungliga meningen och alla kärnidéer. Tonen ska vara personlig men något mer reflekterande och strukturerad.
        Svara ENDAST med den nya, renskrivna texten.

        Originaltext:
        "{originalText}"

        Renskriven text:
        """;

        try
        {
            // Anropet är också mycket enklare
            var response = await _model.GenerateContentAsync(prompt, cancellationToken: ct);
            
            // Och att hämta texten är superenkelt
            var rewrittenText = response.Text;
            
            return rewrittenText.Trim();
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine($"Fel vid anrop till Gemini: {ex.Message}");
            return "Kunde inte generera text från AI på grund av ett serverfel.";
        }
    }
}