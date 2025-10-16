using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Diary.Commands.DeleteDiary;
using Application.Student.Repository;
using Domain.Common;
using Domain.Models.Diary;
using FluentValidation;
using MediatR;

namespace Application.Diary.Commands.UpdateDiary;

public sealed class UpdateDiaryCommandHandler : IRequestHandler<UpdateDiaryCommand, OperationResult>
{
    private readonly IDiaryRepository _diaryRepository;
    private readonly IValidator<UpdateDiaryCommand> _validator;
    private readonly IStudentRepository _studentRepository;


    public UpdateDiaryCommandHandler(IDiaryRepository diaryRepository, IValidator<UpdateDiaryCommand> validator, IStudentRepository studentRepository)
    {
        _diaryRepository = diaryRepository;
        _validator = validator;
        _studentRepository = studentRepository;
    }

    public async Task<OperationResult> Handle(UpdateDiaryCommand request, CancellationToken ct)
    {
        var diaryEntity = await _diaryRepository.GetTrackedByIdAsync(request.Id, ct);

        if (diaryEntity == null)
        {
            return OperationResult.Failure(Error.NotFound("Diary.NotFound","Dagboken hittades inte"));
        }

        var student = await _studentRepository.GetByExternalIdAsync(request.UserId, ct);
        if (student == null)
        {
            return OperationResult.Failure(Error.Validation(ErrorCodes.General.Forbidden,"Användaren hittades inte "));
        }

        if (diaryEntity.StudentId != student.Id)
        {
            return OperationResult.Failure(Error.Validation(ErrorCodes.General.Forbidden, "Användaren har inte behörighet att uppdatera inlägget"));
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