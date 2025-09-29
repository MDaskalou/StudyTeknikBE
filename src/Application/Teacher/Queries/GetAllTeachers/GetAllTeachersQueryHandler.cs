using Application.Abstractions.IPersistence;
using Application.Common.Results;
using Application.Teacher.Dtos;
using Application.Teacher.Repository;
using MediatR;

namespace Application.Teacher.Queries.GetAllTeachers
{
    public sealed class GetAllTeachersQueryHandler 
        : IRequestHandler<GetAllTeachersQuery, OperationResult<IReadOnlyList<GetAllTeachersDto>>>
    
    {
        private readonly ITeacherRepository _teacherRepository;
        
        public GetAllTeachersQueryHandler(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }

        public async Task<OperationResult<IReadOnlyList<GetAllTeachersDto>>>
            Handle(GetAllTeachersQuery request, CancellationToken cancellationToken)
        {
            var teachers = await _teacherRepository.GetAllAsync(cancellationToken);
            return OperationResult<IReadOnlyList<GetAllTeachersDto>>.Success(teachers);
        }
    }
}