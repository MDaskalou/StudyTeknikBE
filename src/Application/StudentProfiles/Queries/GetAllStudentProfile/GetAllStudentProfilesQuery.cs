using Application.Common.Results;
using Application.StudentProfiles.DTOs;
using Domain.Common;
using MediatR;
// För Result<T>

namespace Application.StudentProfile.Queries.GetAllStudentProfile
{
    public record GetAllStudentProfilesQuery() : IRequest<OperationResult<List<StudentProfileDto>>>;
}