using Application.Common.Results;
using Application.Courses.Commands.CreateCourse;
using Application.Courses.Commands.DeleteCourse;
using Application.Courses.Commands.UpdateCourse;
using Application.Courses.DTOs.Course;
using Application.Courses.Queries.GetCourseById;
using Application.Courses.Queries.GetCoursesForProfile;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace StudyTeknik.Controllers
{
    /// <summary>
    /// Course Management API
    /// Handles CRUD operations for courses within a student profile
    /// </summary>
    [ApiController]
    [Authorize]
    [Route("api/student-profiles/{studentProfileId:guid}/courses")]
    public class CourseController : ControllerBase
    {
        private readonly IMediator _mediator;

        public CourseController(IMediator mediator)
        {
            _mediator = mediator;
        }

        /// <summary>
        /// Create a new course for a student profile
        /// </summary>
        /// <response code="201">Course created successfully</response>
        /// <response code="400">Validation error or name already exists</response>
        /// <response code="404">Student profile not found</response>
        [HttpPost]
        public async Task<IActionResult> CreateCourse(
            [FromRoute] Guid studentProfileId,
            [FromBody] CreateCourseRequest request,
            CancellationToken ct)
        {
            var command = new CreateCourseCommand(
                studentProfileId,
                request.Name,
                request.Description,
                request.Difficulty
            );

            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess)
            {
                return CreatedAtAction(nameof(GetCourseById), 
                    new { studentProfileId, courseId = result.Value!.Id }, 
                    result.Value);
            }

            return MapErrorToResponse(result.Error);
        }

        /// <summary>
        /// Get all courses for a student profile
        /// </summary>
        /// <response code="200">List of courses</response>
        /// <response code="404">Student profile not found</response>
        [HttpGet]
        public async Task<IActionResult> GetCoursesForProfile(
            [FromRoute] Guid studentProfileId,
            CancellationToken ct)
        {
            var query = new GetCoursesForProfileQuery(studentProfileId);
            var result = await _mediator.Send(query, ct);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return MapErrorToResponse(result.Error);
        }

        /// <summary>
        /// Get a specific course by ID
        /// </summary>
        /// <response code="200">Course details</response>
        /// <response code="403">Course does not belong to this profile</response>
        /// <response code="404">Course or profile not found</response>
        [HttpGet("{courseId:guid}")]
        public async Task<IActionResult> GetCourseById(
            [FromRoute] Guid studentProfileId,
            [FromRoute] Guid courseId,
            CancellationToken ct)
        {
            var query = new GetCourseByIdQuery(courseId, studentProfileId);
            var result = await _mediator.Send(query, ct);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return MapErrorToResponse(result.Error);
        }

        /// <summary>
        /// Update an existing course
        /// </summary>
        /// <response code="200">Updated course</response>
        /// <response code="400">Validation error or name already exists</response>
        /// <response code="403">Course does not belong to this profile</response>
        /// <response code="404">Course or profile not found</response>
        [HttpPut("{courseId:guid}")]
        public async Task<IActionResult> UpdateCourse(
            [FromRoute] Guid studentProfileId,
            [FromRoute] Guid courseId,
            [FromBody] UpdateCourseRequest request,
            CancellationToken ct)
        {
            var command = new UpdateCourseCommand(
                courseId,
                studentProfileId,
                request.Name,
                request.Description,
                request.Difficulty
            );

            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess)
            {
                return Ok(result.Value);
            }

            return MapErrorToResponse(result.Error);
        }

        /// <summary>
        /// Delete a course
        /// </summary>
        /// <response code="204">Course deleted successfully</response>
        /// <response code="403">Course does not belong to this profile</response>
        /// <response code="404">Course or profile not found</response>
        [HttpDelete("{courseId:guid}")]
        public async Task<IActionResult> DeleteCourse(
            [FromRoute] Guid studentProfileId,
            [FromRoute] Guid courseId,
            CancellationToken ct)
        {
            var command = new DeleteCourseCommand(courseId, studentProfileId);
            var result = await _mediator.Send(command, ct);

            if (result.IsSuccess)
            {
                return NoContent();
            }

            return MapErrorToResponse(result.Error);
        }

        /// <summary>
        /// Map error results to appropriate HTTP responses
        /// </summary>
        private IActionResult MapErrorToResponse(Error error)
        {
            return error.Type switch
            {
                ErrorType.Validation => BadRequest(new { error = error.Description }),
                ErrorType.NotFound => NotFound(new { error = error.Description }),
                ErrorType.Conflict => Conflict(new { error = error.Description }),
                ErrorType.Forbidden => Forbid(),
                ErrorType.BadRequest => BadRequest(new { error = error.Description }),
                ErrorType.Unauthorized => Unauthorized(new { error = error.Description }),
                _ => StatusCode(500, new { error = "An unexpected error occurred" })
            };
        }
    }
}

