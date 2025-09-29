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
        private readonly IAppDbContext _db;
        private IValidator<DeleteStudentCommand> _validator;

        public DeleteUserCommandHandler(IStudentRepository repository, IAppDbContext db,
            IValidator<DeleteStudentCommand> validator)
        {
            _repository = repository;
            _db = db;
            _validator = validator;
        }

        public async Task<OperationResult> Handle(DeleteStudentCommand request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var error = Error.Validation("Validation.Error", "Student-Id får inte vara tomt");
                return OperationResult.Failure(error);

            }

            var userEntity = await _repository.GetTrackedByIdAsync(request.Id, cancellationToken);
            if (userEntity is null)
            {
                return OperationResult.Success();
            }

            var enrollments = await _db.Enrollments
                .Where(e => e.StudentId == request.Id)
                .ToListAsync(cancellationToken);

            if (enrollments.Any())
            {
                _db.Enrollments.RemoveRange(enrollments);
            }
            // TODO: I en riktig applikation skulle du även behöva hantera 
            // och radera annan relaterad data här, t.ex. studentens dagböcker.
            
            _db.Users.Remove(userEntity);
            await _db.SaveChangesAsync(cancellationToken);
            return OperationResult.Success();
        }
    }
}