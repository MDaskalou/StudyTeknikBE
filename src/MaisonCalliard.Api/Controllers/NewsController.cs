using MaisonCalliard.Application.News;
using MaisonCalliard.Application.News.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaisonCalliard.Api.Controllers;

[ApiController]
[Route("api/news")]
public sealed class NewsController : ControllerBase
{
    private readonly INewsService _newsService;

    public NewsController(INewsService newsService)
    {
        _newsService = newsService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll(CancellationToken cancellationToken)
    {
        var items = await _newsService.GetAllAsync(cancellationToken);
        return Ok(items);
    }

    [Authorize(Roles = "admin")]
    [HttpPost]
    public async Task<IActionResult> Create([FromForm] CreateNewsItemRequest request, IFormFile? image, CancellationToken cancellationToken)
    {
        Stream? stream = null;
        string? fileName = null;
        string? contentType = null;

        if (image is not null)
        {
            stream = image.OpenReadStream();
            fileName = image.FileName;
            contentType = image.ContentType;
        }

        var result = await _newsService.CreateAsync(request, stream, fileName, contentType, cancellationToken);
        return CreatedAtAction(nameof(GetAll), new { id = result.Id }, result);
    }

    [Authorize(Roles = "admin")]
    [HttpPut("{id:guid}")]
    public async Task<IActionResult> Update(Guid id, [FromForm] UpdateNewsItemRequest request, IFormFile? image, CancellationToken cancellationToken)
    {
        Stream? stream = null;
        string? fileName = null;
        string? contentType = null;

        if (image is not null)
        {
            stream = image.OpenReadStream();
            fileName = image.FileName;
            contentType = image.ContentType;
        }

        try
        {
            var result = await _newsService.UpdateAsync(id, request, stream, fileName, contentType, cancellationToken);
            return Ok(result);
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }

    [Authorize(Roles = "admin")]
    [HttpDelete("{id:guid}")]
    public async Task<IActionResult> Delete(Guid id, CancellationToken cancellationToken)
    {
        try
        {
            await _newsService.DeleteAsync(id, cancellationToken);
            return NoContent();
        }
        catch (KeyNotFoundException)
        {
            return NotFound();
        }
    }
}
