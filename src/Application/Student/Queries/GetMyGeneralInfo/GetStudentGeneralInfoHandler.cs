using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories; // För ICurrentUserService
using Application.Common.Results;
using Application.Student.Dtos;
using Application.Student.Repository; // Ditt StudentRepository
using MediatR;

namespace Application.Student.Queries.GetMyGeneralInfo
{
    public class GetMyGeneralInfoHandler : IRequestHandler<GetStudentGeneralInfoQuery, OperationResult<StudentGeneralDto>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly ICurrentUserService _currentUserService;

        public GetMyGeneralInfoHandler(IStudentRepository studentRepository, ICurrentUserService currentUserService)
        {
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<OperationResult<StudentGeneralDto>> Handle(GetStudentGeneralInfoQuery request, CancellationToken cancellationToken)
        {
            // FIX 1: Hantera Nullable Guid
            Guid? userIdNullable = _currentUserService.UserId;

            // Om userId är null ELLER Empty -> returnera fel
            if (!userIdNullable.HasValue || userIdNullable.Value == Guid.Empty)
            {
                // FIX 2: Använd den GENERISKA Failure-metoden
                // Vi måste säga "Detta är ett misslyckande för typen StudentGeneralDto"
                return OperationResult<StudentGeneralDto>.Failure(
                    Error.Unauthorized("User.Anonymous", "Ingen användare identifierad.")
                );
            }

            // Nu vet vi att det finns ett värde, så vi använder .Value
            Guid userId = userIdNullable.Value;

            var student = await _studentRepository.GetByIdAsync(userId, cancellationToken);

            if (student is null)
            {
                // FIX 2 igen: Generisk Failure
                return OperationResult<StudentGeneralDto>.Failure(
                    Error.NotFound("Student.NotFound", "Hittade ingen studentinformation.")
                );
            }

            var dto = new StudentGeneralDto(
                student.Id,
                student.Email,
                student.FirstName,
                student.LastName
            );

            // FIX 3: Använd generisk Success (eller OperationResult.Success(dto) om du har den helpern)
            return OperationResult<StudentGeneralDto>.Success(dto);
        }
    }
}