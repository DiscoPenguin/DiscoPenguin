using StarWarsTcgApi.Application.DTOs.Requests;
using StarWarsTcgApi.Application.DTOs.Responses;
using StarWarsTcgApi.Application.Interfaces;
using StarWarsTcgApi.Domain.Interfaces;
using StarWarsTcgApi.Domain.Models;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text.Json;
using System.Transactions;

namespace StarWarsTcgApi.Application.Services
{
    public class GameService : IGameService
    {
        private readonly IGameRepository _gameRepository;
        private readonly IUserRepository _userRepository;
        private readonly IPlayerRepository _playerRepository;
        private readonly IDeckRepository _deckRepository;
        private readonly IGameCardRepository _gameCardRepository;
        private readonly ICardRepository _cardRepository; // To retrieve static card data
        private readonly IGameLogRepository _gameLogRepository;
        private readonly IDisposable _transactionScope; // To manage transactions (simplified in this code, for the time being)

        //Inject all the necessary repositories
        public GameService(
            IGameRepository gameRepository,
            IUserRepository userRepository,
            IPlayerRepository playerRepository,
            IDeckRepository deckRepository,
            IGameCardRepository gameCardRepository,
            ICardRepository cardRepository,
            IGameLogRepository gameLogRepository
        )
        {
            _gameRepository = gameRepository;
            _userRepository = userRepository;
            _playerRepository = playerRepository;
            _deckRepository = deckRepository;
            _gameCardRepository = gameCardRepository;
            _cardRepository = cardRepository;
            _gameLogRepository = gameLogRepository;
            // In a real scenario, you should use a UnitOfWork or Transaction Manager
            _transactionScope = new TransactionScope(TransactionScopeAsyncFlowOption.Enabled); //Example from System.Transactions
        }
        public async Task<GameDetailsResponse?> CreateGameAsync(CreateGameRequest request)
        {
            #region --- Game Logic : Validate Input ---
            if (request.LightPlayerId == request.DarkPlayerId)
            {
                throw new ArgumentException("Players must be distinct");
            }
            // Validate the decks exist and belong to the player specified
            var lightPlayerDeck = await _deckRepository.GetByIdAsync(request.LightPlayerDeckId);
            var darkPlayerDeck = await _deckRepository.GetByIdAsync(request.DarkPlayerDeckId);
            if (lightPlayerDeck == null || darkPlayerDeck == null)
            {
                throw new ArgumentException("One or both decks specified do not exist or do not belong to the specified player");
            }
            if (!lightPlayerDeck.IsValid || !darkPlayerDeck.IsValid)
            {
                throw new ArgumentException("One or both decks are listed as invalid");
            }
            #endregion

            //TODO: --- Transaction Management ---
            // In a real-world application, begin a database transaction here to ensure atomicity
            // If any step were to fail, everything would roll back
            try
            {
                // Create a new Game
                var newGame = new Game
                {
                    Id = null, // Will be reset to a valid value by database incrementor
                    LightPlayerId = 0, // Will be reassigned once a player is created from the indicated User Id
                    DarkPlayerId = 0, // Will be reassigned once a player is created from the indicated User Id
                    WinnerPlayerId = null,
                    CurrentTurnNumber = 0,
                    CurrentPlayerId = 0, // Will be reassigned once the Dark Side player is created from the indicated User Id,
                    CurrentPhaseId = (await _gameLogRepository.GetAllGamePhasesAsync()).First(gp => gp.Name == "None").Id,
                    CurrentStatusId = (await _gameLogRepository.GetAllGameStatusesAsync()).First(gs => gs.Name == "In Progress").Id,
                    LastUpdated = DateTime.UtcNow
                };
                int newGameId = await _gameRepository.AddAsync(newGame);

                // --- Log Game Creation ---
                await _gameLogRepository.AddAsync(new GameLog
                {
                    Id = null,
                    GameId = newGameId,
                    Timestamp = DateTime.UtcNow,
                    TurnNumber = 0,
                    PhaseId = newGame.CurrentPhaseId,
                    PlayerId = null, // No specific player initiated game creation log
                    ActionId = (await _gameLogRepository.GetAllActionTypesAsync()).First(at => at.Name == "GameCreated").Id,
                    ActionDetails = JsonDocument.Parse("{ {\"Message\": \"Game container created\"}, {\"Game.Id\": " + newGameId + "} }")
                });

                // Create a new instance of a User, as a Player of <<this>> game
                var lightPlayer = new Player
                {
                    Id = -1,
                    GameId = newGameId,
                    UserId = request.LightUserId.ToString(),
                    ForceTotal = 0,
                    BuildPoints = 0
                };
                int lightPlayerId = await _playerRepository.AddAsync(lightPlayer);
                lightPlayer.Id = lightPlayerId;
                
                // --- Log Game Creation ---
                await _gameLogRepository.AddAsync(new GameLog
                {
                    Id = null,
                    GameId = newGameId,
                    Timestamp = DateTime.UtcNow,
                    TurnNumber = 0,
                    PhaseId = newGame.CurrentPhaseId,
                    PlayerId = null, // No specific player initiated game creation log
                    ActionId = (await _gameLogRepository.GetAllActionTypesAsync()).First(at => at.Name == "PlayerAssignedToGame").Id,
                    ActionDetails = JsonDocument.Parse("{ {\"Message\": \"Assigned Light Player\"}, {\"Player.Id\": " + lightPlayerId + "} }")
                });

                // Create a new instance of a Player, as a Player of <<this>> game
                var darkPlayer = new Player
                {
                    Id = -1,
                    GameId = newGameId,
                    UserId = request.DarkUserId.ToString(),
                    ForceTotal = 0,
                    BuildPoints = 0
                };
                int darkPlayerId = await _playerRepository.AddAsync(darkPlayer);
                darkPlayer.Id = darkPlayerId;

                // --- Log Game Creation ---
                await _gameLogRepository.AddAsync(new GameLog
                {
                    Id = null,
                    GameId = newGameId,
                    Timestamp = DateTime.UtcNow,
                    TurnNumber = 0,
                    PhaseId = newGame.CurrentPhaseId,
                    PlayerId = null, // No specific player initiated game creation log
                    ActionId = (await _gameLogRepository.GetAllActionTypesAsync()).First(at => at.Name == "PlayerAssignedToGame").Id,
                    ActionDetails = JsonDocument.Parse("{ {\"Message\": \"Assigned Dark Player\"}, {\"Player.Id\": " + darkPlayerId + "} }")
                });

                //update the Game with the Player IDs
                newGame.Id = newGameId;
                newGame.LightPlayerId = lightPlayerId;
                newGame.DarkPlayerId = darkPlayerId;
                await _gameRepository.UpdateAsync(newGame);

                // --- Log Game Creation ---
                await _gameLogRepository.AddAsync(new GameLog
                {
                    Id = null,
                    GameId = newGame.Id.Value,
                    Timestamp = DateTime.UtcNow,
                    TurnNumber = 0,
                    PhaseId = newGame.CurrentPhaseId,
                    PlayerId = null, // No specific player initiated game creation log
                    ActionId = (await _gameLogRepository.GetAllActionTypesAsync()).First(at => at.Name == "GameCreated").Id,
                    ActionDetails = JsonDocument.Parse("{\"Message\": \"Game created\"}")
                });

                // Take a copy of each player's chosen Decks into the Game_Cards, listed as in the zone "Deck"
                var deckZoneId = (await _gameCardRepository.GetAllCardZonesAsync()).First(cz => cz.Name == "Deck").Id;
                await PopulateDeckIntoGameCards(newGameId, darkPlayerId, darkPlayerDeck, deckZoneId);
                await PopulateDeckIntoGameCards(newGameId, lightPlayerId, lightPlayerDeck, deckZoneId);

                // --- Draw Initial Hands (7 cards) ---
                await DrawCardsForPlayerAsync(newGameId, darkPlayerId, 7);
                await DrawCardsForPlayerAsync(newGameId, lightPlayerId, 7);

                // --- Commit Transaction ---
                //TODO: _transactionScope.Complete();

                // --- Return Response DTO ---
                var gameStatusName = (await _gameLogRepository.GetAllGameStatusesAsync()).First(gs => gs.Id == newGame.CurrentStatusId).Name;
                var currentPhaseName = (await _gameLogRepository.GetAllGamePhasesAsync()).First(gp => gp.Id == newGame.CurrentPhaseId).Name;

                return new GameDetailsResponse
                {
                    GameId = newGame.Id.Value,
                    LightPlayerId = newGame.LightPlayerId,
                    //LightPlayerUserName = lightPlayer.Name,
                    DarkPlayerId = newGame.DarkPlayerId,
                    //DarkPlayerUserName = darkPlayer.Name,
                    //CurrentTurnPlayerUsername = lightPlayer.Name,
                    CurrentPhaseName = currentPhaseName,
                    GameStatusName = gameStatusName,
                    LastUpdated = newGame.LastUpdated,
                    CreatedAt = newGame.CreatedAt,
                    LightPlayerState = await GetPlayerStateResponse(lightPlayer),
                    DarkPlayerState = await GetPlayerStateResponse(darkPlayer)
                };
            }
            catch (Exception ex)
            {
                // _transactionScope.Dispose(); //TODO: Rollback transaction on error
                Console.WriteLine($"Error creating game: {ex.Message}");
                // Log exception
                return null;
            }
        }

        #region Data Fetch
        public async Task<IEnumerable<GameStatistics>> GetGameStatisticsAsync(Guid? userId)
        {
            var gameStatistics = await _gameRepository.GetGameStatisticsAsync();
            if (userId.HasValue)
            {
                return gameStatistics.Where(s => s.UserId == userId.Value.ToString());
            }
            return gameStatistics;
        }
        public async Task<GameDetailsResponse?> GetGameByIdAsync(int gameId)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null) return null;

            var lightPlayer = await _playerRepository.GetByIdAsync(game.LightPlayerId);
            var darkPlayer = await _playerRepository.GetByIdAsync(game.DarkPlayerId);
            var currentTurnPlayer = await _playerRepository.GetByIdAsync(game.CurrentPlayerId);
            var winnerUser = game.WinnerPlayerId.HasValue ? await _playerRepository.GetByIdAsync(game.WinnerPlayerId.Value) : null;
            var gameStatus = await _gameLogRepository.GetAllGameStatusesAsync();
            var gamePhase = await _gameLogRepository.GetAllGamePhasesAsync();

            var lightUserInGame = await _userRepository.GetByIdAsync(Guid.Parse(lightPlayer?.UserId));
            var darkUserInGame = await _userRepository.GetByIdAsync(Guid.Parse(darkPlayer?.UserId));

            return new GameDetailsResponse
            {
                GameId = game.Id.Value,
                LightPlayerId = game.LightPlayerId,
                //LightPlayerUserName = lightPlayer?.Username ?? "N/A",
                DarkPlayerId = game.DarkPlayerId,
                //DarkPlayerUserName = darkPlayer?.Username ?? "N/A",
                CurrentTurnPlayerUserName = currentTurnPlayer?.Username ?? "N/A",
                CurrentPhaseName = gamePhase.FirstOrDefault(gp => gp.Id == game.CurrentPhaseId)?.Name ?? "Unknown",
                GameStatusName = gameStatus.FirstOrDefault(gs => gs.Id == game.CurrentStatusId)?.Name ?? "Unknown",
                WinnerUserName = winnerUser?.Username,
                LastUpdated = game.LastUpdated,
                CreatedAt = game.CreatedAt,
                LightPlayerState = lightUserInGame != null ? await GetPlayerStateResponse(lightPlayer) : null,
                DarkPlayerState = darkUserInGame != null ? await GetPlayerStateResponse(darkPlayer) : null
            };
        }

        public async Task<IEnumerable<GameDetailsResponse>> GetUserGamesAsync(int playerId)
        {
            var games = await _gameRepository.GetGamesForPlayerAsync(playerId);
            var gameDetailsResponses = new List<GameDetailsResponse>();
            foreach (var game in games)
            {
                var gameDetails = await GetGameByIdAsync(game.Id.Value); // Re-use method to get full details
                if (gameDetails != null)
                {
                    gameDetailsResponses.Add(gameDetails);
                }
            }
            return gameDetailsResponses;
        }

        public async Task<bool> PlayCardAsync(PlayCardRequest request)
        {
            // --- Game Logic: Validate Request ---
            var game = await _gameRepository.GetByIdAsync(request.GameId);
            if (game == null) return false;

            var playingPlayer = await _playerRepository.GetByIdAsync(request.PlayerId);
            if (playingPlayer == null || playingPlayer.GameId != request.GameId) return false; // Player not in this game

            // Check if it's the player's turn
            var currentTurnPlayer = await _playerRepository.GetByIdAsync(game.CurrentPlayerId);
            if (currentTurnPlayer == null || currentTurnPlayer.Id != playingPlayer.Id)
            {
                throw new InvalidOperationException("It's not this player's turn.");
            }

            var gameCard = await _gameCardRepository.GetByIdAsync(request.GameCardId);
            if (gameCard == null || gameCard.GameId != request.GameId || gameCard.OwnerPlayerId != playingPlayer.Id) return false;

            var handZoneId = (await _gameCardRepository.GetAllCardZonesAsync()).First(cz => cz.Name == "Hand").Id;
            if (gameCard.CurrentZoneId != handZoneId)
            {
                throw new InvalidOperationException("Selected Card is not in player's hand.");
            }

            var cardDefinition = await _cardRepository.GetByIdAsync(gameCard.CardId);
            if (cardDefinition == null) return false;

            var targetZone = (await _gameCardRepository.GetAllCardZonesAsync()).FirstOrDefault(cz => cz.Id == request.TargetZoneId);
            //TODO: targetZone is not "PlayArea"; Should be the card's arena or the build zone
            if (targetZone == null || targetZone.Name != "PlayArea")
            {
                throw new ArgumentException("Invalid target zone for this card.");
            }

            // --- Game Logic: Deploy Cost & Rules (Simplified) ---
            //TODO: Handle logic for partially building a card (to Build Zone)
            //TODO: Handle logic for completing a card build (to Arena or Active)
            int cardCost = -1;
            if (int.TryParse(cardDefinition.Cost, out cardCost))
            {
                if (playingPlayer.BuildPoints < cardCost)
                {
                    targetZone = (await _gameCardRepository.GetAllCardZonesAsync()).FirstOrDefault(cz => cz.Name == "Build");
                }
                // Deduct build points:
                playingPlayer.BuildPoints -= cardCost;
                await _playerRepository.UpdateAsync(playingPlayer);
            }

            // --- Game Logic: Handle Location Cards in Specific Zones ---
            if (cardDefinition.Type == "Location" && targetZone.CanHaveLocation)
            {
                // Check if a Location card already exists in this specific play area for this game
                var existingLocation = await _gameCardRepository.GetLocationCardInPlayAreaAsync(request.GameId, targetZone.Id);
                if (existingLocation != null)
                {
                    // Move existing location to discard pile
                    var discardZoneId = (await _gameCardRepository.GetAllCardZonesAsync()).First(cz => cz.Name == "Discard").Id;
                    existingLocation.CurrentZoneId = discardZoneId;
                    await _gameCardRepository.UpdateAsync(existingLocation);

                    // Log old location removal
                    await _gameLogRepository.AddAsync(new GameLog
                    {
                        Id = null,
                        GameId = request.GameId,
                        Timestamp = DateTime.UtcNow,
                        TurnNumber = (await _gameRepository.GetByIdAsync(request.GameId))?.CurrentTurnNumber,
                        PhaseId = (await _gameRepository.GetByIdAsync(request.GameId))?.Id,
                        PlayerId = request.PlayerId,
                        ActionId = (await _gameLogRepository.GetAllActionTypesAsync()).First(at => at.Name == "LocationReplaced").Id, // Assuming "LocationReplaced" action type
                        ActionDetails = JsonDocument.Parse($"{{\"ReplacedCardId\": \"{existingLocation.Id}\", \"NewCardId\": \"{gameCard.Id}\", \"ZoneId\": {targetZone.Id}}}")
                    });
                }
            }
            // --- Update Game_Card State ---
            gameCard.CurrentZoneId = request.TargetZoneId;
            gameCard.Sequence = null; // Reset sequence if moving to an unordered play area
            await _gameCardRepository.UpdateAsync(gameCard);

            // --- Log the Action ---
            await _gameLogRepository.AddAsync(new GameLog
            {
                Id = null,
                GameId = request.GameId,
                Timestamp = DateTime.UtcNow,
                TurnNumber = (await _gameRepository.GetByIdAsync(request.GameId))?.CurrentTurnNumber ?? 0, // Placeholder
                PhaseId = (await _gameRepository.GetByIdAsync(request.GameId))?.CurrentPhaseId,
                PlayerId = request.PlayerId,
                ActionId = (await _gameLogRepository.GetAllActionTypesAsync()).First(at => at.Name == "CardPlayed").Id,
                ActionDetails = JsonDocument.Parse($"{{\"GameCardId\": \"{gameCard.Id}\", \"CardName\": \"{cardDefinition.Name}\", \"TargetZoneId\": {request.TargetZoneId}}}")
            });

            return true;
        }

        public async Task<bool> DrawCardAsync(int gameId, int playerId)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null) return false;

            var player = await _playerRepository.GetByIdAsync(playerId);
            if (player == null || player.GameId != gameId) return false;

            // Check if it's the player's turn (or if draw is allowed out of turn, e.g., setup)
            var currentTurnPlayer = await _playerRepository.GetByIdAsync(game.CurrentPlayerId);
            if (currentTurnPlayer == null || currentTurnPlayer.Id != player.Id)
            {
                throw new InvalidOperationException("It's not this player's turn to draw.");
            }

            var deckZoneId = (await _gameCardRepository.GetAllCardZonesAsync()).First(cz => cz.Name == "Deck").Id;
            var handZoneId = (await _gameCardRepository.GetAllCardZonesAsync()).First(cz => cz.Name == "Hand").Id;

            // Get the top card from the deck (lowest sequence in zone)
            var deckCards = (await _gameCardRepository.GetGameCardsOwnedByPlayerInZoneAsync(gameId, playerId, deckZoneId))
                                .OrderBy(gc => gc.Sequence)
                                .ToList();

            if (!deckCards.Any())
            {
                Console.WriteLine($"Player {player.UserId} has no cards left in their deck.");
                // Game ending condition could be here
                return false;
            }

            var cardToDraw = deckCards.First();
            cardToDraw.CurrentZoneId = handZoneId;
            cardToDraw.Sequence = null; // Hand is re-sequenced by UI

            await _gameCardRepository.UpdateAsync(cardToDraw);

            // Re-sequence remaining cards in deck (optional, but good for consistency)
            for (int i = 0; i < deckCards.Count - 1; i++)
            {
                deckCards[i+1].Sequence = i; // The card that was at index 1 is now index 0, etc.
                await _gameCardRepository.UpdateAsync(deckCards[i+1]);
            }

            // Log the action
            var cardDefinition = await _cardRepository.GetByIdAsync(cardToDraw.CardId);
            await _gameLogRepository.AddAsync(new GameLog
            {
                Id = null,
                GameId = gameId,
                Timestamp = DateTime.UtcNow,
                TurnNumber = (await _gameRepository.GetByIdAsync(gameId))?.CurrentTurnNumber,
                PhaseId = (await _gameRepository.GetByIdAsync(gameId))?.CurrentPhaseId,
                PlayerId = playerId,
                ActionId = (await _gameLogRepository.GetAllActionTypesAsync()).First(at => at.Name == "CardDrawn").Id,
                ActionDetails = JsonDocument.Parse($"{{\"GameCardId\": \"{cardToDraw.Id}\", \"CardName\": \"{cardDefinition?.Name ?? "Unknown Card"}\"}}")
            });

            return true;
        }

        public async Task<bool> EndTurnAsync(int gameId, int currentPlayerId)
        {
            var game = await _gameRepository.GetByIdAsync(gameId);
            if (game == null) return false;

            var player = await _playerRepository.GetByIdAsync(currentPlayerId);
            if (player == null || player.GameId != gameId) return false;

            var currentTurnPlayer = await _playerRepository.GetByIdAsync(game.CurrentPlayerId);
            if (currentTurnPlayer == null || currentTurnPlayer.Id != player.Id)
            {
                throw new InvalidOperationException("It's not this player's turn to end.");
            }

            var gamePhases = (await _gameLogRepository.GetAllGamePhasesAsync()).ToList();
            var currentPhase = gamePhases.FirstOrDefault(gp => gp.Id == game.CurrentPhaseId);
            if (currentPhase == null) return false;

            // Determine next phase
            int nextPhaseId = gamePhases.First(gp => gp.Name == "Untap").Id; // Default to start of next turn
            int nextTurnUserId = (game.LightPlayerId == currentTurnPlayer.Id) ? game.DarkPlayerId : game.LightPlayerId;
            int nextTurnNumber = game.CurrentTurnNumber + 1; // Assuming a CurrentTurnNumber property on Game

            //TODO: Simplified phase progression: This needs to be a more complex state machine
            // For now, after any phase, it moves to the next player's Untap phase.
            // A more complete system would cycle through Deploy -> Battle -> Force -> End Phase -> next player's Untap

            // Update game state
            game.CurrentPhaseId = nextPhaseId;
            game.CurrentPlayerId = nextTurnUserId;
            // game.CurrentTurnNumber = nextTurnNumber;
            game.LastUpdated = DateTime.UtcNow;
            await _gameRepository.UpdateAsync(game);

            // Log the action
            await _gameLogRepository.AddAsync(new GameLog
            {
                Id = null,
                GameId = gameId,
                Timestamp = DateTime.UtcNow,
                TurnNumber = nextTurnNumber,
                PhaseId = nextPhaseId,
                PlayerId = currentPlayerId,
                ActionId = (await _gameLogRepository.GetAllActionTypesAsync()).First(at => at.Name == "TurnEnded").Id,
                ActionDetails = JsonDocument.Parse($"{{\"PlayerId\": \"{currentPlayerId}\", \"NextTurnUserId\": \"{nextTurnUserId}\"}}")
            });

            return true;
        }
        #endregion

        #region Helper Methods to support game logic
        //TODO: Re-evaluate the placement of the ValidateDeck method (See DeckService.CreateDeckAsync)
        public bool ValidateDeck(IEnumerable<Card> cardsInDeck, int totalCardCount, Dictionary<int, string> allCardSides)
        {
            //TODO: ** Deck.IsValid ** controlled by database trigger
            // Rule: Must have at least 60 cards
            if (totalCardCount < 60)
            {
                return false;
            }

            // Rule: Cannot contain Cards from both Light and Dark sides
            bool hasLight = cardsInDeck.Any(c => c.Side == "Light");
            bool hasDark = cardsInDeck.Any(c => c.Side == "Dark");
            if (hasLight && hasDark)
            {
                return false; // Mixed sides are not allowed
            }

            // Rule: A DECK can contain as many as four of any one Card
            var cardCounts = cardsInDeck.GroupBy(c => c.Id)
                                        .ToDictionary(g => g.Key, g => g.Count());

            foreach (var entry in cardCounts)
            {
                var card = cardsInDeck.First(c => c.Id == entry.Key);
                if (entry.Value > 4)
                {
                    return false; // More than 4 copies of a card
                }
            }
            return true;
        }

        private async Task PopulateDeckIntoGameCards(int gameId, int playerId, Deck deck, int deckZoneId)
        {
            var gameCardsToInsert = new List<GameCard>();
            int sequence = 0; // For deck order

            foreach (var deckCard in deck.DeckCards)
            {
                for (int i = 0; i < deckCard.Quantity; i++)
                {
                    gameCardsToInsert.Add(new GameCard
                    {
                        Id = null,
                        GameId = gameId,
                        CardId = deckCard.CardId,
                        OwnerPlayerId = playerId,
                        ControllerPlayerId = playerId, // Initially, owner is controller
                        CurrentZoneId = deckZoneId,
                        Sequence = sequence++, // Assign sequence in deck
                        Tapped = false,
                        Damage = 0,
                        FaceDown = false
                    });
                }
            }

            // Shuffle the deck (Fisher-Yates shuffle)
            var rng = new Random();
            int n = gameCardsToInsert.Count;
            while (n > 1)
            {
                n--;
                int k = rng.Next(n + 1);
                GameCard value = gameCardsToInsert[k];
                gameCardsToInsert[k] = gameCardsToInsert[n];
                gameCardsToInsert[n] = value;
            }

            // Re-assign sequence after shuffling
            for (int i = 0; i < gameCardsToInsert.Count; i++)
            {
                gameCardsToInsert[i].Sequence = i;
                await _gameCardRepository.AddAsync(gameCardsToInsert[i]); // Add individually after shuffle
            }
        }

        private async Task DrawCardsForPlayerAsync(int gameId, int playerId, int count)
        {
            var deckZoneId = (await _gameCardRepository.GetAllCardZonesAsync()).First(cz => cz.Name == "Deck").Id;
            var handZoneId = (await _gameCardRepository.GetAllCardZonesAsync()).First(cz => cz.Name == "Hand").Id;

            var deckCards = (await _gameCardRepository.GetGameCardsOwnedByPlayerInZoneAsync(gameId, playerId, deckZoneId))
                                .OrderBy(gc => gc.Sequence)
                                .Take(count)
                                .ToList();

            foreach (var cardToDraw in deckCards)
            {
                cardToDraw.CurrentZoneId = handZoneId;
                cardToDraw.Sequence = null; // Hands typically don't maintain explicit order in DB
                await _gameCardRepository.UpdateAsync(cardToDraw);

                var cardDefinition = await _cardRepository.GetByIdAsync(cardToDraw.CardId);
                await _gameLogRepository.AddAsync(new GameLog
                {
                    Id = null,
                    GameId = gameId,
                    Timestamp = DateTime.UtcNow,
                    TurnNumber = 0, // Initial draw, before turn 1
                    PhaseId = (await _gameLogRepository.GetAllGamePhasesAsync()).First(gp => gp.Name == "Untap").Id,
                    PlayerId = playerId,
                    ActionId = (await _gameLogRepository.GetAllActionTypesAsync()).First(at => at.Name == "CardDrawn").Id,
                    ActionDetails = JsonDocument.Parse($"{{\"GameCardId\": \"{cardToDraw.Id}\", \"CardName\": \"{cardDefinition?.Name ?? "Unknown Card"}\", \"InitialDraw\": true}}")
                });
            }
            // Re-sequence remaining cards in deck after drawing
            var remainingDeckCards = (await _gameCardRepository.GetGameCardsOwnedByPlayerInZoneAsync(gameId, playerId, deckZoneId))
                                .OrderBy(gc => gc.Sequence)
                                .ToList();
            for (int i = 0; i < remainingDeckCards.Count; i++)
            {
                remainingDeckCards[i].Sequence = i;
                await _gameCardRepository.UpdateAsync(remainingDeckCards[i]);
            }
        }

        private async Task<PlayerStateResponse> GetPlayerStateResponse(Player player)
        {
            var cardsInHandCount = (await _gameCardRepository.GetGameCardsOwnedByPlayerInZoneAsync(
                player.GameId, player.Id, (await _gameCardRepository.GetAllCardZonesAsync()).First(cz => cz.Name == "Hand").Id)).Count();
            var cardsInDeckCount = (await _gameCardRepository.GetGameCardsOwnedByPlayerInZoneAsync(
                player.GameId, player.Id, (await _gameCardRepository.GetAllCardZonesAsync()).First(cz => cz.Name == "Deck").Id)).Count();
            var cardsInDiscardCount = (await _gameCardRepository.GetGameCardsOwnedByPlayerInZoneAsync(
                player.GameId, player.Id, (await _gameCardRepository.GetAllCardZonesAsync()).First(cz => cz.Name == "Discard").Id)).Count();

            var user = await _userRepository.GetByIdAsync(Guid.Parse(player.UserId));

            return new PlayerStateResponse
            {
                PlayerId = player.Id,
                UserId = Guid.Parse(player.UserId),
                Username = player.Username, // string.Concat(user?.FirstName, ' ', user?.LastName).Trim(),
                ForceTotal = player.ForceTotal,
                CardsInHandCount = cardsInHandCount,
                CardsInDeckCount = cardsInDeckCount,
                CardsInDiscardCount = cardsInDiscardCount
            };
        }
        #endregion
    }
}