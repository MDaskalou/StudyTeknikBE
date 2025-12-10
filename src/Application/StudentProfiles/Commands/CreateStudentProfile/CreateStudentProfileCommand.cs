using Application.Common.Results;
using MediatR;

namespace Application.StudentProfiles.Commands.CreateStudentProfile
{
    public record CreateStudentProfileCommand(
        Guid StudentId,
        int PlanningHorizonWeeks,
        TimeSpan WakeUpTime,
        TimeSpan BedTime) : IRequest<OperationResult<Guid>>; 
}