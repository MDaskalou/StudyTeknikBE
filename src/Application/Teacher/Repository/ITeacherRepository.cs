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
        //Read
         Task <GetTeacherByIdDto?> GetByIdAsync(Guid id, CancellationToken ct);
         Task<IReadOnlyList<GetAllTeachersDto>> GetAllAsync(CancellationToken ct);
         
         //Check
         
         Task<bool>EmailExistsAsync(string email, CancellationToken ct);
        //Write
         Task<UserEntity?> GetTrackedByIdAsync(Guid id, CancellationToken ct);
         Task<OperationResult> UpdateAsync(UserEntity user, CancellationToken ct);
         Task<OperationResult>AddAsync(UserEntity user, CancellationToken ct);
         
        
    }
}