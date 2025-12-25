using Application.Abstractions.IPersistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.AI.Commands;
using Application.AI.Commands.GenerateFlashCards;
using Application.AI.Dtos;
using Application.Common.Results;
using MediatR;
using Infrastructure.Service;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        
        private readonly ISender _mediator;
        
        public AiController(ISender mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("Rewrite")]
        public async Task<IActionResult> RewriteText([FromBody] RewriteRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new RewriteTextCommand(request.Text);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.Validation => BadRequest(result.Error), 
                    ErrorType.NotFound => NotFound(result.Error),   
                    ErrorType.Conflict => Conflict(result.Error),  
                    _ => StatusCode(500, result.Error)          
                };            
            }
        
            return Ok(result.Value); 
        }

        [HttpPost("generate-cards")]
        public async Task<IActionResult> GenerateFlashCards(
            [FromBody] GenerateFlashCardsFromTextRequestDto request,
            CancellationToken cancellationToken)
        {
            var command = new GenerateFlashCardsCommand(request.PdfContent, request.DeckId);

            var result = await _mediator.Send(command, cancellationToken);

            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.Validation => BadRequest(result.Error),
                    ErrorType.NotFound => NotFound(result.Error),
                    ErrorType.Forbidden => Forbid(),
                    ErrorType.Conflict => Conflict(result.Error),
                    _ => StatusCode(500, result.Error)
                };
            }

            return Ok(result.Value);
        }

        [HttpPost("generate-cards-from-file")]
        public async Task<IActionResult> GenerateFlashCardsFromFile(
            IFormFile file,
            [FromForm] Guid deckId,
            CancellationToken cancellationToken)
        {
            // Validera fil
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { error = "Ingen fil uppladdad" });
            }

            if (!file.FileName.EndsWith(".pdf", StringComparison.OrdinalIgnoreCase))
            {
                return BadRequest(new { error = "Endast PDF-filer tillåtna" });
            }

            if (file.Length > 10 * 1024 * 1024) // 10 MB limit
            {
                return BadRequest(new { error = "Filen är för stor (max 10 MB)" });
            }

            try
            {
                // Läs PDF-filen
                byte[] fileBytes;
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream, cancellationToken);
                    fileBytes = memoryStream.ToArray();
                }

                // Extrahera text från PDF
                var pdfContent = PdfExtractor.ExtractTextFromPdf(fileBytes);

                if (string.IsNullOrWhiteSpace(pdfContent))
                {
                    return BadRequest(new { error = "Kunde inte extrahera text från PDF" });
                }

                // Begränsa innehållets storlek för AI
                if (pdfContent.Length > 50000)
                {
                    pdfContent = pdfContent.Substring(0, 50000);
                }

                // Skapa och skicka kommandot
                var command = new GenerateFlashCardsCommand(pdfContent, deckId);
                var result = await _mediator.Send(command, cancellationToken);

                if (result.IsFailure)
                {
                    return result.Error.Type switch
                    {
                        ErrorType.Validation => BadRequest(result.Error),
                        ErrorType.NotFound => NotFound(result.Error),
                        ErrorType.Forbidden => Forbid(),
                        ErrorType.Conflict => Conflict(result.Error),
                        _ => StatusCode(500, result.Error)
                    };
                }

                return Ok(result.Value);
            }
            catch (InvalidOperationException ex)
            {
                Console.WriteLine($"❌ PDF Extraction Error: {ex.Message}");
                return StatusCode(500, new { error = $"PDF-fel: {ex.Message}" });
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ Unexpected Error in GenerateFlashCardsFromFile: {ex.GetType().Name}");
                Console.WriteLine($"   Message: {ex.Message}");
                if (ex.InnerException != null)
                {
                    Console.WriteLine($"   Inner Exception: {ex.InnerException.GetType().Name}");
                    Console.WriteLine($"   Inner Message: {ex.InnerException.Message}");
                }
                Console.WriteLine($"   StackTrace: {ex.StackTrace}");
                
                return StatusCode(500, new { error = $"Fel vid flashcard-generering: {ex.GetType().Name}: {ex.Message}" });
            }
        }
    }
}

