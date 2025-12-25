# AI Flashcard Generation Feature

## Overview
Detta feature möjliggör för studenter att automatiskt generera 10-15 flashcards genom att ladda upp en PDF-fil. AI analyserar innehållet och skapar begrepp/frågor (FrontText) med motsvarande svar (BackText).

## API Endpoints

### 1. Generate Flashcards from Text
**Endpoint:** `POST /api/ai/generate-cards`

**Request Body:**
```json
{
  "pdfContent": "Extraherad text från PDF...",
  "deckId": "guid-till-deck"
}
```

**Response:**
```json
{
  "count": 12,
  "cards": [
    {
      "frontText": "Vad är fotosyntesis?",
      "backText": "Fotosyntesis är processen där växter använder solljus för att omvandla vatten och koldioxid till glukos och syre..."
    },
    ...
  ]
}
```

### 2. Generate Flashcards from PDF File
**Endpoint:** `POST /api/ai/generate-cards-from-file`

**Request Type:** multipart/form-data

**Parameters:**
- `file` (IFormFile) - PDF-filen (max 10 MB)
- `deckId` (Guid) - ID på decket där korten ska läggas

**Response:** Samma som endpoint 1

## How It Works

### Flow
1. Användare laddar upp en PDF-fil eller skickar text
2. PDF-text extraheras via iText7-biblioteket
3. Text skickas till Google Gemini AI med en structured prompt
4. AI genererar JSON med flashcards
5. Korten parsas och läggs automatiskt till i det valda decket
6. Resultatet returneras till användaren

### Validation
- PDF-innehål max 50 000 tecken
- Fil-storlek max 10 MB
- Endast PDF-filer tillåtna
- DeckId måste vara valid
- Användaren måste äga decket

## Architecture

### New Files Created:
- `Application/AI/Commands/GenerateFlashCards/GenerateFlashCardsCommand.cs`
- `Application/AI/Commands/GenerateFlashCards/GenerateFlashCardsCommandHandler.cs`
- `Application/AI/Commands/GenerateFlashCards/GenerateFlashCardsCommandValidator.cs`
- `Application/AI/Dtos/GenerateFlashCardsRequestDto.cs`
- `Application/AI/Dtos/GenerateFlashCardsResponseDto.cs`
- `Application/AI/Dtos/GenerateFlashCardsFromTextRequestDto.cs`
- `Infrastructure/Service/PdfExtractor.cs`

### Modified Files:
- `Application/Abstractions/IPersistence/IAIService.cs` - Added GenerateFlashCardsAsync method
- `Infrastructure/Service/AIService.cs` - Implemented GenerateFlashCardsAsync method
- `StudyTeknik/Controllers/AiController.cs` - Added two new endpoints

### NuGet Packages Added:
- `itext7.core` - PDF-parsing library

## AI Prompt
AI uppmanas att:
1. Analysera läsmaterial (PDF-innehål)
2. Identifiera 10-15 viktiga begrepp/frågor
3. Returnera strukturerad JSON med frontText och backText
4. Fokusera på:
   - Viktiga koncept och definitioner
   - Begrepp som är lätta att testa
   - Variation i ämnena

## Error Handling
- Validation errors → 400 Bad Request
- Not Found → 404 Not Found
- Forbidden/Authorization → 403 Forbidden
- AI errors → 500 Internal Server Error with details

## Future Enhancements
- [ ] Support for DOCX, TXT files
- [ ] Batch processing för flera PDFs
- [ ] Custom prompt templates
- [ ] Flashcard regeneration/refinement
- [ ] Difficulty level selection
- [ ] Custom question types

