using Domain.Abstractions.Enum;
using FluentValidation;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Linq;
using Application.Common.Results;
using Application.Teacher.Repository;
using Domain.Common;
using Domain.Entities;
using MediatR;

namespace Application.Teacher.Commands.DeleteTeacher
{
  

    public sealed class DeleteTeacherCommandHandler : IRequestHandler<DeleteTeacherCommand, OperationResult>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IValidator<DeleteTeacherCommand> _validator;

        public DeleteTeacherCommandHandler(ITeacherRepository teacherRepository, IValidator<DeleteTeacherCommand> validator)
        {
            _teacherRepository = teacherRepository;
            _validator = validator;
        }

        public async Task<OperationResult> Handle(DeleteTeacherCommand request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join("," ,  validationResult.Errors.Select(e => e.ErrorMessage).ToArray());
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Validation, errorMessage));
            }

            var userEntity = await _teacherRepository.GetTrackedByIdAsync(request.Id, ct);

            if (userEntity is null)
            {
                return OperationResult.Failure(Error.NotFound("Teacher.NotFound", $"Lärare med ID {request.Id} kunde inte hittas."));
            }

            // Anropa repositoryt för att utföra raderingen
            return await _teacherRepository.DeleteAsync(userEntity.Id, ct); // Skicka med hela objektet
        }
    }
}