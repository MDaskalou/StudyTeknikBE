using Application.Common.Results;
using Application.Student.Dtos;
using Application.Student.Repository;
using Domain.Abstractions.Enum;
using Domain.Entities;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Domain.Common;
using Domain.Models.Users;
using Infrastructure.Persistence.Mapper;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _db;
        public StudentRepository(AppDbContext db) => _db = db;

        // === READ-METODER ===

        public async Task<IReadOnlyList<StudentDetailsDto>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Users
                .AsNoTracking()
                .Where(u => u.Role == Role.Student)
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Select(u => new StudentDetailsDto(u.Id, u.FirstName, u.LastName, u.Email))
                .ToListAsync(ct);
        }

        public async Task<GetStudentByIdDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == id && u.Role == Role.Student)
                .Select(u => new GetStudentByIdDto(u.Id, u.FirstName, u.LastName, u.Email, u.SecurityNumber, null))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<UserEntity?> GetTrackedByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == Role.Student, ct);
        }

        public async Task<bool> EmailExistsAsync(string email, CancellationToken ct)
        {
            return await _db.Users.AnyAsync(u => u.Email == email, ct);
        }

        public async Task<bool> ClassExistsAsync(Guid classId, CancellationToken ct)
        {
            return await _db.Classes.AnyAsync(c => c.Id == classId, ct);
        }

        // === WRITE-METODER ===

        public async Task<OperationResult> AddAsync(User user, CancellationToken ct)
        {
            var userEntity = user.ToEntity();
            try
            {
                var enrollment = new EnrollmentEntity
                {
                    Id = Guid.NewGuid(),
                    CreatedAtUtc = userEntity.CreatedAtUtc,
                    UpdatedAtUtc = userEntity.UpdatedAtUtc,
                    StudentId = userEntity.Id,
                };

                _db.Users.Add(userEntity);
                _db.Enrollments.Add(enrollment);
                await _db.SaveChangesAsync(ct);
                return OperationResult.Success();
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(Error.InternalServiceError("Database.Error", "Ett databasfel inträffade vid skapande av student."));
            }
        }

        public async Task AddEntityAsync(UserEntity userEntity, CancellationToken ct)
        {
            await _db.Users.AddAsync(userEntity, ct);
            // Spara OMEDELBART för att slutföra provisioneringen.
            await _db.SaveChangesAsync(ct); 
        }

        public async Task<OperationResult> UpdateAsync(User user, CancellationToken ct)
        {
            var userEntity = user.ToEntity();

            try
            {
                _db.Users.Update(userEntity);
                await _db.SaveChangesAsync(ct);
                return OperationResult.Success();
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(Error.InternalServiceError("Database.Error", "Ett databasfel inträffade vid uppdatering av student."));
            }
        }

        public async Task<OperationResult> DeleteAsync(Guid id, CancellationToken ct)
        {
            try
            {
                await _db.Enrollments
                    .Where(e => e.StudentId == id)
                    .ExecuteDeleteAsync(ct);

                var rowsAffected = await _db.Users
                    .Where(u => u.Id == id && u.Role == Role.Student)
                    .ExecuteDeleteAsync(ct);

                if (rowsAffected == 0)
                {
                    // Detta är inte ett "krasch"-fel, så vi kan returnera ett mer specifikt fel
                    return OperationResult.Failure(Error.NotFound("Student.NotFound", $"Kunde inte hitta student med ID {id} att radera."));
                }

                return OperationResult.Success();
            }
            catch (DbUpdateException)
            {
                return OperationResult.Failure(Error.InternalServiceError("Database.Error", "Ett databasfel inträffade vid radering av student."));
            }
        }

        public async Task<UserEntity> GetByExternalIdAsync(string externalId, CancellationToken ct)
        {
            return await _db.Users
                .FirstOrDefaultAsync(u => u.ExternalSubject == externalId, ct);
        }

        public async Task<User?> GetDomainUserByExternalIdAsync(string externalId, CancellationToken ct)
        {
            var userEntity = await _db.Users
                .AsNoTracking()
                .FirstOrDefaultAsync(u => u.ExternalSubject == externalId, ct);

            if (userEntity is null)
            {
                return null;
            }
            
            return userEntity.ToModel();
        }

        public async Task<User?> GetTrackedDomainUserByIdAsync(Guid id, CancellationToken ct)
        {
            var userEntity = await _db.Users
                .FirstOrDefaultAsync(u => u.Id == id && u.Role == Role.Student, ct);
            if (userEntity is null)
            {
                return null;
            }
            
            return userEntity.ToModel();
        }
    }
}