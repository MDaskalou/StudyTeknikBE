using Application.Common.Results;
using Application.Student.Dtos;
using Application.Teacher.Dtos;
using Application.Teacher.Repository;
using FluentValidation;
using MediatR;

namespace Application.Teacher.Commands.UpdateTeacherDetails
{
    public class UpdateTeacherDetailsCommandHandler : IRequestHandler<UpdateTeacherDetailsCommand, OperationResult>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IValidator<UpdateTeacherDetailsDto> _dtoValidator;

        // FIX 1: IAppDbContext är nu helt borttagen från konstruktorn
        public UpdateTeacherDetailsCommandHandler(
            ITeacherRepository teacherRepository, 
            IValidator<UpdateTeacherDetailsDto> dtoValidator)
        {
            _teacherRepository = teacherRepository;
            _dtoValidator = dtoValidator;
        }
        
        public async Task<OperationResult> Handle(UpdateTeacherDetailsCommand request, CancellationToken ct)
        {
            var userEntity = await _teacherRepository.GetTrackedByIdAsync(request.Id, ct);
            if (userEntity is null)
            {
                return OperationResult.Failure(Error.NotFound("Teacher.NotFound", $"Lärare med ID {request.Id} kunde inte hittas."));
            }

            var updateTeacherResponse = new UpdateTeacherDetailsDto()
            {
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                Email = userEntity.Email,
                SecurityNumber = userEntity.SecurityNumber
            };

            try
            {
                request.PatchDoc.ApplyTo(updateTeacherResponse);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(Error.Validation("Patch.InvalidOperation", ex.Message));
            }

            var validationResult = await _dtoValidator.ValidateAsync(updateTeacherResponse, ct);
            if (!validationResult.IsValid)
            {
                return OperationResult.Failure(Error.Validation("Validation.Error", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            var newEmail = updateTeacherResponse.Email.Trim().ToLowerInvariant();
            if (userEntity.Email != newEmail && await _teacherRepository.EmailExistsAsync(newEmail, ct))
            {
                return OperationResult.Failure(Error.Conflict("Teacher.EmailAlreadyExists", "Den nya e-postadressen används redan."));
            }

            userEntity.FirstName = updateTeacherResponse.FirstName;
            userEntity.LastName = updateTeacherResponse.LastName;
            userEntity.Email = newEmail;
            userEntity.SecurityNumber = updateTeacherResponse.SecurityNumber;
            userEntity.UpdatedAtUtc = DateTime.UtcNow;

            // Använd repositoryt för att spara, enligt det nya mönstret
            return await _teacherRepository.UpdateAsync(userEntity, ct);
        }
    }
}