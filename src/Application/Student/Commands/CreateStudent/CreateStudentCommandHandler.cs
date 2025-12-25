﻿using Application.Common.Results;
using Application.Student.Commands.CreateStudent;
using Application.Student.Dtos;
using Application.Student.Repository;
using Domain.Models.Users; // <-- VIKTIGT: Importera din rika User-modell
using Domain.Common;
using FluentValidation;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public sealed class CreateStudentHandler
    : IRequestHandler<CreateStudentCommand, OperationResult<StudentCreatedDto>>
{
    private readonly IStudentRepository _studentRepository;
    private readonly IValidator<CreateStudentCommand> _validator;

    public CreateStudentHandler(IStudentRepository studentRepository, IValidator<CreateStudentCommand> validator)
    {
        _studentRepository = studentRepository;
        _validator = validator;
    }

    public async Task<OperationResult<StudentCreatedDto>> Handle(CreateStudentCommand command, CancellationToken ct)
    {
        var validationResult = await _validator.ValidateAsync(command, ct);
        if (!validationResult.IsValid)
        {
            var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
            var validationError = Error.Validation(ErrorCodes.General.Validation, errorMessages);
            return OperationResult<StudentCreatedDto>.Failure(validationError);
        }

        var email = command.Email.Trim().ToLowerInvariant();

        if (await _studentRepository.EmailExistsAsync(email, ct))
        {
            var error = Error.Conflict(ErrorCodes.StudentError.EmailAlreadyExists, "En student med den här eposten finns redan.");
            return OperationResult<StudentCreatedDto>.Failure(error);
        }

        if (!await _studentRepository.ClassExistsAsync(command.ClassId, ct))
        {
            var error = Error.NotFound("Class.Notfound", $"Klassen med id {command.ClassId} kunde inte hittas.");
            return OperationResult<StudentCreatedDto>.Failure(error);
        }

        var user = new User(
            command.FirstName.Trim(),
            command.LastName.Trim(),
            command.SecurityNumber,
            email,
            Role.Student,
            "manual", 
            "Student" 
        );
        
        var addResult = await _studentRepository.AddAsync(user, ct);
        if (addResult.IsFailure)
        {
            return OperationResult<StudentCreatedDto>.Failure(addResult.Error);
        }
        
        var dto = new StudentCreatedDto(user.Id, user.FirstName, user.LastName, user.Email, command.ClassId);
        return OperationResult<StudentCreatedDto>.Created(dto);
    }
}