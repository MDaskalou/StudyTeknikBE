using Application.Abstractions.IPersistence.Repositories;
using Application.Common.Results;
using Application.Decks.Dtos;
using Application.Decks.IRepository;
using Application.Mapper;
using Domain.Common;
using MediatR;

namespace Application.Decks.Queries.GetDeckById
{
    public sealed class GetDeckByIdQueryHandler: IRequestHandler<GetDeckByIdQuery, OperationResult<DeckDto>>
    {
        private readonly IDeckRepository _deckRepository;
        private readonly ICurrentUserService _currentUserService;
        
        public GetDeckByIdQueryHandler(IDeckRepository deckRepository, ICurrentUserService currentUserService)
        {
            _deckRepository = deckRepository;
            _currentUserService = currentUserService;
        }

        public async Task<OperationResult<DeckDto>> Handle(GetDeckByIdQuery request, CancellationToken ct)
        {
            var userId = _currentUserService.UserId;
            if (userId == null)
            {
                return OperationResult<DeckDto>.Failure(Error.NotFound(ErrorCodes.General.NotFound,"Användare inte hittad."));
            }
            
            var deck = await _deckRepository.GetByIdAsync(request.DeckId, ct); 
            if (deck == null)
            {
                return OperationResult<DeckDto>.Failure(Error.NotFound(ErrorCodes.DeckError.NotFound, "Kortleken hittades inte."));
            }

            if (deck.UserId != userId)
            {
                return OperationResult<DeckDto>.Failure(
                    Error.NotFound(ErrorCodes.DeckError.NotFound, "Kortleken hittades inte."));
            }
            
            return OperationResult<DeckDto>.Success(DeckMapper.ToDto(deck));
            
        }
        
    }
}