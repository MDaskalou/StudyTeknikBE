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
using Domain.Common;
using Domain.Models.Common;

namespace Application.Teacher.Commands.CreateTeacher
{
    public sealed class CreateTeacherCommandHandler 
        : IRequestHandler<CreateTeacherCommand, OperationResult<CreateTeacherDto>>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IValidator<CreateTeacherCommand> _validator;
        
        public CreateTeacherCommandHandler(
            ITeacherRepository teacherRepository,
            IValidator<CreateTeacherCommand> validator)
        {
            _teacherRepository = teacherRepository;
            _validator = validator;
        }

        public async Task<OperationResult<CreateTeacherDto>> Handle(CreateTeacherCommand command, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(command, ct);
            if (!validationResult.IsValid)
            {
                var error = Error.Validation(ErrorCodes.General.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage)));
                return OperationResult<CreateTeacherDto>.Failure(error);
            }

            var email = command.Email.Trim().ToLowerInvariant();
            if (await _teacherRepository.EmailExistsAsync(email, ct)) // Korrekt metodnamn
            {
                var error = Error.Conflict(ErrorCodes.TeacherError.EmailAlreadyExists, $"En användare med e-postadressen '{email}' finns redan.");
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
                Role = Role.Teacher,
                CreatedAtUtc = now,
                UpdatedAtUtc = now,
                ExternalProvider = "manual",
                ExternalSubject = ""
            };

            var addResult = await _teacherRepository.AddAsync(user, ct);
            if (addResult.IsFailure)
            {
                return OperationResult<CreateTeacherDto>.Failure(addResult.Error);
            }

            var responseDto = new CreateTeacherDto(
                user.Id,
                $"{user.FirstName} {user.LastName}", 
                user.Email);
                
            return OperationResult<CreateTeacherDto>.Created(responseDto);
        }
    }
}