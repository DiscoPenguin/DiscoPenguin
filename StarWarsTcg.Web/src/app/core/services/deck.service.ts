import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { HttpResponse } from '@angular/common/http';
import { BaseService } from './base.service';
import { Deck } from '../../models/Deck';
import { DeckBuilderService } from './deck-builder.service';

export interface CardItem {
  cardId: number;
  quantity: number;
}
export interface DeckCart {
  items: CardItem[];
  DeckName: string;
  DeckId: number;
  
  // Optional properties
  averageCost?: number;
  averageSpeed?: number;
  averagePower?: number;
  averageHealth?: number;
  numberOfCards?: number;
  totalPointValue?: number;
}

export const emptyDeck: CardItem[] = [
  {
    cardId: -1,
    quantity: 0
  }
];

@Injectable({
  providedIn: 'root'
})
export class DeckService extends BaseService {
  private _deckBuilderService! : DeckBuilderService;
  public nextDeckBuilderId: number = -1;
  
  cart = signal<DeckCart>({
    items: emptyDeck,
    DeckName: 'My Deck',
    DeckId: -1
  });  

  constructor(
    private http: HttpClient ,
    private readonly deckBuilderService: DeckBuilderService 
  ) {
    super('Decks');
    this._deckBuilderService = deckBuilderService;
    this.initializeDeckId();
  }

  private async initializeDeckId() {
    try {
      const nextDeckId = await this._deckBuilderService.getNextDeckId();
      if (typeof nextDeckId === 'number') {
        this.nextDeckBuilderId = nextDeckId;
        this.cart.update(currentCart => ({
          ...currentCart,
          DeckId: nextDeckId
        }));
      } else {
        console.warn('getNextDeckId() return undefined or a non-numeric value');
      }
    }
    catch(error) {
      console.error('Failed to retrieve next DeckId:', error);
    }
  }

  addItem(item: CardItem) {
    this.cart.update((currentCart) => {
      currentCart.items = currentCart.items.filter(c => c.cardId >= 0);
      const existingCard = currentCart.items.find(c => c.cardId == item.cardId);

      if (existingCard) {
        existingCard.quantity += item.quantity;
        if (existingCard.quantity > 4) { existingCard.quantity = 4;}
      } else {
        currentCart.items.push(item);
      }

      this._deckBuilderService.addItem({ deckId: this.cart().DeckId, cardId: item.cardId, quantity: item.quantity })

      // perform calculations
      return currentCart;
    });
  }
  emptyCart() {
    this.cart.update((currentCart) => {
      currentCart.items = emptyDeck;
      currentCart.DeckName = 'My Deck';
      //TODO: Persist via DeckBuilderService.removeDeck(deckId)

      // perform calculations
      return currentCart;
    })
  }
  removeItem(item: CardItem) {
    this.cart.update((currentCart) => {
      currentCart.items = currentCart.items.filter(c => c.cardId >= 0);
      const existingCard = currentCart.items.find(c => c.cardId == item.cardId);
      if (existingCard) {
        existingCard.quantity--;
        if (existingCard?.quantity <= 0) {
          currentCart.items = currentCart.items.filter(c => c.cardId !== existingCard.cardId);
        }
      }
      
      this._deckBuilderService.removeItem({ deckId: this.cart().DeckId, cardId: item.cardId, quantity: item.quantity });
      
      // perform calculations
      return currentCart;
    })
  }

  submitDeck(deck: Deck): Observable<Deck> {
    const deckToCreate = { ...deck, id: null };
    console.log('submitDeck => ', deckToCreate);
    //TODO: Persist this deck from [swtcg.DeckBuilder] to [swtcg.Deck and swtcg.Deck_Cards]
    return this.http.post<Deck>(this.apiUrl, deckToCreate).pipe(
      map((response: Deck) => {
        console.log('Deck created successfully');
        response.createdAt = new Date(response.createdAt);
        response.lastUpdated = new Date(response.lastUpdated);
        return response;
      }),
      catchError(this.handleError)
    )
  }
}
