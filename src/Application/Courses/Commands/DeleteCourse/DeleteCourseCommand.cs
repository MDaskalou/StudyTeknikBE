using Application.Common.Results;
using MediatR;

namespace Application.Courses.Commands.DeleteCourse
{
    public sealed record DeleteCourseCommand(
        Guid CourseId,
        Guid StudentProfileId
    ) : IRequest<OperationResult>;
}

