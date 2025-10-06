using Application.Common.Results;
using Application.Student.Dtos;
using Application.Student.Repository;
using Domain.Common; // <-- Lägg till denna using
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Student.Commands.UpdateStudentDetails
{
    public sealed class UpdateStudentDetailsCommandHandler : IRequestHandler<UpdateStudentDetailsCommand, OperationResult>
    {
        private readonly IStudentRepository _studentRepository;
        private readonly IValidator<UpdateStudentDetailsDto> _dtoValidator;

        public UpdateStudentDetailsCommandHandler(
            IStudentRepository studentRepository,
            IValidator<UpdateStudentDetailsDto> dtoValidator)
        {
            _studentRepository = studentRepository;
            _dtoValidator = dtoValidator;
        }

        public async Task<OperationResult> Handle(UpdateStudentDetailsCommand request, CancellationToken ct)
        {
            var userEntity = await _studentRepository.GetTrackedByIdAsync(request.Id, ct);
            if (userEntity is null)
            {
                // Använder ErrorCodes
                return OperationResult.Failure(Error.NotFound(ErrorCodes.StudentError.NotFound, $"Student med ID {request.Id} kunde inte hittas."));
            }

            var detailsToPatch = new UpdateStudentDetailsDto
            {
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                Email = userEntity.Email,
                SecurityNumber = userEntity.SecurityNumber
            };

            try
            {
                request.PatchDoc.ApplyTo(detailsToPatch);
            }
            catch (Exception ex)
            {
                // Använder ErrorCodes
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Validation, ex.Message));
            }

            var validationResult = await _dtoValidator.ValidateAsync(detailsToPatch, ct);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                // Använder ErrorCodes
                var error = Error.Validation(ErrorCodes.General.Validation, errorMessages);
                return OperationResult.Failure(error);
            }

            var newEmail = detailsToPatch.Email.Trim().ToLowerInvariant();
            if (userEntity.Email != newEmail && await _studentRepository.EmailExistsAsync(newEmail, ct))
            {
                // Använder ErrorCodes
                return OperationResult.Failure(Error.Conflict(ErrorCodes.StudentError.EmailAlreadyExists, "Den nya e-postadressen används redan."));
            }

            userEntity.FirstName = detailsToPatch.FirstName;
            userEntity.LastName = detailsToPatch.LastName;
            userEntity.Email = newEmail;
            userEntity.SecurityNumber = detailsToPatch.SecurityNumber;
            userEntity.UpdatedAtUtc = DateTime.UtcNow;

            return await _studentRepository.UpdateAsync(userEntity, ct);
        }
    }
}