using Application.Teacher.Dtos;
using Domain.Entities;
using System;
using System.Threading;
using System.Threading.Tasks;
namespace Application.Teacher.Repository
{
    public interface ITeacherRepository
    {
         Task <GetTeacherByIdDto?> GetByIdAsync(Guid id, CancellationToken ct);
         
         Task<IReadOnlyList<GetAllTeachersDto>> GetAllAsync(CancellationToken ct);
         
         void Add(UserEntity user);
        
    }
}