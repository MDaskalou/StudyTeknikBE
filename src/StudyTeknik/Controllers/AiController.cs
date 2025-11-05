using Application.Abstractions.IPersistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.AI.Commands;
using Microsoft.AspNetCore.Mvc; 
using Application.AI.Commands.GenerateFlashcardsFromDocument;
using Application.AI.Commands.Rewrite;
using Application.AI.Dtos;
using Application.Common.Results;
using MediatR;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/ai")]
    [Authorize]
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
                };            }
        
            return Ok(result.Value); 
        }
        
        [ApiExplorerSettings(IgnoreApi = true)]
        [HttpPost("generate-cards-from-file")]
        
        public async Task<IActionResult> GenerateCardsFromFile(
            [FromForm] Guid deckId, // 'deckId' måste matcha namnet i Next.js FormData
            [FromForm] IFormFile file, // 'file' måste matcha namnet i Next.js FormData
            CancellationToken ct)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(Error.Validation("Fil", "Ingen fil laddades upp."));
            }

            // Läs filen in i en minnesström
            await using var memoryStream = new MemoryStream();
            await file.CopyToAsync(memoryStream, ct);
            memoryStream.Position = 0; // Nollställ strömmen så att den kan läsas från början

            // Skapa det nya command-objektet
            var command = new GenerateFlashcardsFromDocumentCommand(
                deckId, 
                memoryStream,
                file.FileName
            );

            // Skicka till din nya handler
            var result = await _mediator.Send(command, ct);

            // Returnera svaret (antingen List<AiGeneratedCardDto> eller ett fel)
            if (result.IsFailure)
            {
                return result.Error.Type switch
                {
                    ErrorType.Validation => BadRequest(result.Error), 
                    ErrorType.NotFound => NotFound(result.Error),   
                    ErrorType.Forbidden => Forbid(), // Lade till Forbidden
                    _ => StatusCode(500, result.Error)          
                }; 
            }
        
            return Ok(result.Value); // Returnerar List<AiGeneratedCardDto>
        }
    
        
    }
}