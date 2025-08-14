namespace StarWarsTcgApi.Domain.Models
{
    public class Game
    {
        public required int? Id { get; set; }
        public required int LightPlayerId { get; set; }
        public required int DarkPlayerId { get; set; }
        public required int? WinnerPlayerId { get; set; }
        public required int CurrentTurnNumber { get; set; } = 0;
        public required int CurrentPlayerId { get; set; }
        public required int CurrentPhaseId { get; set; }
        public required int CurrentStatusId { get; set; }
        public DateTime LastUpdated { get; set; }
        public DateTime CreatedAt { get; set; }

        public Game()
        {
            CurrentStatusId = 0; // New -- GameStatus.New
            CurrentTurnNumber = 0;
            CurrentPhaseId = 0;
            CreatedAt = DateTime.UtcNow;
        }
        public void AdvancePhase()
        {
            //TODO: Logic to switch the current player, increment the Turn number
            // Increment by one, cycle until game_phase.orderby = CurrentPhaseId, 
            // [Phase (OrderBy)] MOD 10 == 0 indicates a new game phase, otherwise this phase is the next step
            // e.g., {10 Start, 13 Shuffle Deck, 15 Mulligan}, {30 Ready, 31 Untap, 32 Gain Force}, etc.
            // see: swtcg.game_phase
            // see: StarWarsTcgApi.Domain/Models/GamePhase.cs
        }
        public void SetWinner(int playerId)
        {
            WinnerPlayerId = playerId;
            CurrentStatusId = 999; //End of Game -- GameStatus.Finished
            CurrentPhaseId = 999;
            LastUpdated = DateTime.UtcNow;
        }
    }
}
