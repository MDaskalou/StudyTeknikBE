﻿using Application.Abstractions;
using Application.Common.Results;
using Application.Diary.Commands.UpdateDiaryDetails;
using Application.Diary.Dtos;
using Application.Abstractions.IPersistence.Repositories;
using Domain.Common;
using FluentValidation;
using MediatR;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Application.Diary.Commands.UpdateDiaryDetails
{
    public sealed class UpdateDiaryDetailsCommandHandler : IRequestHandler<UpdateDiaryDetailsCommand, OperationResult>
    {
        private readonly IDiaryRepository _diaryRepository;
        private readonly IValidator<UpdateDiaryDetailsDto> _dtoValidator;
        private readonly ICurrentUserService _currentUser;

        public UpdateDiaryDetailsCommandHandler(
            IDiaryRepository diaryRepository, 
            IValidator<UpdateDiaryDetailsDto> dtoValidator, 
            ICurrentUserService currentUser)
        {
            _diaryRepository = diaryRepository;
            _dtoValidator = dtoValidator;
            _currentUser = currentUser;
        }

        public async Task<OperationResult> Handle(UpdateDiaryDetailsCommand request, CancellationToken ct)
        {
            // 1. Säkerhetskontroll: Hämta inloggad användare
            var studentId = _currentUser.UserId;
            if (studentId is null)
            {
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Validation, "Användaren är inte inloggad."));
            }

            // 2. Hämta dagboksinlägget
            var diaryEntity = await _diaryRepository.GetTrackedByIdAsync(request.Id, ct);
            if (diaryEntity is null)
            {
                return OperationResult.Failure(Error.NotFound(ErrorCodes.General.NotFound, $"Dagboksinlägg med ID {request.Id} kunde inte hittas."));
            }

            // 3. VIKTIG SÄKERHETSKONTROLL: Kontrollera att studenten äger inlägget
            if (diaryEntity.StudentId != studentId)
            {
                return OperationResult.Failure(Error.Validation("User.Forbidden", "Du har inte behörighet att redigera detta inlägg."));
            }

            var detailsToPatch = new UpdateDiaryDetailsDto { Text = diaryEntity.Text };

            try
            {
                request.PatchDoc.ApplyTo(detailsToPatch);
            }
            catch (Exception ex)
            {
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Validation, ex.Message));
            }

            var validationResult = await _dtoValidator.ValidateAsync(detailsToPatch, ct);
            if (!validationResult.IsValid)
            {
                return OperationResult.Failure(Error.Validation(ErrorCodes.General.Validation, string.Join(", ", validationResult.Errors.Select(e => e.ErrorMessage))));
            }

            // 5. Uppdatera entiteten och spara
            diaryEntity.Text = detailsToPatch.Text;
            diaryEntity.UpdatedAtUtc = DateTime.UtcNow;

            return await _diaryRepository.UpdateAsync(diaryEntity, ct);
        }
    }
}