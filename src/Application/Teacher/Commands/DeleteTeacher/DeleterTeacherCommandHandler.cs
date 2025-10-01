using Domain.Abstractions.Enum;
using FluentValidation;

namespace Application.Teacher.Commands.DeleteTeacher
{
    using System;
    using System.Threading;
    using System.Threading.Tasks;
    using Application.Common.Results;
    using Application.Teacher.Repository;
    using Domain.Entities;
    using MediatR;

    public sealed class DeleterTeacherCommandHandler : IRequestHandler<DeleteTeacherCommand, OperationResult>
    {
        private readonly ITeacherRepository _teacherRepository;
        private readonly IValidator<DeleteTeacherCommand> _validator;

        public DeleterTeacherCommandHandler(ITeacherRepository teacherRepository, IValidator<DeleteTeacherCommand> validator)
        {
            _teacherRepository = teacherRepository;
            _validator = validator;
        }

        public async Task<OperationResult> Handle(DeleteTeacherCommand request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var error = Error.Validation("Validation.Error", "Lärar-ID får inte vara tomt.");
                return OperationResult.Failure(error);
            }

            // Hämta läraren för att se om den existerar
            var userEntity = await _teacherRepository.GetTrackedByIdAsync(request.Id, ct);

            // Förenklad kontroll: Om vi inte får tillbaka något, finns inte läraren.
            if (userEntity is null)
            {
                return OperationResult.Failure(Error.NotFound("Teacher.NotFound", $"Lärare med ID {request.Id} kunde inte hittas."));
            }

            // Anropa repositoryt för att utföra raderingen
            return await _teacherRepository.DeleteAsync(userEntity.Id, ct); // Skicka med hela objektet
        }
    }
}