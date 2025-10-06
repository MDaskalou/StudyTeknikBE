using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Domain.Common;
using FluentValidation;
using MediatR;

namespace Application.Diary.Commands.DeleteDiary
{
    public sealed class DeleteDiaryCommandHandler : IRequestHandler<DeleteDiaryCommand, OperationResult>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly ICurrentUserService _currentUser;
        private readonly IValidator<DeleteDiaryCommand> _validator;
        
        public DeleteDiaryCommandHandler(IDiaryRepository diaryRepository, ICurrentUserService currentUser, IValidator<DeleteDiaryCommand> validator)
        {
            _diaryRepository = diaryRepository;
            _currentUser = currentUser;
            _validator = validator;
        }

        public async Task<OperationResult> Handle(DeleteDiaryCommand request, CancellationToken ct)
        {
            var studentId = _currentUser.UserId;
            if (studentId == null)
            {
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Validation, "Användaren är inte inloggad."));
            }
            
            var diaryToDelete = await _diaryRepository.GetTrackedByIdAsync(request.Id, ct);
            if (diaryToDelete is null)
            {
                return OperationResult.Success();
            }

            if (diaryToDelete.StudentId != studentId)
            {
                return OperationResult.Failure(Error.Validation("User.Forbidden", "Du har inte behörighet att radera detta inlägg."));
            }
            
            return await _diaryRepository.DeleteAsync(diaryToDelete, ct);

        }
    }
}