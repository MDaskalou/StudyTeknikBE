using Application.Common.Results;
using Application.Student.Dtos;
using Application.Student.Repository;
using FluentValidation;
using MediatR;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Student.Queries.GetStudentById
{
    // FIX 1: Uppdatera returtypen här med ett frågetecken (?)
    public sealed class GetStudentByIdHandler : IRequestHandler<GetStudentByIdQuery, OperationResult<GetStudentByIdDto?>>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IValidator<GetStudentByIdQuery> _validator;

        public GetStudentByIdHandler(IStudentRepository studentRepository, IValidator<GetStudentByIdQuery> validator)
        {
            _studentRepository = studentRepository;
            _validator = validator;
        }

        public async Task<OperationResult<GetStudentByIdDto?>> Handle(GetStudentByIdQuery request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                var validationError = Error.Validation("Validation.Error", errorMessages);
                return OperationResult<GetStudentByIdDto?>.Failure(validationError);
            }

            var studentDto = await _studentRepository.GetByIdAsync(request.Id, cancellationToken);

            if (studentDto is null)
            {
                var error = Error.NotFound("Student.NotFound", $"Kunde inte hitta en student med ID: {request.Id}");
                return OperationResult<GetStudentByIdDto?>.Failure(error);
            }

            return OperationResult<GetStudentByIdDto?>.Success(studentDto);
        }
    }
}