using Application.Common.Results;
using Application.Student.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Domain.Models.Users;

namespace Application.Student.Repository
{
    public interface IStudentRepository
    {
        // === READ (LÄSA DATA) ===
        Task<IReadOnlyList<StudentDetailsDto>> GetAllAsync(CancellationToken ct);
        Task<GetStudentByIdDto?> GetByIdAsync(Guid id, CancellationToken ct);
        Task<UserEntity?> GetTrackedByIdAsync(Guid id, CancellationToken ct);
        Task<bool> EmailExistsAsync(string email, CancellationToken ct);
        Task<bool> ClassExistsAsync(Guid classId, CancellationToken ct);

        // === WRITE (SKRIVA DATA) ===
        Task<OperationResult> AddAsync(User user, CancellationToken ct);
        //Det är för min DevelopmentAuthenticationMiddleware ska fungera  
        Task AddEntityAsync(UserEntity userEntity, CancellationToken ct);
        Task<OperationResult> UpdateAsync(User user, CancellationToken ct);
        Task<User?> GetTrackedDomainUserByIdAsync(Guid id, CancellationToken ct);
        Task<OperationResult> DeleteAsync(Guid id, CancellationToken ct);
        Task<User?> GetDomainUserByExternalIdAsync(string externalId, CancellationToken ct);
        
        Task<UserEntity?> GetStudentByExternalIdAsync(string externalId, CancellationToken ct);
        Task<UserEntity?> GetByExternalIdAsync(string externalId, CancellationToken ct);
        
    }
}