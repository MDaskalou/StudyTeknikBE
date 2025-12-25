using Application.Courses.Repository;
using Application.StudentProfiles.IRepository;
using Domain.Models.StudentProfiles;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    public sealed class CourseRepository : ICourseRepository
    {
        private readonly AppDbContext _context;

        public CourseRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<Course?> GetByIdAsync(Guid courseId, CancellationToken ct = default)
        {
            var courseEntity = await _context.Courses
                .AsNoTracking()
                .FirstOrDefaultAsync(c => c.Id == courseId, ct);

            if (courseEntity == null)
                return null;

            return MapToDomain(courseEntity);
        }

        public async Task<List<Course>> GetByStudentProfileIdAsync(Guid studentProfileId, CancellationToken ct = default)
        {
            var courseEntities = await _context.Courses
                .AsNoTracking()
                .Where(c => c.StudentProfileId == studentProfileId)
                .ToListAsync(ct);

            return courseEntities.Select(MapToDomain).ToList();
        }

        public async Task<bool> IsNameUniquePerProfileAsync(Guid studentProfileId, string name, Guid? excludeCourseId = null, CancellationToken ct = default)
        {
            var query = _context.Courses
                .AsNoTracking()
                .Where(c => c.StudentProfileId == studentProfileId && 
                           c.Name.ToLower() == name.ToLower());

            if (excludeCourseId.HasValue && excludeCourseId.Value != Guid.Empty)
            {
                query = query.Where(c => c.Id != excludeCourseId.Value);
            }

            var exists = await query.AnyAsync(ct);
            return !exists; // Returns true if unique (no duplicates found)
        }

        public async Task AddAsync(Course course, CancellationToken ct = default)
        {
            var courseEntity = MapToEntity(course);
            await _context.Courses.AddAsync(courseEntity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(Course course, CancellationToken ct = default)
        {
            var existingEntity = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == course.Id, ct);

            if (existingEntity == null)
                throw new InvalidOperationException($"Course with id {course.Id} not found");

            // Update properties
            existingEntity.Name = course.Name;
            existingEntity.Description = course.Description;
            existingEntity.Difficulty = course.Difficulty;
            existingEntity.UpdatedAtUtc = course.UpdatedAtUtc;

            _context.Courses.Update(existingEntity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid courseId, CancellationToken ct = default)
        {
            var courseEntity = await _context.Courses
                .FirstOrDefaultAsync(c => c.Id == courseId, ct);

            if (courseEntity != null)
            {
                _context.Courses.Remove(courseEntity);
                await _context.SaveChangesAsync(ct);
            }
        }

        // --- PRIVATE MAPPING METHODS ---

        private static Course MapToDomain(Domain.Entities.CourseEntity entity)
        {
            return Course.Load(
                id: entity.Id,
                studentProfileId: entity.StudentProfileId,
                name: entity.Name,
                description: entity.Description,
                difficulty: entity.Difficulty,
                created: entity.CreatedAtUtc,
                updated: entity.UpdatedAtUtc
            );
        }

        private static Domain.Entities.CourseEntity MapToEntity(Course domain)
        {
            return new Domain.Entities.CourseEntity
            {
                Id = domain.Id,
                StudentProfileId = domain.StudentProfileId,
                Name = domain.Name,
                Description = domain.Description,
                Difficulty = domain.Difficulty,
                CreatedAtUtc = domain.CreatedAtUtc,
                UpdatedAtUtc = domain.UpdatedAtUtc
            };
        }
    }
}

