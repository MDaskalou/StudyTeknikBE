using Domain.Models.StudySessions;

namespace Application.StudySessions.Repository
{
    /// <summary>
    /// Repository Interface - returns/accepts DOMAIN MODELS, not entities.
    /// The implementation handles DB <-> Domain mapping.
    /// </summary>
    public interface IStudySessionRepository
    {
        /// <summary>
        /// Get study session by ID
        /// </summary>
        Task<StudySession?> GetByIdAsync(Guid sessionId, CancellationToken ct = default);

        /// <summary>
        /// Get all study sessions for a user
        /// </summary>
        Task<List<StudySession>> GetByUserIdAsync(Guid userId, CancellationToken ct = default);

        /// <summary>
        /// Get all study sessions for a course
        /// </summary>
        Task<List<StudySession>> GetByCourseIdAsync(Guid courseId, CancellationToken ct = default);

        /// <summary>
        /// Add new study session (with steps)
        /// </summary>
        Task AddAsync(StudySession session, CancellationToken ct = default);

        /// <summary>
        /// Update existing study session
        /// </summary>
        Task UpdateAsync(StudySession session, CancellationToken ct = default);

        /// <summary>
        /// Delete study session and its steps
        /// </summary>
        Task DeleteAsync(Guid sessionId, CancellationToken ct = default);
    }
}

