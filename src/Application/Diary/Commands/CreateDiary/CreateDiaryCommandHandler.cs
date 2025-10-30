using Application.Abstractions;
using Application.Abstractions.IPersistence.Repositories; // <-- VIKTIG
using Application.Common.Results;
using Application.Diary.Dtos;
using Application.Student.Repository;
using Domain.Common;
using Domain.Entities;
using Domain.Models.Diary;
using FluentValidation;
using MediatR;
// TA BORT: using Microsoft.AspNetCore.Http;
// TA BORT: using System.Security.Claims;

namespace Application.Diary.Commands.CreateDiary
{
    public sealed class CreateDiaryCommandHandler
        : IRequestHandler<CreateDiaryCommand, OperationResult<CreateDiaryEntryDto>>
    {
        IDiaryRepository _diaryRepository;
        IValidator<CreateDiaryCommand> _validator;
        IStudentRepository _studentRepository;
        ICurrentUserService _currentUserService; // <-- SKA ANVÄNDA DENNA

        public CreateDiaryCommandHandler(
            IDiaryRepository diaryRepository,
            IValidator<CreateDiaryCommand> validator,
            IStudentRepository studentRepository,
            ICurrentUserService currentUserService) // <-- Injicera den
        {
            _diaryRepository = diaryRepository;
            _validator = validator;
            _studentRepository = studentRepository;
            _currentUserService = currentUserService;
        }

        public async Task<OperationResult<CreateDiaryEntryDto>> Handle(CreateDiaryCommand request, CancellationToken ct)
        {
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult<CreateDiaryEntryDto>.Failure(Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }

            // STEG 2: Hämta INTERNT ID från vår service (som middlewaren har satt)
            var studentGuid = _currentUserService.UserId; // <-- Läs från servicen

            if (!studentGuid.HasValue || studentGuid.Value == Guid.Empty)
            {
                // Om vi hamnar här har middlewaren misslyckats
                return OperationResult<CreateDiaryEntryDto>.Failure(
                    Error.Validation(ErrorCodes.General.Validation, "Kunde inte identifiera den inloggade användaren."));
            }
            
            // STEG 3: Hämta studenten
            // (Vi använder GetTrackedByIdAsync eftersom vi vet det INTERNA Guid-ID:t)
            UserEntity? student = await _studentRepository.GetTrackedByIdAsync(studentGuid.Value, ct);
            
            if (student is null)
            {
                return OperationResult<CreateDiaryEntryDto>.Failure(Error.NotFound("User.NotFound", "Användaren kunde inte hittas i systemet."));
            }

            // STEG 4: Kontrollera för 409 CONFLICT (vårt ursprungliga fel)
            if (await _diaryRepository.EntryExistsForDateAsync(student.Id, request.EntryDate, ct))
            {
                return OperationResult<CreateDiaryEntryDto>.Failure(Error.Conflict(ErrorCodes.DiaryError.DailyLimitExceeded, "Endast ett dagboksinlägg per dag är tillåtet"));
            }

            // STEG 5: Skapa och spara
            var diaryEntry = new DiaryEntry(
                student.Id,
                request.EntryDate,
                request.Text);
                
            await _diaryRepository.AddAsync(diaryEntry, ct);
            
            // STEG 6: Returnera
            var responseDto = new CreateDiaryEntryDto(
                diaryEntry.Id, 
                diaryEntry.StudentId,
                diaryEntry.EntryDate);
            
            return OperationResult<CreateDiaryEntryDto>.Success(responseDto);
        }
    }
}