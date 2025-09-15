using Domain.Abstractions;

namespace Domain.Entities.Diary
{
    // Veckosammanfattning (unik per StudentId + YearWeek, t.ex. "2025-W36")
    public sealed class WeeklySummary : IAggregateRoot
    {
        // Identitet & metadata
        public Guid Id { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        // Kärndata
        public Guid StudentId { get; private set; }
        public string YearWeek { get; private set; } = default!;
        public string Text { get; private set; } = string.Empty;

        // För EF Core
        private WeeklySummary() { }

        // Publik skapare (du kan också göra en statisk fabrik om du vill)
        public WeeklySummary(Guid studentId, string yearWeek, string text)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("StudentId krävs.", nameof(studentId));

            if (string.IsNullOrWhiteSpace(yearWeek))
                throw new ArgumentException("YearWeek krävs.", nameof(yearWeek));

            // OBS: formatkontroll av ISO-vecka gör vi i Application (FluentValidation)
            var now = DateTime.UtcNow;

            Id = Guid.NewGuid();
            CreatedAtUtc = now;
            UpdatedAtUtc = now;

            StudentId = studentId;
            YearWeek = yearWeek.Trim();
            UpdateText(text); // trimmar och touch:ar
        }

        public void UpdateText(string newText)
        {
            Text = (newText ?? string.Empty).Trim();
            Touch();
        }

        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}