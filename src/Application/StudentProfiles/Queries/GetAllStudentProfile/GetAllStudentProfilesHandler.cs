using Application.Common.Results;
using Application.StudentProfile.Queries.GetAllStudentProfile;
using Application.StudentProfiles.DTOs;
using Application.StudentProfiles.IRepository;
using MediatR; // Se till att denna pekar rätt
// ... andra usings ...

namespace Application.StudentProfiles.Queries.GetAllStudentProfile
{
    // Notera att vi bytt returtyp till OperationResult<List<StudentProfileDto>>
    public class GetAllStudentProfilesHandler 
        : IRequestHandler<GetAllStudentProfilesQuery, OperationResult<List<StudentProfileDto>>>
    {
        private readonly IStudentProfileRepository _repository;

        public GetAllStudentProfilesHandler(IStudentProfileRepository repository)
        {
            _repository = repository;
        }

        public async Task<OperationResult<List<StudentProfileDto>>> Handle(
            GetAllStudentProfilesQuery request, 
            CancellationToken cancellationToken)
        {
            var profiles = await _repository.GetAllAsync(cancellationToken);

            var dtos = profiles.Select(p => new StudentProfileDto(
                p.Id,
                p.StudentId,
                p.PlanningHorizonWeeks,
                p.WakeUpTime,
                p.BedTime
            )).ToList();

            // NU fungerar detta tack vare den nya raden vi lade till:
            return OperationResult.Success(dtos);
        }
    }
}