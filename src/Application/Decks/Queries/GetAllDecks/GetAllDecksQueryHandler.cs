#nullable enable
using Application.Common.Results;
using Application.Decks.Dtos;
using Application.Decks.IRepository; // Kontrollera namespace
using Application.Mapper;
using Domain.Common; // För ErrorCodes
using MediatR;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Application.Abstractions.IPersistence.Repositories;

namespace Application.Decks.Queries.GetAllDecks
{
    public sealed class GetAllDecksQueryHandler
        : IRequestHandler<GetAllDecksQuery, OperationResult<List<DeckDto>>>
    {
        private readonly IDeckRepository _deckRepository; 
        private readonly ICurrentUserService _currentUserService;

        public GetAllDecksQueryHandler(
            IDeckRepository deckRepository, 
            ICurrentUserService currentUserService)
        {
            _deckRepository = deckRepository;
            _currentUserService = currentUserService;
        }

        public async Task<OperationResult<List<DeckDto>>> Handle(
            GetAllDecksQuery request, CancellationToken ct)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return OperationResult<List<DeckDto>>.Failure(
                    Error.Forbidden(ErrorCodes.General.Forbidden, "Användaren är inte inloggad."));
            }

            var domainDecks = await _deckRepository.GetByUserIdAsync(userId.Value, ct); 

            // 3. Mappa domänmodellerna till DTOs
            var dtos = domainDecks.Select(DeckMapper.ToDto).ToList(); 

            // 4. Returnera lyckat resultat med listan av DTOs
            // FIX: Specificera <List<DeckDto>> vid Success
            return OperationResult<List<DeckDto>>.Success(dtos);
        }
    }
}