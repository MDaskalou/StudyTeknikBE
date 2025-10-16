using Application.Common.Results;
using Application.Student.Commands.UpdateStudent;
using Application.Student.Repository;
using Domain.Common;
using FluentValidation;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Student.Dtos;
using Domain.Models.Common;
using Domain.Models.Users;

namespace Application.Student.Commands.UpdateStudent
{
    public sealed class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, OperationResult>
    {
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
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                var validationError = Error.Validation(ErrorCodes.General.Validation, errorMessages);
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage).ToArray())));
            }

            var userEntity = await _studentRepository.GetTrackedByIdAsync(command.Id, ct);
            if (userEntity is null)
            {
                return OperationResult.Failure(Error.NotFound(ErrorCodes.StudentError.NotFound, $"Student med ID {command.Id} kunde inte hittas."));
            }

            var newEmail = command.Email.Trim().ToLowerInvariant();
            if (userEntity.Email != newEmail && await _studentRepository.EmailExistsAsync(newEmail, ct))
            {
                return OperationResult.Failure(Error.Conflict(ErrorCodes.StudentError.EmailAlreadyExists, "Den nya e-postadressen används redan."));
            }
            
            var user = new User(
                command.FirstName.Trim(),
                command.LastName.Trim(),
                command.SecurityNumber,
                command.Email.Trim(),
                Role.Student,
                "manual", // Anger att denna användare skapades manuellt i systemet
                "Student" // Ett platshållar-ID
            );
            
            var updateRequest = new StudentDetailsDto(user.Id, user.FirstName, user.LastName, user.Email );
            return OperationResult<StudentDetailsDto>.Success(updateRequest);

        }
    }
}