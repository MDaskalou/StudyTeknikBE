﻿using Domain.Models.Diary;
using Application.AI.Dtos;
using Application.StudySessions.DTOs;

namespace Application.Abstractions.IPersistence
{
    public interface IAIService
    {
        Task<string> RewriteDiaryEntryAsync(string originalText, CancellationToken cancellationToken);
        Task<GenerateFlashCardsResponseDto> GenerateFlashCardsAsync(string pdfContent, CancellationToken cancellationToken);
        
        Task<List<CreateStudySessionStepRequest>> GenerateStudyStepsAsync(
            string sessionGoal,
            int plannedMinutes,
            int energyLevel,
            int difficultyLevel,
            int motivationLevel,
            string? learningChallenges,
            string? studyEnvironment,
            string? additionalContext,
            CancellationToken ct);
    }
}