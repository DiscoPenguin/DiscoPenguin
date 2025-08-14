export interface Deck {
  id: number | null;
  name: string;
  description: string | null;
  createdBy: string | undefined;
  createdAt: Date;
  lastUpdated: Date;
  isValid: boolean;
  isPublic: boolean;
  // Optional
  deckCards?: DeckItem[];
}

export interface DeckItem {
  type: 'DeckCard' | 'DeckBuilder';
  id: string; 
  deckId: number;
  cardId: string;
  quantity: number;
}