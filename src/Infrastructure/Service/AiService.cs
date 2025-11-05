// Fil: Infrastructure/Services/AIService.cs

using Application.Abstractions.IPersistence; 
using Microsoft.Extensions.Configuration;
using System.Threading.Tasks;
using GenerativeAI;
using GenerativeAI.Types; // För GenerationConfig
using Application.AI.Dtos; 
using System.Collections.Generic;
using System.IO;
using System.Text; 
using System.Text.Json; 
using System.Threading;
using UglyToad.PdfPig; 
using System.Linq;
using System; 

namespace Infrastructure.Service
{
    public class AiService : IAiService
    {
        private readonly GenerativeModel _model; 
        private readonly IConfiguration _configuration;
        
        public AiService(IConfiguration configuration)
        {
            _configuration = configuration;
            var apiKey = configuration["GoogleAI:ApiKey"];
            if (string.IsNullOrEmpty(apiKey))
            {
                throw new InvalidOperationException("GoogleAI:ApiKey saknas i konfigurationen.");
            }
            
            _model = new GenerativeModel(
                model: "gemini-pro-latest", 
                apiKey: apiKey
            );
        }

        // --- DIN BEFINTLIGA 'RewriteDiaryEntryAsync'-METOD ---
        public async Task<string> RewriteDiaryEntryAsync(string originalText, CancellationToken ct)
        {
            var prompt = $"""
                          Du är en stöttande studiecoach och en skicklig redaktör.
                          Ditt uppdrag är att varsamt renskriva och förbättra ett dagboksinlägg från en student.

                          **Mål:**
                          Förbättra textens kvalitet utan att förlora studentens personliga röst eller de ursprungliga idéerna.

                          **Regler (Viktigt!):**
                          1.  **Korrigera:** Rätta all grammatik, stavfel och meningsbyggnad.
                          2.  **Förbättra:** Gör texten tydligare och mer lättläst. Förbättra meningsflödet.
                          3.  **Behåll tonen:** Texten ska fortfarande låta som en personlig dagboksanteckning, inte en formell uppsats.
                          4.  **ÄNDRA INTE INNEHÅLLET:** Du får absolut INTE lägga till ny information, nya fakta eller ta bort några av studentens kärnidéer eller reflektioner.

                          **Svarsformat:**
                          Svara ENDAST med den nya, renskrivna texten. Inkludera inga egna kommentarer, rubriker eller förklaringar.

                          **Originaltext:**
                          "{originalText}"

                          **Renskriven text:**
                          """;
            
            var response = await _model.GenerateContentAsync(prompt, cancellationToken: ct);
            return response.Text.Trim();
        }

        private string ExtractTextFromPdf(Stream pdfStream)
        {
            var allText = new StringBuilder();
            using (var document = PdfDocument.Open(pdfStream)) 
            {
                foreach (var page in document.GetPages())
                {
                    allText.Append(page.Text);
                    allText.Append("\n\n"); 
                }
            }
            return allText.ToString();
        }

        public async Task<List<AiGeneratedCardDto>> GenerateFlashcardsFromDocumentAsync(
            Stream fileStream, 
            string fileName, 
            CancellationToken cancellationToken)
        {
            string extractedText;

            // --- A. Extrahera text ---
            if (fileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                extractedText = ExtractTextFromPdf(fileStream);
            }
            else
            {
                throw new NotSupportedException("Endast .pdf stöds just nu.");
            }

            if (string.IsNullOrWhiteSpace(extractedText))
            {
                throw new InvalidOperationException("Filen verkar vara tom.");
            }
            
            var systemPrompt = """
            Du är en expert på pedagogik och studieteknik.
            Ditt mål är att hjälpa en student att skapa högkvalitativa flashcards (frågekort).

            Regler för bra flashcards:
            1.  **Atomära:** Varje kort ska testa EN sak.
            2.  **Tydliga:** Frågan ska vara otvetydig.
            3.  **Värdefulla:** Fokusera på nyckelkoncept, definitioner, orsaker/konsekvenser och viktiga fakta. Undvik triviala detaljer.

            Uppgift:
            Läs texten jag tillhandahåller. Generera upp till 15 av de bästa, mest pedagogiska 'fråga/svar'-paren från texten.

            Svarsformat:
            Svara ENDAST med en JSON-array i formatet: [{"frontText": "Din fråga här", "backText": "Ditt svar här"}]
            """;
            
            var userPrompt = $"Här är texten:\n\n{extractedText}";

            if (userPrompt.Length > 30000) 
            {
                userPrompt = userPrompt.Substring(0, 30000);
            }

           
            // 2. Skapa en lista av 'Part'-objekt
            var parts = new List<Part>
            {
                new Part { Text = systemPrompt },
                new Part { Text = userPrompt }
            };
            
            var response = await _model.GenerateContentAsync(
                parts, 
                cancellationToken: cancellationToken
            );

            
            var aiMessageContent = response.Text;

            if (string.IsNullOrEmpty(aiMessageContent))
            {
                throw new Exception("AI:n returnerade ett tomt svar.");
            }

            string jsonOnly;
            try
            {
                var startIndex = aiMessageContent.IndexOf('[');
                var endIndex = aiMessageContent.LastIndexOf(']');
                
                if(startIndex == -1 || endIndex == -1 || endIndex <= startIndex)
                {
                    throw new Exception("Kunde inte hitta en giltig JSON-array i AI:s svar.");
                }
                
                jsonOnly = aiMessageContent.Substring(startIndex, endIndex - startIndex + 1);
            }
            catch(Exception ex)
            {
                throw new Exception($"Kunde inte parsa AI-svar. Fel: {ex.Message}. Svar: {aiMessageContent}");
            }

            var generatedCards = JsonSerializer.Deserialize<List<AiGeneratedCardDto>>(
                jsonOnly, 
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );

            if (generatedCards == null)
            {
                 throw new Exception("AI:n returnerade ett ogiltigt JSON-format.");
            }

            return generatedCards;
           
        }
    }
}