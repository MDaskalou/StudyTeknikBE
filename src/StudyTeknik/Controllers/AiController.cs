using Application.Abstractions.IPersistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.AI.Dto;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AiController : ControllerBase
    {
        
        private readonly IAIService _aiService;
        
        public AiController(IAIService aiService)
        {
            _aiService = aiService;
        }

        [HttpPost("Rewrite")]
        [Authorize(Policy = "HasWriteScope")]

        public async Task<IActionResult> RewriteText([FromBody] RewriteRequestDto request,
            CancellationToken cancellationToken)
        {
            // Kollar om DTO:n är giltig (t.ex. om Text-fältet finns med)
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            // Anropar din service med texten från requesten
            var rewrittenText = await _aiService.RewriteDiaryEntryAsync(request.Text, cancellationToken);

            // Kollar om AI-tjänsten returnerade ett felmeddelande
            if (rewrittenText.StartsWith("Kunde inte generera text"))
            {
                // Returnerar ett 500-fel (Internal Server Error)
                return StatusCode(500, new { message = rewrittenText });
            }
        
            // Returnerar den nya texten med en 200 OK status
            return Ok(new { rewrittenText });
        }
        
    }
}