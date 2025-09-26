using Application.Student.Dtos;
using Application.Student.Repository;
using Domain.Abstractions.Enum;
using Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class StudentRepository : IStudentRepository
    {
        private readonly AppDbContext _db;
        public StudentRepository(AppDbContext db) => _db = db;

        // ... dina GetAllAsync och GetByIdAsync metoder är korrekta och ska vara kvar ...

        public async Task<IReadOnlyList<StudentDetailsDto>> GetAllAsync(CancellationToken ct)
        {
            return await _db.Users
                .AsNoTracking()
                .Where(u => u.Role == UserRole.Student)
                .OrderBy(u => u.FirstName).ThenBy(u => u.LastName)
                .Select(u => new StudentDetailsDto(u.Id, u.FirstName, u.LastName, u.Email))
                .ToListAsync(ct);
        }

        public async Task<GetStudentByIdDto?> GetByIdAsync(Guid id, CancellationToken ct)
        {
            // Din befintliga kod här...
            return await _db.Users
                .AsNoTracking()
                .Where(u => u.Id == id && u.Role == UserRole.Student)
                .Select(u => new GetStudentByIdDto(u.Id, u.FirstName, u.LastName, u.Email, u.SecurityNumber, null))
                .FirstOrDefaultAsync(ct);
        }

        public async Task<UserEntity?> GetTrackedByIdAsync(Guid id, CancellationToken ct)
        {
            return await _db.Users.FirstOrDefaultAsync(u => u.Id == id && u.Role == UserRole.Student, ct);
        }

        public void Add(UserEntity user, Guid classId)
        {
            // 1. Skapa den relaterade "enrollment"-entiteten
            var enrollment = new EnrollmentEntity
            {
                Id = Guid.NewGuid(),
                CreatedAtUtc = user.CreatedAtUtc,
                UpdatedAtUtc = user.UpdatedAtUtc,
                StudentId = user.Id, // <-- Här används user.Id (stort I)
                ClassId = classId
            };

            // 2. Lägg till båda i DbContext. VI SPARAR INTE HÄR.
            _db.Users.Add(user);
            _db.Enrollments.Add(enrollment);
        }
        
        
    }
}