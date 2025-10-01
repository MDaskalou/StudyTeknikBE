using Application.Common.Results;
using Application.Student.Repository;
using Application.Teacher.Repository;
using FluentValidation;
using MediatR;

namespace Application.Teacher.Commands.UpdateTeacher
{
    public class UpdateTeacherCommandHandler : IRequestHandler<UpdateTeacherCommand, OperationResult>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IValidator<UpdateTeacherCommand> _validator;

        public UpdateTeacherCommandHandler(ITeacherRepository teacherRepository,
            IValidator<UpdateTeacherCommand> validator)
        {
            _teacherRepository = teacherRepository;
            _validator = validator;
        }

        public async Task<OperationResult> Handle(UpdateTeacherCommand command, CancellationToken ct)
        {
            // FIX 1: Använd ValidateAsync
            var validationResult = await _validator.ValidateAsync(command, ct);
            if (!validationResult.IsValid)
            {
                var error = Error.Validation("Validation.Error", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return OperationResult.Failure(error);
            }

            // FIX 2: Hämta först med GetTrackedByIdAsync
            var userEntity = await _teacherRepository.GetTrackedByIdAsync(command.Id, ct);
            if (userEntity is null)
            {
                return OperationResult.Failure(Error.NotFound("Teacher.NotFound", $"Lärare med ID {command.Id} kunde inte hittas."));
            }

            var newEmail = command.Email.Trim().ToLowerInvariant();
            if (userEntity.Email != newEmail && await _teacherRepository.EmailExistsAsync(newEmail, ct))
            {
                return OperationResult.Failure(Error.Conflict("Teacher.EmailAlreadyExists", "Den nya e-postadressen används redan."));
            }

            userEntity.FirstName = command.FirstName.Trim();
            userEntity.LastName = command.LastName.Trim();
            userEntity.Email = newEmail;
            userEntity.SecurityNumber = command.SecurityNumber.Trim();
            userEntity.UpdatedAtUtc = DateTime.UtcNow;

            return await _teacherRepository.UpdateAsync(userEntity, ct);
        }
    }
}