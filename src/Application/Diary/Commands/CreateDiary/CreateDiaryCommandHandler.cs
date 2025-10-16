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

        public async Task<OperationResult<CreateDiaryEntryDto>> Handle(CreateDiaryCommand request,
            CancellationToken ct)
        {
            var student = await _studentRepository.GetByExternalIdAsync(request.UserId, ct);
            
            //Just in time provisioning
            if (student == null)
            {
                var nameParts = request.Name?.Split(' ', 2) ?? new[] { "Ny", "Användare" };
                var firstName = nameParts[0];
                var lastName = nameParts.Length > 1 ? nameParts[1] : "";
                
                var newUser = new UserEntity
                {
                    Id = Guid.NewGuid(),
                    ExternalSubject = request.UserId, // Den viktiga kopplingen till Logto!
                    FirstName = firstName,
                    LastName = lastName,
                    Email = request.Email ?? string.Empty,
                    Role = UserRole.Student,
                    CreatedAtUtc = DateTime.UtcNow,
                    UpdatedAtUtc = DateTime.UtcNow,
                    ExternalProvider = "Logto",
                    SecurityNumber = string.Empty // Sätt standardvärde
                };
                
                var addResult = await _studentRepository.AddAsync(newUser, ct);

                if (addResult.IsFailure)
                {
                    return OperationResult<CreateDiaryEntryDto>.Failure(addResult.Error);
                }
            }

            var studentId = student.Id;
            
            
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errorMessages = string.Join(",",  validationResult.Errors.Select(e => e.ErrorMessage));
                return OperationResult<CreateDiaryEntryDto>.Failure(Error.Validation
                    (ErrorCodes.General.Validation, errorMessages));
            }

            if (await _diaryRepository.EntryExistsForDateAsync(studentId, request.EntryDate, ct))
            {
                return OperationResult<CreateDiaryEntryDto>.Failure(Error.Conflict
                    (ErrorCodes.DiaryError.DailyLimitExceeded, "Endast ett dagboksinlägg per dag är tillåtet"));
            }

            var diaryEntry = new DiaryEntry(
                studentId,
                request.EntryDate,
                request.Text);
            await _diaryRepository.AddAsync(diaryEntry, ct);
            
            
            
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