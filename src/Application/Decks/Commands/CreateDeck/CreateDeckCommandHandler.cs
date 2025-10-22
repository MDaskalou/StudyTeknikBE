using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Decks.Dtos;
using Application.Decks.IRepository;
using Application.Mapper;
using Application.Student.Repository;
using Domain.Common;
using Domain.Models.Flashcards;
using FluentValidation;
using MediatR;

namespace Application.Decks.Commands.CreateDeck
{
    public class CreateDeckCommandHandler : IRequestHandler<CreateDeckCommand, OperationResult<DeckDto>>
    {
        private readonly IDeckRepository _deckRepository;
        private readonly IStudentRepository _studentRepository;
        private readonly IValidator<CreateDeckCommand> _validator;
        private readonly ICurrentUserService _currentUserService;

        public CreateDeckCommandHandler(
            IDeckRepository deckRepository,
            IStudentRepository studentRepository,
            IValidator<CreateDeckCommand> validator,
            ICurrentUserService currentUserService)
        {
            _deckRepository = deckRepository;
            _studentRepository = studentRepository;
            _validator = validator;
            _currentUserService = currentUserService;
        }

        public async Task<OperationResult<DeckDto>> Handle(
            CreateDeckCommand request, CancellationToken ct)
        {
            // STEG 1: Validera
            var validationResult = await _validator.ValidateAsync(request, ct);
            if (!validationResult.IsValid)
            {
                var errorMessage = string.Join(",", validationResult.Errors.Select(x => x.ErrorMessage));
                return OperationResult<DeckDto>.Failure(Error.Validation(ErrorCodes.General.Validation, errorMessage));
            }
            
            // STEG 2: Hämta användar-ID (Antag att .UserId är din interna Guid)
            var userId = _currentUserService.UserId; 
            if (userId == null)
            {
                // FIX 1: Använd Error.Forbidden() som nu finns
                return OperationResult<DeckDto>.Failure(
                    Error.Forbidden(ErrorCodes.General.Forbidden,"Användare är inte inloggad"));
            }
            
            // STEG 3: Hämta användare
            // FIX 2: Byt till metoden som finns i ditt interface
            var student = await  _studentRepository.GetTrackedDomainUserByIdAsync(userId.Value, ct);
            if (student == null)
            {
                return OperationResult<DeckDto>.Failure(
                    Error.NotFound(ErrorCodes.StudentError.NotFound, $"Student med id {userId.Value} kunde inte hittas"));
            }

            // STEG 4: Kontrollera roll
            if (student.Role != Role.Student)
            {
                // FIX 3: Använd Error.Forbidden() som nu finns
                return OperationResult<DeckDto>.Failure(
                    Error.Forbidden(ErrorCodes.General.Forbidden, "Endast studenter kan skapa flashcards"));
            }

            // STEG 5: Skapa domänmodell
            var deck = new Deck(
                request.Title,
                request.CourseName,
                request.SubjectName,
                student.Id
            );
            
            // STEG 6: Spara
            await _deckRepository.AddAsync(deck, ct);
            
            // STEG 7: Mappa och returnera
            var dto = DeckMapper.ToDto(deck); 

            // FIX 4: Specificera <DeckDto> vid Success
            return OperationResult<DeckDto>.Success(dto);
        }
    }
}