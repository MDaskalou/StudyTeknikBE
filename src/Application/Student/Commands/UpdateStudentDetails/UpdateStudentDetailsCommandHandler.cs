using Application.Common.Results;
using Application.Student.Dtos;
using Application.Student.Repository;
using Domain.Common;
using FluentValidation;
using MediatR;
using Domain.Models.Users; // <-- VIKTIGT: Importera din rika User-modell
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
            // STEG 1: Hämta den riktiga "patienten" (User-domänmodellen), inte fotografiet.
            var user = await _studentRepository.GetTrackedDomainUserByIdAsync(request.Id, ct);
            if (user is null)
            {
                return OperationResult.Failure(Error.NotFound(ErrorCodes.StudentError.NotFound, $"Student med ID {request.Id} kunde inte hittas."));
            }

            var detailsToPatch = new UpdateStudentDetailsDto
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                SecurityNumber = user.SecurityNumber
            };
            request.PatchDoc.ApplyTo(detailsToPatch);

            // Valideringslogiken är densamma.
            var validationResult = await _dtoValidator.ValidateAsync(detailsToPatch, ct);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                var error = Error.Validation(ErrorCodes.General.Validation, errorMessages);
                return OperationResult.Failure(error);
            }

            var newEmail = detailsToPatch.Email.Trim().ToLowerInvariant();
            if (user.Email != newEmail && await _studentRepository.EmailExistsAsync(newEmail, ct))
            {
                return OperationResult.Failure(Error.Conflict(ErrorCodes.StudentError.EmailAlreadyExists, "Den nya e-postadressen används redan."));
            }
            
            // STEG 2: KORREKT! Utför operationen på den riktiga patienten med dess egna, säkra metoder.
            user.SetName(detailsToPatch.FirstName.Trim(), detailsToPatch.LastName.Trim());
            user.SetEmail(newEmail);
            user.SetSecurityNumber(detailsToPatch.SecurityNumber.Trim());
            
            // STEG 3: Lämna tillbaka den uppdaterade patienten till arkivet (databasen).
            return await _studentRepository.UpdateAsync(user, ct);
        }
    }
}