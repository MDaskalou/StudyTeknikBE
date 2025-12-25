﻿using Domain.Models.Diary;
using Application.AI.Dtos;

namespace Application.Abstractions.IPersistence
{
    public interface IAIService
    {
        Task<string> RewriteDiaryEntryAsync(string originalText, CancellationToken cancellationToken);
        Task<GenerateFlashCardsResponseDto> GenerateFlashCardsAsync(string pdfContent, CancellationToken cancellationToken);
    }
}