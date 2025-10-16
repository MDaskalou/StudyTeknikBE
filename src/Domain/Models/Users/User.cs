using Domain.Abstractions;
using Domain.Models.Common;

namespace Domain.Models.Users
{
    // Aggregate root för användare
    public sealed class User : IAggregateRoot
    {
        // Identitet & metadata
        public Guid Id { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }

        // Kärndata
        public string FirstName { get; private set; } = default!;
        public string LastName { get; private set; } = default!;
        public string SecurityNumber { get; private set; } = default!; // personnummer som text
        public string Email { get; private set; } = default!;
        public Role Role { get; private set; }

        // Extern IdP
        public string? ExternalProvider { get; private set; } // t.ex. "clerk", "logto"
        public string? ExternalSubject { get; private set; } // IdP "sub"

        // VO: samtycke
        public StudentConsent Consent { get; private set; } = StudentConsent.Revoked(null);

        private User()
        {
        } // För EF

        public User(
            string firstName,
            string lastName,
            string securityNumber,
            string email,
            Role role,
            string? externalProvider = null,
            string? externalSubject = null)
        {
            Id = Guid.NewGuid();
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = CreatedAtUtc;

            SetName(firstName, lastName);
            SetSecurityNumber(securityNumber);
            SetEmail(email);
            Role = role;

            if (!string.IsNullOrWhiteSpace(externalProvider) &&
                !string.IsNullOrWhiteSpace(externalSubject))
            {
                LinkExternalIdentity(externalProvider!, externalSubject!);
            }
        }

        // --- Domänmetoder ---

        public void ChangeRole(Role newRole)
        {
            if (Role == newRole) return;
            Role = newRole;
            Touch();
        }

        public void SetEmail(string email)
        {
            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException("Email krävs.", nameof(email));

            var normalized = email.Trim();
            if (!normalized.Contains('@'))
                throw new ArgumentException("Ogiltig email.", nameof(email));

            Email = normalized;
            Touch();
        }

        public void SetName(string firstName, string lastName)
        {
            if (string.IsNullOrWhiteSpace(firstName))
                throw new ArgumentException("Förnamn krävs.", nameof(firstName));
            if (string.IsNullOrWhiteSpace(lastName))
                throw new ArgumentException("Efternamn krävs.", nameof(lastName));

            FirstName = firstName.Trim();
            LastName = lastName.Trim();
            Touch();
        }

        public void SetSecurityNumber(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                throw new ArgumentException("Personnummer krävs.", nameof(value));

            // MVP: enkel trim. (Senare: validera/hashea/maska enligt GDPR)
            SecurityNumber = value.Trim();
            Touch();
        }

        public void SetConsent(bool given, string? setBy)
        {
            Consent = StudentConsent.Create(given, setBy );
            Touch();
        }

        public void LinkExternalIdentity(string provider, string subject)
        {
            if (string.IsNullOrWhiteSpace(provider))
                throw new ArgumentException("Provider krävs.", nameof(provider));
            if (string.IsNullOrWhiteSpace(subject))
                throw new ArgumentException("Subject krävs.", nameof(subject));

            ExternalProvider = provider.Trim().ToLowerInvariant();
            ExternalSubject = subject.Trim();
            Touch();
        }

        public void UnlinkExternalIdentity()
        {
            ExternalProvider = null;
            ExternalSubject = null;
            Touch();
        }

        public static User Load(
            Guid id,
            string firstName,
            string lastName,
            string email,
            Role role,
            string securityNumber,
            DateTime createdAtUtc,
            DateTime updatedAtUtc,
            string? externalProvider,
            string? externalSubject,
            StudentConsent consent)
        {
            return new User
            {
                Id = id,
                FirstName = firstName,
                LastName = lastName,
                Email = email,
                Role = role,
                CreatedAtUtc = createdAtUtc,
                UpdatedAtUtc = updatedAtUtc,
                ExternalProvider = externalProvider,
                ExternalSubject = externalSubject,
                Consent = consent
            };
        }

        // Uppdaterar senast-ändrad-tid. Anropas vid alla state-förändringar.
        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}
