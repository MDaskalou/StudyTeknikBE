using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Diary.Dtos;
using Domain.Common;
using FluentValidation;
using MediatR;

namespace Application.Diary.Queries.GetDiaryById
{
    public class GetDiaryByIdHandler : IRequestHandler<GetDiaryByIdQuery, OperationResult<GetDiaryByIdDto>>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IValidator<GetDiaryByIdQuery> _validator;
        private readonly ICurrentUserService _currentUserService;
        
        public GetDiaryByIdHandler(IDiaryRepository diaryRepository, IValidator<GetDiaryByIdQuery> validator, ICurrentUserService currentUserService)
        {
            _diaryRepository = diaryRepository;
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task<OperationResult<GetDiaryByIdDto>> Handle(GetDiaryByIdQuery request, CancellationToken ct)
        {
            var studentId = _currentUserService.UserId;
            if (studentId == null)
            {
                return OperationResult<GetDiaryByIdDto>.Failure(Error.Validation(ErrorCodes.General.Validation, "ID för dagboken får inte vara tom"));
            }
            
            var diaryEntity = await _diaryRepository.GetTrackedByIdAsync(request.Id, ct);
            if (diaryEntity == null)
            {
                return OperationResult<GetDiaryByIdDto>.Failure(Error.NotFound(ErrorCodes.General.NotFound, $"Diary ID {request.Id} hittades inte"));
                
            }
            
            if (diaryEntity.StudentId != studentId)
            {
                return OperationResult<GetDiaryByIdDto>.Failure(Error.Validation(ErrorCodes.General.Validation, "Du har inte behörighet att se denna dagbok"));
            }
            
            var diaryRequestDto = new GetDiaryByIdDto(
                diaryEntity.Id,
                diaryEntity.StudentId,
                diaryEntity.EntryDate,
                diaryEntity.Text
            );
            
            return OperationResult<GetDiaryByIdDto>.Success(diaryRequestDto);
            
        }

    }
}