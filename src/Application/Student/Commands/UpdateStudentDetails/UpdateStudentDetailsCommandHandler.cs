using Application.Abstractions.IPersistence;
using Application.Common.Results;
using Application.Student.Dtos; // <-- Lägg till denna using-rad
using Application.Student.Repository;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.JsonPatch;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Student.Commands.UpdateStudentDetails
{
    // Bytte namn på klassen för att matcha filen och din namngivning
    public sealed class UpdateStudentDetailsCommandHandler : IRequestHandler<UpdateStudentDetailsCommand, OperationResult>
    {
        // Bytte namn här för att vara konsekvent
        private readonly IStudentRepository _studentRepository;
        private readonly IAppDbContext _db;
        private readonly IValidator<UpdateStudentDetailsDto> _dtoValidator;
        
        // --- HÄR ÄR FIXEN ---
        public UpdateStudentDetailsCommandHandler(
            IStudentRepository studentRepository, 
            IAppDbContext db, 
            IValidator<UpdateStudentDetailsDto> dtoValidator) // FIX 1: Ber om rätt DTO-validator
        {
            _studentRepository = studentRepository;
            _db = db;
            _dtoValidator = dtoValidator; // FIX 2: Tilldelar rätt parameter till fältet
        }

        public async Task<OperationResult> Handle(UpdateStudentDetailsCommand request, CancellationToken ct)
        {
            var userEntity = await _studentRepository.GetTrackedByIdAsync(request.Id, ct);
            if (userEntity is null)
            {
                return OperationResult.Failure(Error.NotFound("Student.NotFound", $"Student med ID {request.Id} kunde inte hittas."));
            }

            // Nu kan vi ta bort "Dtos."-prefixet tack vare using-raden
            var studentToPatch = new UpdateStudentDetailsDto
            {
                FirstName = userEntity.FirstName,
                LastName = userEntity.LastName,
                Email = userEntity.Email,
                SecurityNumber = userEntity.SecurityNumber
            };
            
            try
            {
                request.PatchDoc.ApplyTo(studentToPatch);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(Error.Validation("Patch.InvalidOperation", ex.Message));
            }

            var validationResult = await _dtoValidator.ValidateAsync(studentToPatch, ct);
            if (!validationResult.IsValid)
            {
                var error = Error.Validation("Validation.Error", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return OperationResult.Failure(error);
            }
            
            var newEmail = studentToPatch.Email.Trim().ToLowerInvariant();
            if (userEntity.Email != newEmail && await _db.Users.AnyAsync(u => u.Email == newEmail, ct))
            {
                var error = Error.Conflict("Student.EmailAlreadyExists", "Den nya e-postadressen används redan.");
                return OperationResult.Failure(error);
            }
            
            userEntity.FirstName = studentToPatch.FirstName;
            userEntity.LastName = studentToPatch.LastName;
            userEntity.Email = newEmail;
            userEntity.SecurityNumber = studentToPatch.SecurityNumber;
            userEntity.UpdatedAtUtc = DateTime.UtcNow;

            await _db.SaveChangesAsync(ct);
            
            return OperationResult.Success();
        }
    }
}