﻿using System;
using Domain.Abstractions.Enum;
using Domain.Common; // För Result

namespace Domain.Models.StudentProfiles
{
    public sealed class Course
    {
        // 1. Manuella egenskaper (istället för arv)
        public Guid Id { get; private set; }
        public Guid StudentProfileId { get; internal set; }
        public DateTime CreatedAtUtc { get; internal set; }
        public DateTime UpdatedAtUtc { get; internal set; }

        public string Name { get; internal set; }
        public string? Description { get; internal set; }
        public CourseDifficulty Difficulty { get; internal set; }

        // Tom konstruktor för EF Core
        private Course() { }

        // 2. INTERNAL: Bara StudentProfile får skapa denna
        internal Course(Guid id, Guid studentProfileId, string name, string? description, CourseDifficulty difficulty)
        {
            Id = id;
            StudentProfileId = studentProfileId;
            Name = name;
            Description = description;
            Difficulty = difficulty;
            
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = CreatedAtUtc;
        }

        // 3. FACTORY METHOD
        public static Result<Course> Create(Guid studentProfileId, string name, string? description, CourseDifficulty difficulty)
        {
            if (string.IsNullOrWhiteSpace(name))
                return Result<Course>.Failure("Kursnamn krävs.");

            if (studentProfileId == Guid.Empty)
                return Result<Course>.Failure("Giltigt StudentProfileId krävs.");

            return Result<Course>.Success(new Course(Guid.NewGuid(), studentProfileId, name, description, difficulty));
        }

        // 4. Beteende
        public void UpdateDifficulty(CourseDifficulty newDifficulty)
        {
            if (Difficulty == newDifficulty) return;
            
            Difficulty = newDifficulty;
            Touch();
        }

        public void UpdateDetails(string name, string? description, CourseDifficulty difficulty)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("Kursnamn krävs.", nameof(name));

            Name = name;
            Description = description;
            Difficulty = difficulty;
            Touch();
        }

        // 5. Load-metod för IRepository
        public static Course Load(
            Guid id,
            Guid studentProfileId, 
            string name, 
            string? description, 
            CourseDifficulty difficulty,
            DateTime created, 
            DateTime updated)
        {
            return new Course(id, studentProfileId, name, description, difficulty)
            {
                CreatedAtUtc = created,
                UpdatedAtUtc = updated
            };
        }

        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}