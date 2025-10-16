using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Diary.Dtos;
using Application.Student.Repository;
using Domain.Common;
using System.Linq; 
using MediatR;

namespace Application.Diary.Queries.GetAllDiary
{
    public class GetAllDiaryQueryHandler : IRequestHandler<GetAllDiaryQuery, OperationResult<IReadOnlyList<GetAllDiaryDto>>>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IStudentRepository _studentRepository;
        
        public GetAllDiaryQueryHandler(IDiaryRepository diaryRepository, IStudentRepository studentRepository)
        {
            _diaryRepository = diaryRepository;
            _studentRepository = studentRepository;
        }

        public async Task<OperationResult<IReadOnlyList<GetAllDiaryDto>>> Handle(GetAllDiaryQuery request, CancellationToken ct)
        {
            // Hitta studenten baserat på ID:t från requesten
            var student = await _studentRepository.GetByExternalIdAsync(request.UserId, ct);

            if (student == null)
            {
                return OperationResult<IReadOnlyList<GetAllDiaryDto>>.Success(new List<GetAllDiaryDto>());
            }

            var diaryEntities = await _diaryRepository.GetAllForStudentAsync(student.Id, ct);
            
            // Mappa till din nya DTO och korta ner texten för en summeringsvy
            var returnGetAllDiaryDtos = diaryEntities.Select(d => new GetAllDiaryDto(
                d.Id,
                d.Text.Length > 100 ? d.Text.Substring(0, 100) + "..." : d.Text,
                d.EntryDate
            )).ToList();
            
            return OperationResult<IReadOnlyList<GetAllDiaryDto>>.Success(returnGetAllDiaryDtos);
        }
    }
}