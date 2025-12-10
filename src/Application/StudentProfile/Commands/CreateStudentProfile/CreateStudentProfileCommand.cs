using Application.Common.Results;
using MediatR;

namespace Application.StudentProfile.Commands.CreateStudentProfile
{
    public record CreateStudentProfileCommand(
        Guid StudentId,
        int PlanningHorizonWeeks,
        TimeSpan WakeUpTime,
        TimeSpan BedTime) : IRequest<OperationResult<Guid>>; 
}