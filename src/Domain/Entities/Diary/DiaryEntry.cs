using Domain.Abstractions;

namespace Domain.Entities.Diary
{
    //Todo: Dagbokpost per datum
    //Todo:Unik per(studnetid , entrydate) i db- konfiguration
    //Todo: Text maxlängd i Application/Infrastructure
    // TODO: validera längd (t.ex. 5000) i Application-validator.

    
    //En dagbokspost (text, datum, elev).
    public sealed class DiaryEntry : IAggregateRoot
    {
        //Identitet och metadata 
        
        public Guid Id { get; set; }
        public DateTime CreatedAtUtc { get; set; }
        public DateTime UpdatedAtUtc { get; set; }
        
        //Kärndata 
        
        public Guid StudentId { get; set; }
        public DateOnly EntryDate { get; set; }
        public string Text { get; set; } = string.Empty;
        
        public DiaryEntry(){}
        
        public DiaryEntry(Guid studentId, DateOnly entryDate, string text)
        {
            if( studentId == Guid.Empty ) throw new ArgumentNullException("Student id krävs", nameof(studentId));
            if( string.IsNullOrWhiteSpace(text) ) throw new ArgumentNullException("Text krävs", nameof(text));
            
            Id = Guid.NewGuid();
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = DateTime.UtcNow;
            
            StudentId = studentId;
            EntryDate = entryDate;
            Text = text;
        }
        
        public void UpdateText(string newText)
        {
            if( string.IsNullOrWhiteSpace(newText) ) throw new ArgumentNullException("Text krävs", nameof(newText));
            Text = newText;
            Touch();
        }
        
        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;

    }
}