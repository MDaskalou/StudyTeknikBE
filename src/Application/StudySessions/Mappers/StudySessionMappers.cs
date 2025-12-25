using Application.StudySessions.DTOs;
using Domain.Models.StudySessions;

namespace Application.StudySessions.Mappers
{
    public static class StudySessionMappers
    {
        /// <summary>
        /// Map Domain Model to DTO
        /// </summary>
        public static StudySessionDto ToDto(this StudySession domain)
        {
            return new StudySessionDto(
                Id: domain.Id,
                UserId: domain.UserId,
                CourseId: domain.CourseId,
                SessionGoal: domain.SessionGoal,
                StartDateUtc: domain.StartDateUtc,
                EndDateUtc: domain.EndDateUtc,
                PlannedMinutes: domain.PlannedMinutes,
                ActualMinutes: domain.ActualMinutes,
                EnergyStart: domain.EnergyStart,
                EnergyEnd: domain.EnergyEnd,
                Status: domain.Status,
                Steps: domain.Steps.Select(s => s.ToDto()).ToList(),
                CreatedAtUtc: domain.CreatedAtUtc,
                UpdatedAtUtc: domain.UpdatedAtUtc
            );
        }

        /// <summary>
        /// Map StudySessionStep Domain Model to DTO
        /// </summary>
        public static StudySessionStepDto ToDto(this StudySessionStep domain)
        {
            return new StudySessionStepDto(
                Id: domain.Id,
                OrderIndex: domain.OrderIndex,
                StepType: domain.StepType,
                Description: domain.Description,
                DurationMinutes: domain.DurationMinutes,
                IsCompleted: domain.IsCompleted,
                CreatedAtUtc: domain.CreatedAtUtc
            );
        }
    }
}

