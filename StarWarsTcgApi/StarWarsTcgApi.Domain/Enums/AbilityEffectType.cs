namespace StarWarsTcgApi.Domain.Enums
{
    public enum AbilityEffectType
    {
        None = 0,
        ModifyPower = 1, // Change card's power
        ModifyDefense = 2, // Change card's defense/armor
        ModifyForfeit = 3, // Change card's forfeit value
        DrawCards = 4,
        GenerateForce = 5,
        LoseForce = 6,
        DealDamage = 7, // Deal direct damage to opponent/card/player
        HealDamage = 8,
        MoveCard = 9, // Move card between zones (hand, reserve, used, lost)
        CancelAbility = 10, // Negate another ability
        DestroyCard = 11,
        StunCard = 12, // Prevent a card from acting
        SearchDeck = 13,
        DiscardCards = 14,
        RevealCards = 15,
        GainKeyword = 16, // Temporarily or permanently grant a keyword
        RemoveKeyword = 17, // Temporarily or permanently remove a keyword
        TargetCard = 18, // Select a target for another effect
        SetAttribute = 19, // Set a specific attribute (e.g., Power becomes X)
        PreventDamage = 20,
        ReflectDamage = 21,
        GrantImmunity = 22,
        RemoveImmunity = 23,
        TakeControlOfCard = 24,
        ShuffleDeck = 25,
        SacrificeCard = 26,
        SwapCards = 27,
        FlipCard = 28,
        ApplyModifier = 29, // Generic modifier application
    }
}