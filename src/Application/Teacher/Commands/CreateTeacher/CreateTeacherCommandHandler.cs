using Application.Abstractions;
using Application.Common.Results;
using Application.Teacher.Commands.CreateTeacher;
using Application.Teacher.Dtos;
using Application.Teacher.Repository;
using Domain.Abstractions.Enum;
using Domain.Entities;
using FluentValidation;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Teacher.Commands.CreateTeacher
{
    // Din klassdeklaration var nästan rätt, vi behöver bara korrigera retur-DTO:n
    public sealed class CreateTeacherCommandHandler 
        : IRequestHandler<CreateTeacherCommand, OperationResult<CreateTeacherDto>>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IValidator<CreateTeacherCommand> _validator;
        
        // Notera: IAppDbContext behövs inte här i det nya mönstret
        public CreateTeacherCommandHandler(
            ITeacherRepository teacherRepository,
            IValidator<CreateTeacherCommand> validator)
        {
            _teacherRepository = teacherRepository;
            _validator = validator;
        }

        // Här är den enda, kompletta Handle-metoden
        public async Task<OperationResult<CreateTeacherDto>> Handle(CreateTeacherCommand command, CancellationToken ct)
        {
            // Validera inkommande kommando
            var validationResult = await _validator.ValidateAsync(command, ct);
            if (!validationResult.IsValid)
            {
                var error = Error.Validation("Validation.Error", string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return OperationResult<CreateTeacherDto>.Failure(error);
            }

            // Kontrollera om e-post existerar via repositoryt
            var email = command.Email.Trim().ToLowerInvariant();
            if (await _teacherRepository.EmailExistAsync(email, ct)) // Korrekt metodnamn
            {
                var error = Error.Conflict("Teacher.EmailAlreadyExists", $"En användare med e-postadressen '{email}' finns redan.");
                return OperationResult<CreateTeacherDto>.Failure(error);
            }


            var now = DateTime.UtcNow;

            // Skapa UserEntity-objektet
            var user = new UserEntity
            {
                Id = Guid.NewGuid(),
                FirstName = command.FirstName.Trim(),
                LastName = command.LastName.Trim(),
                Email = email,
                SecurityNumber = command.SecurityNumber.Trim(),
                Role = UserRole.Teacher,
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
                ExternalProvider = "manual",
                ExternalSubject = ""
            };

            // Anropa repositoryts AddAsync-metod för att spara till databasen
            var addResult = await _teacherRepository.AddAsync(user, ct);
            if (addResult.IsFailure)
            {
                // Om något gick fel i databasen, returnera felet
                return OperationResult<CreateTeacherDto>.Failure(addResult.Error);
            }

            // Skapa det korrekta svars-DTO:t (TeacherCreatedDto)
            var responseDto = new CreateTeacherDto(
                user.Id,
                $"{user.FirstName} {user.LastName}", 
                user.Email);
                
            return OperationResult<CreateTeacherDto>.Created(responseDto);
        }
    }
}