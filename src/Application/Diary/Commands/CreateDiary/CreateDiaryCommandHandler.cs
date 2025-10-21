using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Diary.Dtos;
using Application.Student.Repository;
using Domain.Abstractions.Enum;
using Domain.Common;
using Domain.Entities;
using Domain.Models.Diary;
using FluentValidation;
using MediatR;

namespace Application.Diary.Commands.CreateDiary
{
    public sealed class CreateDiaryCommandHandler
        : IRequestHandler<CreateDiaryCommand, OperationResult<CreateDiaryEntryDto>>
    {
        IDiaryRepository _diaryRepository;
        IValidator<CreateDiaryCommand> _validator;
        IStudentRepository _studentRepository;


        public CreateDiaryCommandHandler(
            IDiaryRepository diaryRepository,
            IValidator<CreateDiaryCommand> validator,
            IStudentRepository studentRepository)
        {
            _diaryRepository = diaryRepository;
            _validator = validator;
            _studentRepository = studentRepository;
        }

        public async Task<OperationResult<CreateDiaryEntryDto>> Handle(CreateDiaryCommand request, CancellationToken ct)
        {
            // STEG 1: Validera inkommande data
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult<CreateDiaryEntryDto>.Failure(Error.Validation(ErrorCodes.General.Validation, errorMessages));
            }
            
            // STEG 2: Hämta den lokala användaren baserat på Logto-ID:t
            // Vi kan lita på att användaren finns, eftersom vår middleware redan har kört.
            // Notera att vi nu hämtar den rika domänmodellen 'User', inte en 'UserEntity'.
            var student = await _studentRepository.GetDomainUserByExternalIdAsync(request.UserId, ct);
            
            if (student is null)
            {
                // Detta borde teoretiskt aldrig hända, men är en bra säkerhetskontroll.
                return OperationResult<CreateDiaryEntryDto>.Failure(Error.NotFound("User.NotFound", "Användaren kunde inte hittas i systemet."));
            }

            // STEG 3: Kontrollera affärsregler (t.ex. ett inlägg per dag)
            if (await _diaryRepository.EntryExistsForDateAsync(student.Id, request.EntryDate, ct))
            {
                return OperationResult<CreateDiaryEntryDto>.Failure(Error.Conflict(ErrorCodes.DiaryError.DailyLimitExceeded, "Endast ett dagboksinlägg per dag är tillåtet"));
            }

            // STEG 4: Skapa och spara den nya dagboksanteckningen
            var diaryEntry = new DiaryEntry(
                student.Id, // Använd det stabila, interna Guid-ID:t
                request.EntryDate,
                request.Text);
                
            await _diaryRepository.AddAsync(diaryEntry, ct);
            
            // STEG 5: Returnera ett framgångsrikt resultat
            var responseDto = new CreateDiaryEntryDto(
                diaryEntry.Id, 
                diaryEntry.StudentId,
                diaryEntry.EntryDate);
            
            return OperationResult<CreateDiaryEntryDto>.Success(responseDto);
        }

        public async Task<UserEntity> GetByExternalIdAsync(string externalId, CancellationToken ct)
        {
            return await _studentRepository.GetByExternalIdAsync(externalId, ct);
        }
    }
}