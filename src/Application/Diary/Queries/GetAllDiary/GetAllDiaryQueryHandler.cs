using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Diary.Dtos;
using Domain.Common;
using MediatR;

namespace Application.Diary.Queries.GetAllDiary
{
    public class GetAllDiaryQueryHandler : IRequestHandler<GetAllDiaryQuery, OperationResult<IReadOnlyList<GetAllDiaryDto>>>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly ICurrentUserService _currentUser;
        
        public GetAllDiaryQueryHandler(IDiaryRepository diaryRepository, ICurrentUserService currentUser)
        {
            _diaryRepository = diaryRepository;
            _currentUser = currentUser;
        }

        public async Task<OperationResult<IReadOnlyList<GetAllDiaryDto>>> Handle(GetAllDiaryQuery request,
            CancellationToken ct)
        {
            var studentId = _currentUser.UserId;
            if (studentId == null)
            {
                return OperationResult<IReadOnlyList<GetAllDiaryDto>>.Failure(
                    Error.Validation(ErrorCodes.General.Validation, "Användaren är inte inloggad.")
                    
                    );            
            }
            
            var diaries = await _diaryRepository.GetAllDiariesForStudentAsync(studentId.Value, ct);            
            return OperationResult<IReadOnlyList<GetAllDiaryDto>>.Success(diaries);
        }
    }
}