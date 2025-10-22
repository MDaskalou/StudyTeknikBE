using Application.Common.Results;
using Application.Student.Dtos;
using Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

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
        Task<OperationResult> AddAsync(UserEntity user, CancellationToken ct);
        Task<OperationResult> UpdateAsync(UserEntity user, CancellationToken ct);
        Task<OperationResult> DeleteAsync(Guid id, CancellationToken ct);
        
        // ===Externa Subject===
        
        Task<UserEntity?> GetByExternalIdAsync(string externalId, CancellationToken ct);
    }
}