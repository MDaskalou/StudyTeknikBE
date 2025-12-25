using Application.Common.Results;
using Application.StudentProfiles.IRepository;
using Domain.Common;
using MediatR;
using System.Security.Claims;

namespace Application.StudentProfiles.Commands.CreateStudentProfile
{
    public class CreateStudentProfileHandler :IRequestHandler<CreateStudentProfileCommand, OperationResult<Guid>>
    {
       private readonly IStudentProfileRepository _studentProfileRepository;
       
       public CreateStudentProfileHandler(IStudentProfileRepository studentProfileRepository)
       {
           _studentProfileRepository = studentProfileRepository;
       }

       public async Task<OperationResult<Guid>> Handle(CreateStudentProfileCommand request,
           CancellationToken cancellationToken)
       {
           var profileExists = await _studentProfileRepository.
               ExistsByUserIdAsync(request.StudentId, cancellationToken);

           if (profileExists)
           {
               return OperationResult<Guid>.Failure(new Error(
                   "StudentProfile.AlreadyExists", 
                   "Användaren har redan en studentprofil.",
                   ErrorType.Conflict));
           }
           
           var profile = new Domain.Models.StudentProfiles.StudentProfile(
               request.StudentId,
               request.PlanningHorizonWeeks,
               request.WakeUpTime,
               request.BedTime);
           
           await _studentProfileRepository.AddAsync(profile, cancellationToken);
           
           return OperationResult<Guid>.Success(profile.Id);
       }
    }
}