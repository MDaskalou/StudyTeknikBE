using Application.StudySessions.Commands.CreateStudySession;
using Application.StudySessions.Commands.StartStudySession;
using Application.StudySessions.Commands.CompleteStudySessionStep;
using Application.StudySessions.Commands.EndStudySession;
using Application.StudySessions.Queries.GetStudySessionById;
using Application.StudySessions.DTOs;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudyTeknik.Controllers
{
    [ApiController]
    [Authorize]
    [Route("api/study-sessions")]
    public class StudySessionController : ControllerBase
    {
        private readonly IMediator _mediator;

        public StudySessionController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new study session with steps
        /// </summary>
        [HttpPost]
        [ProducesResponseType(typeof(StudySessionDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        public async Task<IActionResult> CreateStudySession(
            [FromBody] CreateStudySessionRequest request,
            CancellationToken ct)
        {
            var command = new CreateStudySessionCommand(
                request.CourseId,
                request.SessionGoal,
                request.PlannedMinutes,
                request.EnergyStart,
                request.Steps
            );

            var result = await _mediator.Send(command, ct);
            return result.IsSuccess
                ? CreatedAtAction(nameof(CreateStudySession), new { id = result.Value.Id }, result.Value)
                : BadRequest(result.Error);
        }

        /// <summary>
        /// Get study session by ID
        /// </summary>
        [HttpGet("{sessionId:guid}")]
        [ProducesResponseType(typeof(StudySessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetStudySession(Guid sessionId, CancellationToken ct)
        {
            var query = new GetStudySessionByIdQuery(sessionId);
            var result = await _mediator.Send(query, ct);
            return result.IsSuccess ? Ok(result.Value) : NotFound(result.Error);
        }

        /// <summary>
        /// Start a study session (transition from Planned to InProgress)
        /// </summary>
        [HttpPatch("{sessionId:guid}/start")]
        [ProducesResponseType(typeof(StudySessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> StartStudySession(Guid sessionId, CancellationToken ct)
        {
            var command = new StartStudySessionCommand(sessionId);
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                return result.Error.Code switch
                {
                    "StudySession.NotFound" => NotFound(result.Error),
                    "StudySession.InvalidStatus" => Conflict(result.Error),
                    _ => BadRequest(result.Error)
                };
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// Mark a specific step as completed
        /// </summary>
        [HttpPatch("{sessionId:guid}/steps/{stepId:guid}/complete")]
        [ProducesResponseType(typeof(StudySessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> CompleteStudySessionStep(
            Guid sessionId,
            Guid stepId,
            CancellationToken ct)
        {
            var command = new CompleteStudySessionStepCommand(sessionId, stepId);
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                return result.Error.Code switch
                {
                    "StudySession.NotFound" => NotFound(new { message = "Session not found" }),
                    "StudySessionStep.NotFound" => NotFound(new { message = "Step not found in this session" }),
                    _ => BadRequest(result.Error)
                };
            }

            return Ok(result.Value);
        }

        /// <summary>
        /// End a study session (transition from InProgress to Completed)
        /// </summary>
        [HttpPatch("{sessionId:guid}/end")]
        [ProducesResponseType(typeof(StudySessionDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> EndStudySession(
            Guid sessionId,
            [FromBody] EndStudySessionRequest request,
            CancellationToken ct)
        {
            var command = new EndStudySessionCommand(sessionId, request.EnergyLevel);
            var result = await _mediator.Send(command, ct);

            if (!result.IsSuccess)
            {
                return result.Error.Code switch
                {
                    "StudySession.NotFound" => NotFound(result.Error),
                    "StudySession.InvalidStatus" => Conflict(result.Error),
                    "StudySession.InvalidEnergyLevel" => BadRequest(result.Error),
                    _ => BadRequest(result.Error)
                };
            }

            return Ok(result.Value);
        }
    }
}

