using MaisonCalliard.Application.Settings;
using MaisonCalliard.Application.Settings.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace MaisonCalliard.Api.Controllers;

[ApiController]
[Route("api/settings")]
public sealed class SettingsController : ControllerBase
{
    private readonly ISettingsService _settingsService;

    public SettingsController(ISettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    [HttpGet("lead-time")]
    public async Task<IActionResult> GetLeadTime(CancellationToken cancellationToken)
    {
        var result = await _settingsService.GetLeadTimeAsync(cancellationToken);
        return Ok(result);
    }

    [Authorize(Roles = "admin")]
    [HttpPut("lead-time")]
    public async Task<IActionResult> UpdateLeadTime([FromBody] UpdateLeadTimeRequest request, CancellationToken cancellationToken)
    {
        var result = await _settingsService.UpdateLeadTimeAsync(request, cancellationToken);
        return Ok(result);
    }
}
