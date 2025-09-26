using Application.Abstractions.IPersistence;
using Application.Common.Results;
using Application.Student.Commands.CreateStudent;
using Application.Student.Dtos;
using Application.Student.Repository;
using Domain.Entities;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq; 
using System.Threading;
using System.Threading.Tasks;

public sealed class CreateStudentHandler
    : IRequestHandler<CreateStudentCommand, OperationResult<StudentCreatedDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IAppDbContext _db;
    private readonly IValidator<CreateStudentCommand> _validator;

    public CreateStudentHandler(IStudentRepository studentRepository, IAppDbContext db, IValidator<CreateStudentCommand> validator)
    {
        _studentRepository = studentRepository;
        _db = db;
        _validator = validator;
    }

    public async Task<OperationResult<StudentCreatedDto>> Handle(CreateStudentCommand command, CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
            var validationError = Error.Validation("Validation.Error", errorMessages);
            return OperationResult<StudentCreatedDto>.Failure(validationError);
        }

        var email = command.Email.Trim().ToLowerInvariant();

        if (await _db.Users.AnyAsync(u => u.Email == email, ct))
        {
            var error = Error.Conflict("Student.EmailAlreadyExists", "En student med denna e-postadress finns redan.");
            return OperationResult<StudentCreatedDto>.Failure(error);
        }

        var klass = await _db.Classes.AsNoTracking().FirstOrDefaultAsync(x => x.Id == command.ClassId, ct);
        if (klass is null)
        {
            var error = Error.NotFound("Class.NotFound", $"Klassen med ID {command.ClassId} kunde inte hittas.");
            return OperationResult<StudentCreatedDto>.Failure(error);
        }

        var now = DateTime.UtcNow;
        var user = new UserEntity
        {
            Id = Guid.NewGuid(),
            CreatedAtUtc = now, UpdatedAtUtc = now,
            FirstName = command.FirstName.Trim(), 
            LastName = command.LastName.Trim(),
            Email = email, 
            Role = Domain.Abstractions.Enum.UserRole.Student,
            SecurityNumber = command.SecurityNumber,
            ExternalProvider = "manual", 
            ExternalSubject = ""
        };
        
        _studentRepository.Add(user, command.ClassId);

        await _db.SaveChangesAsync(ct);
        
        var dto = new StudentCreatedDto(user.Id, user.FirstName, user.LastName, user.Email, command.ClassId);
        return OperationResult<StudentCreatedDto>.Created(dto);    }
}