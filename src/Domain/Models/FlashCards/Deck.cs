#nullable enable
using Domain.Abstractions;
using System.Collections.Generic;
using System.Linq;
using System;
using Domain.Models.Users;

namespace Domain.Models.Flashcards // Ändrade namespace till Entities
{
    public sealed class Deck : IAggregateRoot
    {
        // FIX: Ändrat till private set; för inkapsling
        public Guid Id { get; private set; }
        public DateTime CreatedAtUtc { get; private set; }
        public DateTime UpdatedAtUtc { get; private set; }
        public string Title { get; private set; } = default!;
        public string CourseName { get; private set; } = default!;
        public string SubjectName { get; private set; } = default!;
        
        public Guid UserId { get; private set; }
        public User User { get; private set; } = default!;
        
        // FIX: Typo ändrad från FlashCards till FlashCard
        private readonly List<FlashCard> _flashCards = new();
        public IReadOnlyCollection<FlashCard> FlashCards => _flashCards.AsReadOnly();
        
        private Deck(){}

        public Deck(string title, string courseName, string subjectName, Guid userId)
        {
            Id = Guid.NewGuid();
            CreatedAtUtc = DateTime.UtcNow;
            UpdatedAtUtc = CreatedAtUtc;
            UserId = userId;
            
            SetTitle(title);
            SetCategories(courseName, subjectName);
        }

        public void SetTitle(string title)
        {
            // FIX: Logiken var bakvänd. Kasta fel om den ÄR tom.
            if (string.IsNullOrWhiteSpace(title))
                throw new ArgumentException("Titel krävs.", nameof(title));
                
            Title = title.Trim();
            Touch();
        }

        public void SetCategories(string courseName, string subjectName)
        {
            CourseName = courseName.Trim();
            SubjectName = subjectName.Trim();
            // FIX: Lade till Touch()
            Touch();
        }

        public FlashCard AddFlashCard(string frontText, string backText)
        {
            // FIX: Lade till 'this.Id' som sista parameter
            var newCard = new FlashCard(frontText, backText, this.Id);
            _flashCards.Add(newCard);
            Touch();
            return newCard;
        }

        public void RemoveCard(Guid flashCardId)
        {
            // FIX: Typo 'flashCardsId' -> 'flashCardId' och lade till semikolon
            var card = _flashCards.FirstOrDefault(c => c.Id == flashCardId);
            if (card != null)
            {
                _flashCards.Remove(card);
                Touch();
            }
        }

        public static Deck Load(
            Guid id,
            DateTime createdAtUtc,
            DateTime updatedAtUtc,
            string title,
            string courseName,
            string subjectName,
            Guid userId)
        {
            return new Deck
            {
                Id = id,
                CreatedAtUtc = createdAtUtc,
                UpdatedAtUtc = updatedAtUtc,
                Title = title,
                CourseName = courseName,
                SubjectName = subjectName,
                UserId = userId
            };
        }
        
        private void Touch() => UpdatedAtUtc = DateTime.UtcNow;
    }
}