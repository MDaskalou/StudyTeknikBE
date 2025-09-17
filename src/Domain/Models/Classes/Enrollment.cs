namespace Domain.Models.Classes
{
    // Koppling mellan User och Class (many-to-many).
    // vi har many to many mellan User och Class,
    // Det är för att en elev kan tillföra flera klasser (t.ex. om en elev byter klass under året)
    // och en klass kan ha flera elever.
   
    //Enrollment metoden 
   
    // TODO: Koppling User<->Class (many-to-many).
    //TODO:  Unik id för varje relation.
   
    public class Enrollment
    {
        // Identitet och metadata
        public Guid Id { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        // Relationer (FK)
        public Guid StudentId { get; private set; } // User.Id
        public Guid ClassId   { get; private set; } // Class.Id

        // För EF
        private Enrollment() { }

        public Enrollment(Guid studentId, Guid classId)
        {
            if (studentId == Guid.Empty) throw new ArgumentException("StudentId krävs.", nameof(studentId));
            if (classId   == Guid.Empty) throw new ArgumentException("ClassId krävs.",   nameof(classId));

            Id = Guid.NewGuid();
            var now = DateTime.UtcNow;
            CreatedAtUtc = now;
            UpdatedAtUtc = now;

            StudentId = studentId;
            ClassId   = classId;
        }

        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}