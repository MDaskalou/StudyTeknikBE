using Application.StudySessions.Repository;
using Domain.Entities;
using Domain.Models.StudySessions;
using Infrastructure.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Repositories
{
    /// <summary>
    /// StudySessionRepository - This is the TRANSLATOR between:
    /// - DB Layer (StudySessionsEntity - anemic, no logic)
    /// - Domain Layer (StudySession - rich, with logic)
    /// 
    /// The Interface returns/accepts DOMAIN MODELS.
    /// This class handles all mapping internally.
    /// </summary>
    public sealed class StudySessionRepository : IStudySessionRepository
    {
        private readonly AppDbContext _context;

        public StudySessionRepository(AppDbContext context)
        {
            _context = context;
        }

        public async Task<StudySession?> GetByIdAsync(Guid sessionId, CancellationToken ct = default)
        {
            var entity = await _context.StudySessions
                .AsNoTracking()
                .Include(s => s.Steps)
                .FirstOrDefaultAsync(s => s.Id == sessionId, ct);

            if (entity == null)
                return null;

            return MapToDomain(entity);
        }

        public async Task<List<StudySession>> GetByUserIdAsync(Guid userId, CancellationToken ct = default)
        {
            var entities = await _context.StudySessions
                .AsNoTracking()
                .Include(s => s.Steps)
                .Where(s => s.UserId == userId)
                .ToListAsync(ct);

            return entities.Select(MapToDomain).ToList();
        }

        public async Task<List<StudySession>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default)
        {
            var entities = await _context.StudySessions
                .AsNoTracking()
                .Include(s => s.Steps)
                .Where(s => s.CourseId == courseId)
                .ToListAsync(ct);

            return entities.Select(MapToDomain).ToList();
        }

        public async Task AddAsync(StudySession session, CancellationToken ct = default)
        {
            var entity = MapToEntity(session);
            await _context.StudySessions.AddAsync(entity, ct);
            await _context.SaveChangesAsync(ct);
        }

        public async Task UpdateAsync(StudySession session, CancellationToken ct = default)
        {
            var existingEntity = await _context.StudySessions
                .Include(s => s.Steps)
                .FirstOrDefaultAsync(s => s.Id == session.Id, ct);

            if (existingEntity == null)
                throw new InvalidOperationException($"StudySession with id {session.Id} not found");

            // Update properties
            existingEntity.SessionGoal = session.SessionGoal;
            existingEntity.StartDateUtc = session.StartDateUtc;
            existingEntity.EndDateUtc = session.EndDateUtc;
            existingEntity.PlannedMinutes = session.PlannedMinutes;
            existingEntity.ActualMinutes = session.ActualMinutes;
            existingEntity.EnergyStart = session.EnergyStart;
            existingEntity.EnergyEnd = session.EnergyEnd;
            existingEntity.Status = session.Status;
            existingEntity.UpdatedAtUtc = session.UpdatedAtUtc;

            // Update steps
            existingEntity.Steps.Clear();
            foreach (var step in session.Steps)
            {
                existingEntity.Steps.Add(MapStepToEntity(step, session.Id));
            }

            _context.StudySessions.Update(existingEntity);
            await _context.SaveChangesAsync(ct);
        }

        public async Task DeleteAsync(Guid sessionId, CancellationToken ct = default)
        {
            var entity = await _context.StudySessions
                .FirstOrDefaultAsync(s => s.Id == sessionId, ct);

            if (entity != null)
            {
                _context.StudySessions.Remove(entity);
                await _context.SaveChangesAsync(ct);
            }
        }

        // ========== PRIVATE MAPPING METHODS ==========

        /// <summary>
        /// Map Anemic Entity -> Rich Domain Model
        /// </summary>
        private static StudySession MapToDomain(StudySessionsEntity entity)
        {
            var steps = entity.Steps
                .OrderBy(s => s.OrderIndex)
                .Select(s => StudySessionStep.Load(
                    s.Id,
                    s.OrderIndex,
                    s.StepType,
                    s.Description,
                    s.DurationMinutes,
                    s.IsCompleted,
                    s.CreatedAtUtc))
                .ToList();

            var session = StudySession.Load(
                entity.Id,
                entity.UserId,
                entity.CourseId,
                entity.SessionGoal,
                entity.StartDateUtc,
                entity.EndDateUtc,
                entity.PlannedMinutes,
                entity.ActualMinutes,
                entity.EnergyStart,
                entity.EnergyEnd,
                entity.Status,
                entity.CreatedAtUtc,
                entity.UpdatedAtUtc
            );

            session.LoadSteps(steps);
            return session;
        }

        /// <summary>
        /// Map Rich Domain Model -> Anemic Entity
        /// </summary>
        private static StudySessionsEntity MapToEntity(StudySession domain)
        {
            var entity = new StudySessionsEntity
            {
                Id = domain.Id,
                UserId = domain.UserId,
                CourseId = domain.CourseId,
                SessionGoal = domain.SessionGoal,
                StartDateUtc = domain.StartDateUtc,
                EndDateUtc = domain.EndDateUtc,
                PlannedMinutes = domain.PlannedMinutes,
                ActualMinutes = domain.ActualMinutes,
                EnergyStart = domain.EnergyStart,
                EnergyEnd = domain.EnergyEnd,
                Status = domain.Status,
                CreatedAtUtc = domain.CreatedAtUtc,
                UpdatedAtUtc = domain.UpdatedAtUtc
            };

            // Map steps
            foreach (var step in domain.Steps)
            {
                entity.Steps.Add(MapStepToEntity(step, domain.Id));
            }

            return entity;
        }

        /// <summary>
        /// Map StudySessionStep Domain Model -> StudySessionStepEntity
        /// </summary>
        private static StudySessionStepEntity MapStepToEntity(StudySessionStep domainStep, Guid sessionId)
        {
            return new StudySessionStepEntity
            {
                Id = domainStep.Id,
                StudySessionId = sessionId,
                OrderIndex = domainStep.OrderIndex,
                StepType = domainStep.StepType,
                Description = domainStep.Description,
                DurationMinutes = domainStep.DurationMinutes,
                IsCompleted = domainStep.IsCompleted,
                CreatedAtUtc = domainStep.CreatedAtUtc
            };
        }
    }
}

