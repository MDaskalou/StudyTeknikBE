﻿// Infrastructure/Service/AIService.cs

 using System.Text.Json;
 using Application.Abstractions.IPersistence;
 using Application.AI.Dtos;
 using GenerativeAI;
 using Microsoft.Extensions.Configuration;
 // Se till att denna pekar på ditt interface

 namespace Infrastructure.Service;

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

     public async Task<GenerateFlashCardsResponseDto> GenerateFlashCardsAsync(string pdfContent, CancellationToken ct)
     {
         if (string.IsNullOrWhiteSpace(pdfContent))
         {
             return new GenerateFlashCardsResponseDto(0, new List<FlashCardGeneratedDto>());
         }

         var prompt = @"Du är en AI-assistent som skapar flashcards för studenter baserat på läsmaterial.

Din uppgift är att analysera följande text och skapa 10-15 fokuserade flashcards.
Varje flashcard ska ha:
- FrontText: Ett koncist begrepp, fråga eller nyckelord (max 15 ord)
- BackText: En tydlig och pedagogisk förklaring. Håll det under 60 ord för att göra det lätt att memorera.

Fokusera på:
- Viktiga koncept och definitioner
- Begrepp som är lätta att testa
- Variation i ämnena

SVAR ENDAST med giltigt JSON i denna format (ingen annan text):
{
  ""cards"": [
    {""frontText"": ""Begrepp 1"", ""backText"": ""Förklaring av begrepp 1""},
    {""frontText"": ""Begrepp 2"", ""backText"": ""Förklaring av begrepp 2""},
    ...
  ]
}

Här är texten från PDF:en:
" + pdfContent;

         try
         {
             var response = await _model.GenerateContentAsync(prompt, cancellationToken: ct);
             var responseText = response.Text;

             // Parsera JSON-svaret
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
                         var frontText = frontElement.GetString()?.Trim() ?? "";
                         var backText = backElement.GetString()?.Trim() ?? "";

                         if (!string.IsNullOrEmpty(frontText) && !string.IsNullOrEmpty(backText))
                         {
                             // SÄKERHETSSPÄRR:
                             // Klipp texten om den är över 1000 tecken för att undvika krasch
                             // (Detta skyddar även om du glömmer migrationen, men gör migrationen ändå!)
                             if (backText.Length > 1000) 
                             {
                                 backText = backText.Substring(0, 997) + "...";
                             }
                        
                             cards.Add(new FlashCardGeneratedDto(frontText, backText));
                         }
                     }
                 }
             }

             return new GenerateFlashCardsResponseDto(cards.Count, cards);
         }
         catch (System.Exception ex)
         {
             System.Console.WriteLine($"Fel vid flashcard-generering: {ex.Message}");
             return new GenerateFlashCardsResponseDto(0, new List<FlashCardGeneratedDto>());
         }
     }
 }