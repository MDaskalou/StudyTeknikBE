using System.Text.Json;
using Application.Abstractions.IPersistence;
using Application.AI.Dtos;
using GenerativeAI;
using Microsoft.Extensions.Configuration;
using System.Linq;
using Application.StudySessions.DTOs;
using Domain.Abstractions.Enum;

namespace Infrastructure.Service;

public class AiService : IAIService
{
    private readonly GenerativeModel _model;
    private readonly IConfiguration _configuration;

    public AiService(IConfiguration configuration)
    {
        var apiKey = configuration["GoogleAI:ApiKey"];
        
     
        _model = new GenerativeModel(
            apiKey: apiKey,
            model: "gemini-2.5-flash"
        );
    }

    public async Task<string> RewriteDiaryEntryAsync(string originalText, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(originalText))
        {
            return originalText;
        }

        var prompt = "Renskriv följande text på korrekt svenska. Returnera ENDAST den omskrivna texten, inga förklaringar, inga kommentarer, inga rubriker. Text: " + originalText;
        try
        {
            var response = await _model.GenerateContentAsync(prompt, cancellationToken: ct);
            return response.Text?.Trim() ?? "AI returnerade inget svar.";
        }
        catch (System.Exception ex)
        {
            // Vi använder strängkonkatenering (+) för att undvika "Ambiguous invocation" i Rider
            System.Console.WriteLine("FULLSTÄNDIGT FEL FRÅN GEMINI: " + ex.ToString());
            return "AI-fel: " + ex.Message;
        }
    }

    public async Task<GenerateFlashCardsResponseDto> GenerateFlashCardsAsync(string pdfContent, CancellationToken ct)
    {
        if (string.IsNullOrWhiteSpace(pdfContent))
        {
            return new GenerateFlashCardsResponseDto(0, new List<FlashCardGeneratedDto>());
        }

        var prompt = @"Skapa 10-15 flashcards baserat på följande material. 
        Returnera ENDAST giltig JSON utan förklaringar eller markdown.
            Format:
                {
                    ""cards"": [
                        { ""frontText"": ""Fråga här?"", ""backText"": ""Svar här."" }
                                ]
                 }  
                        frontText ska vara en fråga, backText ska vara svaret .
                        Material: " + pdfContent;

        try
        {
            var response = await _model.GenerateContentAsync(prompt, cancellationToken: ct);
            var responseText = response.Text;

            var jsonStartIndex = responseText.IndexOf('{');
            var jsonEndIndex = responseText.LastIndexOf('}');
            
            if (jsonStartIndex < 0 || jsonEndIndex < 0)
            {
                return new GenerateFlashCardsResponseDto(0, new List<FlashCardGeneratedDto>());
            }

            var jsonString = responseText.Substring(jsonStartIndex, jsonEndIndex - jsonStartIndex + 1);
            var jsonDocument = JsonDocument.Parse(jsonString);
            var root = jsonDocument.RootElement;

            var cards = new List<FlashCardGeneratedDto>();
            if (root.TryGetProperty("cards", out var cardsElement) && cardsElement.ValueKind == JsonValueKind.Array)
            {
                foreach (var card in cardsElement.EnumerateArray())
                {
                    if (card.TryGetProperty("frontText", out var frontElement) && 
                        card.TryGetProperty("backText", out var backElement))
                    {
                        var frontText = frontElement.GetString() ?? "";
                        var backText = backElement.GetString() ?? "";
                        cards.Add(new FlashCardGeneratedDto(frontText, backText));
                    }
                }
            }

            return new GenerateFlashCardsResponseDto(cards.Count, cards);
        }
        catch (System.Exception ex)
        {
            System.Console.WriteLine("FEL VID FLASHCARDS: " + ex.ToString());
            return new GenerateFlashCardsResponseDto(0, new List<FlashCardGeneratedDto>());
        }
    }
    
    public async Task<List<CreateStudySessionStepRequest>> GenerateStudyStepsAsync(
        string sessionGoal,
        int plannedMinutes,
        int energyLevel,
        int difficultyLevel,
        int motivationLevel,
        string? learningChallenges,
        string? studyEnvironment,
        string? additionalContext,
        CancellationToken ct)
    {
        
        var challengesText = string.IsNullOrEmpty(learningChallenges)
            ? "Inga angivna"
            : learningChallenges switch
            {
                "dyslexi"    => "Dyslexi/Lässvårigheter — använd kortare textstycken och fler repetitioner",
                "adhd"       => "Koncentrationssvårigheter/ADHD — max 15-20 min fokusblock, fler pauser",
                "minne"      => "Svårt att komma ihåg — inkludera repetitionssteg efter varje fokusblock",
                "matematik"  => "Svårt med matematik/formler — dela upp i ett problem i taget",
                "abstrakt"   => "Svårt med abstrakta begrepp — använd konkreta exempel i varje steg",
                "stress"     => "Prestationsångest/Stress — börja med andningsövning, sätt realistiska delmål",
                _            => learningChallenges
            };

        var environmentText = string.IsNullOrEmpty(studyEnvironment)
            ? "Ej angiven"
            : studyEnvironment switch
            {
                "hemma_tyst"  => "Hemma i tyst rum — bra för djupt fokusarbete",
                "hemma_stört" => "Hemma med störningsmoment — lägg in fler korta pauser",
                "bibliotek"   => "Bibliotek/Studierum — optimalt för fokus",
                "skola"       => "I skolan — passa på att fråga lärare vid pauser",
                "cafe"        => "Café — använd hörlurar och kortare fokusblock",
                _             => studyEnvironment
            };

        var extraText = string.IsNullOrEmpty(additionalContext)
            ? ""
            : $"\nExtra info från eleven: \"{additionalContext}\"";
      var prompt = $@"Du är en personlig studiecoach för en elev i Sverige.

ELEVPROFIL:
- Mål för sessionen: ""{sessionGoal}""
- Tillgänglig tid: {plannedMinutes} minuter
- Energinivå just nu (1-10): {energyLevel}
- Hur svårt upplever eleven ämnet (1-10): {difficultyLevel}
- Motivationsnivå (1-10): {motivationLevel}
- Utmaningar: {challengesText}
- Studiemiljö: {environmentText}{extraText}

KRITISKA REGLER:
1. OM målet innehåller ett antal sidor, kapitel eller uppgifter — DELA UPP dem jämnt över fokusblocken.
   Exempel: ""30 sidor"" med 3 fokusblock = 10 sidor per block. Skriv detta EXPLICIT i beskrivningen.
2. Varje fokusblock måste ha ett SPECIFIKT delmål med exakt vilket material som ska täckas.
   DÅLIGT: ""Fokus: Läs om kristendomen""
   BRA: ""Fokus: Läs sidorna 1-10 om kristendomens ursprung. Stryk under 3 nyckelbegrepp per sida.""
3. Starta med ett Förberedelsesteg (2-3 min) där eleven planerar hur materialet delas upp.
4. Avsluta med ett Repetitionssteg (2-3 min) för sammanfattning av ALLT material.
5. Anpassa fokusblockens längd efter energi och svårighet:
   - Låg energi/hög svårighet: 15-20 min fokusblock
   - Medel: 20-25 min fokusblock  
   - Hög energi/låg svårighet: 25-35 min fokusblock
6. Total tid på ALLA steg ska summera till EXAKT {plannedMinutes} minuter — inte mer, inte mindre.
7. Lägg in pauser mellan fokusblock (5 min kort paus, 10-15 min lång paus efter 2+ fokusblock).

Returnera ENDAST giltig JSON utan markdown eller förklaringar:
{{
  ""steps"": [
    {{ ""stepType"": 3, ""description"": ""Förberedelse: ..."", ""durationMinutes"": 3 }},
    {{ ""stepType"": 0, ""description"": ""Fokus: Läs sidorna X-Y om ämne Z. Gör X."", ""durationMinutes"": 20 }},
    {{ ""stepType"": 1, ""description"": ""Paus: Res dig och sträck på benen."", ""durationMinutes"": 5 }},
    {{ ""stepType"": 3, ""description"": ""Repetition: Sammanfatta alla X sidor du läst."", ""durationMinutes"": 2 }}
  ]
}}

StepType: 0=Fokus, 1=KortPaus, 2=LångPaus, 3=Förberedelse/Repetition";

        var response = await _model.GenerateContentAsync(prompt, cancellationToken: ct);
        var responseText = response.Text;
    
        var jsonStart = responseText.IndexOf('{');
        var jsonEnd = responseText.LastIndexOf('}');
        var jsonString = responseText.Substring(jsonStart, jsonEnd - jsonStart + 1);
    
        var doc = JsonDocument.Parse(jsonString);
        var steps = new List<CreateStudySessionStepRequest>();
    
        if (doc.RootElement.TryGetProperty("steps", out var stepsEl))
        {
            foreach (var step in stepsEl.EnumerateArray())
            {
                steps.Add(new CreateStudySessionStepRequest(
                    StepType: (SessionStepType)step.GetProperty("stepType").GetInt32(),
                    Description: step.GetProperty("description").GetString() ?? "",
                    DurationMinutes: step.GetProperty("durationMinutes").GetInt32()
                ));
            }
        }
    
        return steps;
    }
    public async Task ListModelsDebugAsync()
    {
        var apiKey = _configuration["GoogleAI:ApiKey"];
        using var client = new HttpClient();
    
        try 
        {
            // Vi anropar Googles API direkt för att se alla modeller
            var url = $"https://generativelanguage.googleapis.com/v1beta/models?key={apiKey}";
            var response = await client.GetStringAsync(url);
        
            Console.WriteLine("--- TILLGÄNGLIGA MODELLER FRÅN GOOGLE ---");
            Console.WriteLine(response);
            Console.WriteLine("------------------------------------------");
        }
        catch (Exception ex)
        {
            Console.WriteLine("Kunde inte hämta modell-listan: " + ex.Message);
        }
    }
}