using System;
using Domain.Abstractions.Enum;
using Domain.Common; // För Result

namespace Domain.Models.StudentProfiles
{
    public sealed class Course
    {
        // 1. Manuella egenskaper (istället för arv)
        public Guid Id { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        public string Name { get; private set; }
        public string? Description { get; private set; }
        public CourseDifficulty Difficulty { get; private set; }

        // Tom konstruktor för EF Core
        private Course() { }

        // 2. INTERNAL: Bara StudentProfile får skapa denna
        internal Course(Guid id, string name, string? description, CourseDifficulty difficulty)
        {
            Id = id;
            Name = name;
            Description = description;
            Difficulty = difficulty;
            
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = CreatedAtUtc;
        }

        // 3. INTERNAL FACTORY
        internal static Result<Course> Create(string name, string? description, CourseDifficulty difficulty)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Course>.Failure("Kursnamn krävs.");

            return Result<Course>.Success(new Course(Guid.NewGuid(), name, description, difficulty));
        }

        // 4. Beteende
        public void UpdateDifficulty(CourseDifficulty newDifficulty)
        {
            if (Difficulty == newDifficulty) return;
            
            Difficulty = newDifficulty;
            Touch();
        }

        // 5. Load-metod för Repository
        public static Course Load(
            Guid id, 
            string name, 
            string? description, 
            CourseDifficulty difficulty,
            DateTime created, 
            DateTime updated)
        {
            return new Course(id, name, description, difficulty)
            {
                CreatedAtUtc = created,
                UpdatedAtUtc = updated
            };
        }

        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}