using Application.Common.Results;
using Application.Teacher.Dtos;
using MediatR;

namespace Application.Teacher.Queries.GetAllTeachers
{
    public record GetAllTeachersQuery  
        : IRequest<OperationResult<IReadOnlyList<GetAllTeachersDto>>>;

    
        
    
}