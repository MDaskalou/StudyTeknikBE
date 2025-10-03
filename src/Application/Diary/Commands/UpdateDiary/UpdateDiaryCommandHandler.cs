using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Domain.Models.Diary;
using FluentValidation;
using MediatR;

namespace Application.Diary.Commands.UpdateDiary;

public sealed class UpdateDiaryCommandHandler : IRequestHandler<UpdateDiaryCommand, OperationResult>
{
    private readonly IDiaryRepository _diaryRepository;
    private readonly IValidator<UpdateDiaryCommand> _validator;
    private readonly ICurrentUserService _currentUser; 


    public UpdateDiaryCommandHandler(IDiaryRepository diaryRepository, IValidator<UpdateDiaryCommand> validator, ICurrentUserService currentUser)
    {
        _diaryRepository = diaryRepository;
        _validator = validator;
        _currentUser = currentUser;
    }

    public async Task<OperationResult> Handle(UpdateDiaryCommand request, CancellationToken ct)
    {
        var diaryEntity = await _diaryRepository.GetTrackedByIdAsync(request.Id, ct);

        if (diaryEntity == null)
        {
            return OperationResult.Failure(Error.NotFound("Diary.NotFound","Dagboken hittades inte"));
        }

        var studentId = _currentUser.UserId;

        if (diaryEntity.StudentId != studentId)
        {
            return OperationResult.Failure(Error.Validation("User.Forbidden","Dagboken hittades inte"));
        }

        var domainEntry = DiaryEntry.Rehydrate(
            diaryEntity.Id, diaryEntity.CreatedAtUtc, diaryEntity.UpdatedAtUtc,
            diaryEntity.StudentId, diaryEntity.EntryDate, diaryEntity.Text);
        
        domainEntry.UpdateText(request.Text);
        
        diaryEntity.Text = domainEntry.Text;
        diaryEntity.UpdatedAtUtc = domainEntry.UpdatedAtUtc;
        return await _diaryRepository.UpdateAsync(diaryEntity, ct);



    }
}