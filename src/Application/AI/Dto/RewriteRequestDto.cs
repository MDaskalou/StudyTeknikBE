using System.ComponentModel.DataAnnotations;

namespace Application.AI.Dto
{
    public class RewriteRequestDto
    {
        [Required] public string Text { get; set; } = string.Empty;
    }
}