using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Student.Repository;
using Domain.Common;
using FluentValidation;
using MediatR;

namespace Application.Diary.Commands.DeleteDiary
{
    public sealed class DeleteDiaryCommandHandler : IRequestHandler<DeleteDiaryCommand, OperationResult>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IValidator<DeleteDiaryCommand> _validator;

        public DeleteDiaryCommandHandler(
            IDiaryRepository diaryRepository, 
            IStudentRepository studentRepository, 
            IValidator<DeleteDiaryCommand> validator)
        {
            _diaryRepository = diaryRepository;
            _studentRepository = studentRepository;
            _validator = validator;
        }

        public async Task<OperationResult> Handle(DeleteDiaryCommand request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }

            var student = await _studentRepository.GetByExternalIdAsync(request.UserId, ct);
            if (student == null)
            {
                // KORRIGERING: Använder Error.Validation som du föreslog för att lösa kompileringsfelet.
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Forbidden, "Användaren har inte behörighet."));
            }

            var diaryEntryToDelete = await _diaryRepository.GetTrackedByIdAsync(request.DiaryEntityId, ct);
            if (diaryEntryToDelete == null)
            {
                return OperationResult.Failure(Error.NotFound(ErrorCodes.DiaryError.NotFound, "Dagboksinlägget kunde inte hittas."));
            }

            if (diaryEntryToDelete.StudentId != student.Id)
            {
                // KORRIGERING: Använder Error.Validation även här.
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Forbidden, "Användaren har inte behörighet att radera detta inlägg."));
            }

            return await _diaryRepository.DeleteAsync(diaryEntryToDelete, ct);
        }
    }
}

