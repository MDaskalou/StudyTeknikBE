using Application.Teacher.Dtos;
using Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
using Application.Common.Results;

namespace Application.Teacher.Repository
{
    public interface ITeacherRepository
    {
         Task <GetTeacherByIdDto?> GetByIdAsync(Guid id, CancellationToken ct);
         
         Task<IReadOnlyList<GetAllTeachersDto>> GetAllAsync(CancellationToken ct);
         
         Task<bool>EmailExistAsync(string email, CancellationToken ct);
         
         Task<OperationResult>AddAsync(UserEntity user, CancellationToken ct);
         
        
    }
}