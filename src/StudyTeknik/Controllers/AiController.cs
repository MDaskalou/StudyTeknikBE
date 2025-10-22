using Application.Abstractions.IPersistence;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Application.AI.Commands;
using Application.AI.Dtos;
using Application.Common.Results;
using MediatR;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/[controller]")]
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
    
        
    }
}