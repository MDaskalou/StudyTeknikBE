using Application.Abstractions.IPersistence;
using Application.Common.Results;
using Application.Student.Repository;
using FluentValidation;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace Application.Student.Commands.DeleteStudent
{
    public class DeleteUserCommandHandler : IRequestHandler<DeleteStudentCommand, OperationResult>
    {
        private readonly IStudentRepository _repository;
        private IValidator<DeleteStudentCommand> _validator;

        public DeleteUserCommandHandler(IStudentRepository repository,
            IValidator<DeleteStudentCommand> validator)
        {
            _repository = repository;
            _validator = validator;
        }

        public async Task<OperationResult> Handle(DeleteStudentCommand request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var error = Error.Validation("Validation.Error", "Student-ID får inte vara tomt.");
                return OperationResult.Failure(error);
            }

            // FIX 2: Anropa bara den nya DeleteAsync-metoden i repositoryt
            return await _repository.DeleteAsync(request.Id, ct);
        }
    }
}