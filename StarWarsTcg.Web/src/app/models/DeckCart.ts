import { Card } from "./Card";

export interface CardItem {
  cardId: number;
  quantity: number;
}
export interface DeckCart {
  items: CardItem[];
  DeckName: string;

  // Optional properties
  averageCost?: number;
  averageSpeed?: number;
  averagePower?: number;
  averageHealth?: number;
  numberOfCards?: number;
  totalPointValue?: number;

  fetchCard(cardId: number): Card | null;
}

// Implementation of DeckCart
class DeckOfCards implements DeckCart {
  items: CardItem[];
  deckName: string;

  constructor(deckName: string, items: CardItem[]) {
    this.deckName = deckName;
    this.items = items;
  }

  // Method to fetch a Card by CardId
  fetchCard(cardId: number): Card | null {
    const card = cardDatabase.find(card => card.cardId === cardId);
    return card ? card : null; // Return the card or null if not found
  }
}

export const emptyDeck: CardItem[] = [
  {
    cardId: -1,
    quantity: 0
  }
];
