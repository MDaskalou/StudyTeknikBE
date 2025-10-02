using Application.Common.Results;
using Application.Student.Commands.UpdateStudentDetails;
using Application.Student.Dtos;
using Application.Student.Repository;
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
                return OperationResult.Failure(Error.NotFound("Student.NotFound", $"Student med ID {request.Id} kunde inte hittas."));
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
                return OperationResult.Failure(Error.Validation("Patch.InvalidOperation", ex.Message));
            }

            var validationResult = await _dtoValidator.ValidateAsync(detailsToPatch, ct);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                var error = Error.Validation("Validation.Error", errorMessages);
                return OperationResult.Failure(error);
            }

            var newEmail = detailsToPatch.Email.Trim().ToLowerInvariant();
            if (userEntity.Email != newEmail && await _studentRepository.EmailExistsAsync(newEmail, ct))
            {
                return OperationResult.Failure(Error.Conflict("Student.EmailAlreadyExists", "Den nya e-postadressen används redan."));
            }
            
            userEntity.FirstName = detailsToPatch.FirstName;
            userEntity.LastName = detailsToPatch.LastName;
            userEntity.Email = newEmail;
            userEntity.SecurityNumber = detailsToPatch.SecurityNumber;
            userEntity.UpdatedAtUtc = DateTime.UtcNow;

            // FIX 3: Anropa repositoryts UpdateAsync-metod som hanterar SaveChanges
            return await _studentRepository.UpdateAsync(userEntity, ct);
        }
    }
}