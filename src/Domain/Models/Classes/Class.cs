using System;
using System.Collections.Generic;
using System.Linq;
using Domain.Abstractions;

namespace Domain.Models.Classes
{
    public class Class : IAggregateRoot
    {
        // Identitet och metadata
        public Guid Id { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        // Egenskaper
        public string SchoolName { get; private set; } = default!;
        public int Year { get; private set; }
        public string ClassName { get; private set; } = default!;

        // Relationer
        private readonly List<Enrollment> _enrollments = new();
        public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();

        private Class() { } // för rehydrate

        // Skapa ny klass i domänen (utan att ta id utifrån)
        public Class(string schoolName, int year, string className)
        {
            SetIdentity();                  // sätter nytt Id + timestamps
            SetClassInfo(schoolName, year, className);
        }

        // Domänbeteende
        public void EnrollStudent(Guid studentId)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID krävs.", nameof(studentId));

            if (_enrollments.Any(e => e.StudentId == studentId))
                throw new InvalidOperationException("Student är redan inskriven i klassen.");

            _enrollments.Add(new Enrollment(studentId, Id));
            Touch();
        }

        public void UnenrollStudent(Guid studentId)
        {
            var enrollment = _enrollments.FirstOrDefault(e => e.StudentId == studentId)
                ?? throw new InvalidOperationException("Student är inte inskriven i klassen.");

            _enrollments.Remove(enrollment);
            Touch();
        }

        // Rehydrate från persistence (Entities)
        public static Class Rehydrate(Guid id, DateTime createdAtUtc, DateTime updatedAtUtc,
                                      string schoolName, int year, string className)
        {
            var c = new Class();
            typeof(Class).GetProperty(nameof(Id))!.SetValue(c, id);
            typeof(Class).GetProperty(nameof(CreatedAtUtc))!.SetValue(c, createdAtUtc);
            typeof(Class).GetProperty(nameof(UpdatedAtUtc))!.SetValue(c, updatedAtUtc);
            typeof(Class).GetProperty(nameof(SchoolName))!.SetValue(c, schoolName);
            typeof(Class).GetProperty(nameof(Year))!.SetValue(c, year);
            typeof(Class).GetProperty(nameof(ClassName))!.SetValue(c, className);
            return c;
        }

        // Hjälpmetoder
        private void SetClassInfo(string schoolName, int year, string className)
        {
            if (string.IsNullOrWhiteSpace(schoolName))
                throw new ArgumentException("Skolnamn krävs.", nameof(schoolName));
            if (string.IsNullOrWhiteSpace(className))
                throw new ArgumentException("Klassnamn krävs.", nameof(className));
            if (year < 2000 || year > 2100)
                throw new ArgumentOutOfRangeException(nameof(year), "Året måste vara mellan 2000 och 2100.");

            SchoolName = schoolName.Trim();
            Year       = year;
            ClassName  = className.Trim();

            Touch();
        }

        private void SetIdentity()
        {
            Id = Guid.NewGuid();
            var now = DateTime.UtcNow;
            CreatedAtUtc = now;
            UpdatedAtUtc = now;
        }

        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}
