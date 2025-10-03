using Domain.Common;
using Domain.Models.Common;

namespace Domain.Models.Users
{
    //Todo: VO för samtycke
    //Todo: immunt(oföränderligt): skapa nytt om ändring krävs
    //Todo: Spara vem som satte samtycke, när,  för spårbarhet
    //Value Object (om eleven har gett samtycke eller ej).
    
    // sealed class är för att förhindra ärvning
    // Varför vi sätter sealed på studentconsent är för att 
    //vi vill inte att någon annan klass ska ärva från den här klassen.
    //Det är en designbeslut för att säkerställa att klassen förblir
    //oförändrad och inte kan utökas med ytterligare funktionalitet.
    public sealed class StudentConsent : ValueObject
    {
        public bool Given { get; }
        public DateTime? GivenAtUtc { get; }
        public string? SetBy { get; }

        private StudentConsent() { }

        private StudentConsent(bool given, DateTime? givenAtUtc, string? setBy)
        {
            Given = given;
            GivenAtUtc = givenAtUtc;
            SetBy = string.IsNullOrWhiteSpace(setBy) ? null : setBy.Trim();
        }

        public static StudentConsent Granted(string? setBy)
            => new StudentConsent(true, DateTime.UtcNow, setBy);

        public static StudentConsent Revoked(string? setBy)
            => new StudentConsent(false, null, setBy);

        public static StudentConsent Create(bool given, string? setBy)
            => given ? Granted(setBy) : Revoked(setBy);

        // Viktigt: matcha ValueObject-signaturen (object?)
        protected override IEnumerable<object?> GetEqualityComponents()
        {
            yield return Given;
            yield return GivenAtUtc;
            yield return SetBy;
        }
    }
}