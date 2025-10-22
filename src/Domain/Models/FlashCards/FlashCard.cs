using System;

namespace Domain.Models.Flashcards 
{
    public sealed class FlashCard
    {
        private const float DEFAULT_EASE_FACTOR = 2.5f;
        private const float MIN_EASE_FACTOR = 1.3f;
        
        // FIX: Ändrat till private set; för inkapsling
        public Guid Id { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }
        
        public string FrontText { get; private set; } = default!;
        public string BackText { get; private set; } = default!;
        
        public Guid DeckId { get; private set; }
        // FIX: Typo 'DeckEntity' -> 'Deck'. Satt till 'default!'
        public Deck Deck { get; private set; } = default!;
        
        public DateTime NextReviewAtUtc { get; private set; }
        public int Interval { get; private set; }
        public float EaseFactor { get; private set; } // Denna var redan korrekt!
        
        private FlashCard(){}

        // Konstruktorn ska vara 'internal' så att bara Deck kan skapa den.
        internal FlashCard(string frontText, string backText, Guid deckId)
        {
            Id = Guid.NewGuid();
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = CreatedAtUtc;
            DeckId = deckId;
            
            UpdateText(frontText, backText);
        }

        public void UpdateText(string frontText, string backText)
        {
            if (string.IsNullOrWhiteSpace(frontText))
                throw new ArgumentException("Främre texten kan inte vara tom.", nameof(frontText));
            if (string.IsNullOrWhiteSpace(backText))
                throw new ArgumentException("Bakre texten kan inte vara tom.", nameof(backText));
            
            FrontText = frontText.Trim();
            BackText = backText.Trim();

            ResetReviewStats();
            Touch();
        }

        // FIX: Hela Review-metoden är omskriven med korrekt SRS-logik.
        public void Review(int lapses)
        {
            if (lapses == 0)
            {
                // ---- Användaren klickade "Rätt" direkt ----
                if (Interval == 0)
                {
                    // 1. Från Nytt -> Lärd (1 dag)
                    Interval = 1;
                }
                else if (Interval == 1)
                {
                    // 2. Från 1 dag -> 6 dagar (standard)
                    Interval = 6; 
                }
                else
                {
                    // 3. Etablerad (använd EaseFactor)
                    Interval = (int)Math.Round(Interval * EaseFactor);
                }
            }
            else
            {
                // ---- Användaren klickade "Fel" minst en gång ----
                
                // Sänk EaseFactor, men inte under minimum
                EaseFactor = Math.Max(MIN_EASE_FACTOR, EaseFactor - 0.2f);
                
                // Återställ intervallet, kortet måste repeteras imorgon
                Interval = 1; 
            }

            // FIX: Nästa review ska vara I FRAMTIDEN, baserat på intervallet.
            NextReviewAtUtc = DateTime.UtcNow.AddDays(Interval);
            Touch();
        }

        public void ResetReviewStats()
        {
            Interval = 0;
            EaseFactor = DEFAULT_EASE_FACTOR;
            NextReviewAtUtc = CreatedAtUtc; // Redo att pluggas direkt
            Touch();
        }
        
        public static FlashCard Load(
            Guid id,
            DateTime createdAtUtc,
            DateTime updatedAtUtc,
            string frontText,
            string backText,
            Guid deckId,
            DateTime nextReviewAtUtc,
            int interval,
            float easeFactor)
        {
            return new FlashCard
            {
                Id = id,
                CreatedAtUtc = createdAtUtc,
                UpdatedAtUtc = updatedAtUtc,
                FrontText = frontText,
                BackText = backText,
                DeckId = deckId,
                NextReviewAtUtc = nextReviewAtUtc,
                Interval = interval,
                EaseFactor = easeFactor
            };
        }
        
        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}