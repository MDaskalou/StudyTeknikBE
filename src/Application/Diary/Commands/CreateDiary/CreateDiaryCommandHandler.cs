using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Diary.Dtos;
using Domain.Common;
using Domain.Models.Diary;
using FluentValidation;
using MediatR;

namespace Application.Diary.Commands.CreateDiary
{
    public sealed class CreateDiaryCommandHandler
        : IRequestHandler<CreateDiaryCommand, OperationResult<CreateDiaryEntryDto>>
    {
        IDiaryRepository _diaryRepository;
        IValidator<CreateDiaryCommand> _validator;
        ICurrentUserService _currentUserService;


        public CreateDiaryCommandHandler(
            IDiaryRepository diaryRepository,
            IValidator<CreateDiaryCommand> validator,
            ICurrentUserService currentUserService)
        {
            _diaryRepository = diaryRepository;
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task<OperationResult<CreateDiaryEntryDto>> Handle(CreateDiaryCommand request,
            CancellationToken ct)
        {
            var studentId = _currentUserService.UserId;
            if (studentId == null)
            {
                return OperationResult<CreateDiaryEntryDto>.Failure(Error.Validation(ErrorCodes.General.Validation,
                    "Användaren är inte inloggad"));
                
            }
            
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(",",  validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult<CreateDiaryEntryDto>.Failure(Error.Validation
                    (ErrorCodes.General.Validation, errorMessages));
            }

            if (await _diaryRepository.EntryExistsForDateAsync(studentId.Value, request.EntryDate, ct))
            {
                return OperationResult<CreateDiaryEntryDto>.Failure(Error.Conflict
                    (ErrorCodes.DiaryError.DailyLimitExceeded, "Endast ett dagboksinlägg per dag är tillåtet"));
            }

            var diaryEntry = new DiaryEntry(
                studentId.Value,
                request.EntryDate,
                request.Text);
            
            var addResult = await _diaryRepository.AddAsync(diaryEntry, ct);
            if (addResult.IsFailure)
            {
                return OperationResult<CreateDiaryEntryDto>.Failure(addResult.Error);
            }
            
            var responseDto = new CreateDiaryEntryDto(
                diaryEntry.Id, 
                diaryEntry.StudentId,
                diaryEntry.EntryDate);
            
            return OperationResult<CreateDiaryEntryDto>.Success(responseDto);
        }
    }
}