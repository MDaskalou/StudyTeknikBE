using Application.Common.Results;
using Application.Student.Commands.UpdateStudent;
using Application.Student.Repository;
using FluentValidation;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Student.Commands.UpdateStudent
{
    public sealed class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, OperationResult>
    {
        // FIX 1: Beroendet till IAppDbContext är nu borta
        private readonly IStudentRepository _studentRepository;
        private readonly IValidator<UpdateStudentCommand> _validator;

        public UpdateStudentCommandHandler(IStudentRepository studentRepository, IValidator<UpdateStudentCommand> validator)
        {
            _studentRepository = studentRepository;
            _validator = validator;
        }

        public async Task<OperationResult> Handle(UpdateStudentCommand command, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(command, ct);
            if (!validationResult.IsValid)
            {
                var error = Error.Validation("Validation.Error", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage).ToArray()));
                return OperationResult.Failure(error);
            }

            var userEntity = await _studentRepository.GetTrackedByIdAsync(command.Id, ct);
            if (userEntity is null)
            {
                return OperationResult.Failure(Error.NotFound("Student.NotFound", $"Student med ID {command.Id} kunde inte hittas."));
            }

            var newEmail = command.Email.Trim().ToLowerInvariant();
            // FIX 2: Använder repositoryt för att kolla om e-post finns
            if (userEntity.Email != newEmail && await _studentRepository.EmailExistsAsync(newEmail, ct))
            {
                return OperationResult.Failure(Error.Conflict("Student.EmailAlreadyExists", "Den nya e-postadressen används redan."));
            }

            // Uppdatera entiteten i minnet
            userEntity.FirstName = command.FirstName.Trim();
            userEntity.LastName = command.LastName.Trim();
            userEntity.Email = newEmail;
            userEntity.SecurityNumber = command.SecurityNumber.Trim();
            userEntity.UpdatedAtUtc = DateTime.UtcNow;

            // FIX 3: Anropa repositoryts UpdateAsync-metod som hanterar SaveChanges
            return await _studentRepository.UpdateAsync(userEntity, ct);
        }
    }
}