using Domain.Abstractions;

namespace Domain.Models.Classes
{
    // TODO: Aggregate Root för skolklass.
    //Aggregate menas att en klass är en rot i en trädstruktur av objekt.
    //Many-to-one relation med User (en klass har många elever, en elev tillhör en klass).
    //Todo: // - Unik kombination (SchoolName, Year, ClassName) i DB.
    //Todo: Äger sina Enrollment-relationer i domänmodell.
    //Enrollment menas relationen mellan User och Class.
    // TODO: validera year (rimlig range), icke tomma strängar.

    //Klassen som elever tillhör (t.ex. “9B på Bräckeskolan”).
    public class Class : IAggregateRoot
    {
        //Identitet och Metadata 
        public Guid Id { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }
        
        //Egenskaper
        public string SchoolName { get; private set; } = default!;
        public int Year { get; private set; }
        public string ClassName { get; private set; } = default!;
        
        //Relationer
        private readonly List<Enrollment> _enrollments = new();
        public IReadOnlyCollection<Enrollment> Enrollments => _enrollments.AsReadOnly();
        
        private Class() { }
        
        public Class(string schoolName, int year, string className)
        {
            SetIdentity();
            SetClassInfo(schoolName, year, className);
        }
        
        //Domänbeteende
        public void EnrollStudent(Guid studentId)
        {
            if (studentId == Guid.Empty)
                throw new ArgumentException("Student ID krävs.", nameof(studentId));

            if (_enrollments.Any(e => e.StudentId == studentId))
                throw new InvalidOperationException("Student är redan inskriven i klassen.");

            _enrollments.Add(new Enrollment(studentId, Id)); // lägg till kopplingen
            Touch();
        }

        public void UnenrollStudent(Guid studentId)
        {
            var enrollment = _enrollments.FirstOrDefault(e => e.StudentId == studentId);
            if (enrollment == null)
                throw new InvalidOperationException("Student är inte inskriven i klassen.");
            
            _enrollments.Remove(enrollment);
            Touch();
        }
        
        // ==== Hjälpmetoder ====
        private void SetClassInfo(string schoolName, int year, string className)
        {
            if (string.IsNullOrWhiteSpace(schoolName))
                throw new ArgumentException("School name krävs.", nameof(schoolName));
            if (string.IsNullOrWhiteSpace(className))
                throw new ArgumentException("Class name krävs.", nameof(className));
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
