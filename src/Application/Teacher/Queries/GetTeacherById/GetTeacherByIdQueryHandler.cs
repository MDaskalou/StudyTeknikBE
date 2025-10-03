using Application.Abstractions.IPersistence;
using Application.Common.Results;
using Application.Teacher.Dtos;
using Application.Teacher.Repository;
using Domain.Common;
using MediatR;

namespace Application.Teacher.Queries.GetTeacherById
{
    public sealed class GetTeacherByIdQueryHandler 
        : IRequestHandler<GetTeacherByIdQuery, OperationResult<GetTeacherByIdDto?>>
    {
        private readonly ITeacherRepository _teacherRepository;
        
        public GetTeacherByIdQueryHandler(ITeacherRepository teacherRepository)
        {
            _teacherRepository = teacherRepository;
        }
        
        public async Task<OperationResult<GetTeacherByIdDto?>> Handle(GetTeacherByIdQuery request, CancellationToken ct)
        {
            var teacher = await _teacherRepository.GetByIdAsync(request.Id, ct);
            
            if (teacher is null)
            {
                var error = Error.NotFound(ErrorCodes.TeacherError.NotFound, $"Kunde inte hitta en lärare med ID: {request.Id}");
                return OperationResult<GetTeacherByIdDto?>.Failure(error);
            }
            
            return OperationResult<GetTeacherByIdDto?>.Success(teacher);
            
            
        }
    }
}