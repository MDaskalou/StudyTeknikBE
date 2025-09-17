namespace Domain.Models.Mentors
{
    //Koppling mellan Mentor (User) och Student (User).
    //TODO: Behövs denna klass? Kan vi inte bara ha en lista med studenter i User-klassen för mentorer?
    //Kan vara bra att ha en separat klass för att kunna lägga till mer information i framtiden.
    //Exempelvis startdatum för mentorskapet, slutdatum, status (aktiv/inaktiv) etc.
    // TODO: säkerställ att mentor har Role.Mentor i Application innan skapande.

    public class MentorAssignment
    {
        //Identitet och metadata
        
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        //FK till User (Student) inga navigation i MVP
        
        public Guid MentorId { get; set; }
        public Guid StudentId { get; set; }
        
        private MentorAssignment() { }
        
        public MentorAssignment(Guid mentorId, Guid studentId)
        {
            
            if( mentorId == Guid.Empty ) throw new ArgumentNullException("Mentor id krävs", nameof(mentorId));
            if( studentId == Guid.Empty ) throw new ArgumentNullException("Student id krävs", nameof(studentId));
            
            Id = Guid.NewGuid();
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = DateTime.UtcNow;
            
            MentorId = mentorId;
            StudentId = studentId;
        }
        
        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
        
    }
}