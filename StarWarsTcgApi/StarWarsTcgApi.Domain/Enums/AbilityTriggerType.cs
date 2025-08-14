namespace StarWarsTcgApi.Domain.Enums
{
    public enum AbilityTriggerType
    {
        // General Triggers
        None = 0, // For passive/continuous abilities that don't have a specific trigger phase
        OnPlay = 1, // When the card is played from hand to table
        OnEnterPlay = 2, // When the card enters play (might be different from OnPlay if brought into play by other means)
        OnLeavePlay = 3, // When the card leaves play (e.g., returned to hand, moved to lost pile)
        OnStartTurn = 4,
        OnEndTurn = 5,
        OnCombatInitiated = 6, // Before combat rolls
        OnCombatDamageDealt = 7,
        OnCombatDamageTaken = 8,
        OnDestroyed = 9, // When the card is about to be moved to lost pile
        OnDiscarded = 10, // When moved to lost pile from hand/deck
        OnDrawCard = 11,
        OnForceGenerated = 12,
        OnMoveCard = 13, // Generic trigger for card movement (e.g., to/from hand, reserve, used)
        OnTargeted = 14, // When this card is targeted by an ability/attack
        OnActivation = 15, // When the card is activated (e.g., taps for resource, uses an active ability)
        OnSacrifice = 16, // When the card is sacrificed for an effect

        // Specific to SWTCG Keywords (can be more abstract, or specific)
        OnAttack = 17, // When this card declares an attack
        OnDefend = 18, // When this card is declared as a defender
        BeforeCombat = 19, // Immediately before combat begins
        AfterCombat = 20, // Immediately after combat resolves
        OnOpponentPlayCard = 21,
        OnOpponentDrawCard = 22,
        OnOpponentLoseForce = 23,
        OnForfeit = 24, // When this card is forfeited
        OnReveal = 25, // When this card is revealed from hand/deck
        OnSetup = 26, // At the start of the game, during setup
        OnFlip = 27, // When the card is flipped (e.g., light/dark side)
        Continuous = 28, // For passive abilities that are always active
    }
}