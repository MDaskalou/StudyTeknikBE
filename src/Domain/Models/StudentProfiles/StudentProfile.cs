using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Abstractions;
using Domain.Abstractions.Enum; // Innehåller IAggregateRoot
using Domain.Common;       // Innehåller Result och DomainException

namespace Domain.Models.StudentProfiles
{
    public sealed class StudentProfile : IAggregateRoot
    {
        // 1. Manuella egenskaper
        public Guid Id { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        public Guid StudentId { get; private set; }

        // Inställningar
        public int PlanningHorizonWeeks { get; private set; }
        public TimeSpan WakeUpTime { get; private set; }
        public TimeSpan BedTime { get; private set; }

        // 2. Inkapslad lista
        private readonly List<Course> _courses = new();
        public IReadOnlyCollection<Course> Courses => _courses.AsReadOnly();

        private StudentProfile() { }

        // Publik konstruktor (Detta är en Root, så man får skapa den utifrån)
        public StudentProfile(Guid studentId, int planningWeeks, TimeSpan wakeUp, TimeSpan bedTime)
        {
            Id = Guid.NewGuid();
            StudentId = studentId;
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = CreatedAtUtc;

            SetPlanningPreferences(planningWeeks, wakeUp, bedTime);
        }

        // --- COMMANDS ---

        public void SetPlanningPreferences(int weeks, TimeSpan wakeUp, TimeSpan bedTime)
        {
            if (weeks < 1) 
                throw new DomainException("Planering måste vara minst 1 vecka.");
            
            PlanningHorizonWeeks = weeks;
            WakeUpTime = wakeUp;
            BedTime = bedTime;
            Touch();
        }

        public Result AddCourse(string name, string? description, CourseDifficulty difficulty)
        {
            // Validering på Profil-nivå
            if (_courses.Any(c => c.Name.Equals(name, StringComparison.OrdinalIgnoreCase)))
                return Result.Failure("Kursen finns redan.");

            // Anropa den interna fabriken på Course
            var courseResult = Course.Create(this.Id, name, description, difficulty);
            
            if (courseResult.IsFailure) 
                return Result.Failure(courseResult.Error);

            _courses.Add(courseResult.Value);
            Touch();
            
            return Result.Success();
        }

        public Result UpdateCourseDifficulty(Guid courseId, CourseDifficulty newDifficulty)
        {
            var course = _courses.FirstOrDefault(c => c.Id == courseId);
            if (course == null) return Result.Failure("Kursen hittades inte.");

            course.UpdateDifficulty(newDifficulty);
            Touch();
            return Result.Success();
        }

        // --- LOAD METOD ---
        public static StudentProfile Load(
            Guid id, 
            Guid studentId, 
            int planningWeeks, 
            TimeSpan wakeUp, 
            TimeSpan bedTime, 
            DateTime created, 
            DateTime updated,
            IEnumerable<Course> courses)
        {
            var profile = new StudentProfile
            {
                Id = id,
                StudentId = studentId,
                PlanningHorizonWeeks = planningWeeks,
                WakeUpTime = wakeUp,
                BedTime = bedTime,
                CreatedAtUtc = created,
                UpdatedAtUtc = updated
            };

            if (courses != null)
                profile._courses.AddRange(courses);

            return profile;
        }

        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}