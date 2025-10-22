namespace Domain.Common
{
    public static class ErrorCodes
    {
        public static class General
        {
            public const string Validation = "Validation.Error";
            public const string NotFound = "NotFound.Error";
            public const string Conflict = "Conflict.Error";
            public const string InternalServiceError = "InternalServiceError.Error";
            public const string Forbidden = "Forbidden.Error"; 

        }

        public static class StudentError
        {
            public const string NotFound = "Student.NotFound";
            public const string EmailAlreadyExists = "Student.EmailAlreadyExists";
        }
        public static class TeacherError
        {
            public const string NotFound = "Teacher.NotFound";
            public const string EmailAlreadyExists = "Teacher.EmailAlreadyExists";
        }
        
        public static class DiaryError
        {
            public const string NotFound = "Diary.NotFound";
            public const string DailyLimitExceeded = "Diary.DailyLimitExceeded";

        }

        public static class DeckError
        {
            public const string NotFound = "Deck.NotFound";
            public const string TitleAlreadyExists = "Deck.TitleAlreadyExists"; 
            public const string NotOwnedByUser = "Deck.NotOwnedByUser";
        }
        public static class FlashCardError
        {
            public const string NotFound = "FlashCard.NotFound";
            public const string NotInDeck = "FlashCard.NotInDeck";
        }
    }
}