using Application.Abstractions.IPersistence;
using Application.Common.Results;
using Application.Student.Repository;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Student.Commands.UpdateStudent
{
    public class UpdateStudentCommandHandler : IRequestHandler<UpdateStudentCommand, OperationResult>
    {
        //Dependencies
        private readonly IStudentRepository _studentRepository;
        private readonly IAppDbContext _db;
        private readonly IValidator<UpdateStudentCommand> _validator;
        
        //Constructor
        public UpdateStudentCommandHandler(IStudentRepository studentRepository, IAppDbContext db, IValidator<UpdateStudentCommand> validator)
        {
            _studentRepository = studentRepository;
            _db = db;
            _validator = validator;
        }

        public async Task<OperationResult> Handle(UpdateStudentCommand request, CancellationToken cancellationToken)
        {
            // Validera inkommande data
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var error = Error.Validation("Validation.Error", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return OperationResult.Failure(error);
            }
            
            // FIX 1: Hämta den spårbara ENTITETEN (originalet), inte DTO:n (kopian)
            var student = await _studentRepository.GetTrackedByIdAsync(request.Id, cancellationToken);
            if (student is null)
            {
                var error = Error.NotFound("Student.NotFound", $"Student med ID {request.Id} kunde inte hittas.");
                return OperationResult.Failure(error);
            }
            
            // Kontrollera om e-posten ändrats och redan är upptagen
            var newEmail = request.Email.Trim().ToLowerInvariant();
            if (student.Email != newEmail && await _db.Users.AnyAsync(u => u.Email == newEmail, cancellationToken))
            {
                var error = Error.Conflict("Student.EmailAlreadyExists", "Den nya e-postadressen används redan av en annan användare.");
                return OperationResult.Failure(error);
            }
            
            // FIX 2: Använd "request" istället för "command" och uppdatera entitetens fält
            student.FirstName = request.FirstName.Trim();
            student.LastName = request.LastName.Trim();
            student.Email = newEmail;
            student.SecurityNumber = request.SecurityNumber.Trim();
            student.UpdatedAtUtc = DateTime.UtcNow;
            
            // Spara ändringarna till databasen
            await _db.SaveChangesAsync(cancellationToken);
            
            // Returnera ett lyckat resultat
            return OperationResult.Success();
        }
    }
}