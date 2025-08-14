import { Injectable, signal } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { HttpResponse } from '@angular/common/http';
import { BaseService } from './base.service';

export interface DeckItem {
  /* DeckItemRequest.cs */
  deckId: number;
  cardId: number;
  quantity: number;
}

@Injectable({
  providedIn: 'root'
})
export class DeckBuilderService extends BaseService {
  //NOTE: /StarWarsTcg/StarWarsTcgApi/StarWarsTcgApi.Application/Services/GenericDeckItemService.cs
  constructor(private http: HttpClient) { super('deckbuilders'); }
  getNextDeckId() {
    return this.http.get<number>(`${this.apiUrl}/deck/next`).toPromise();
  }
  addItem(deckItem: DeckItem){
    return this.http.post<any>(`${this.apiUrl}`, deckItem, { observe: 'response'}).pipe(
      tap((response: HttpResponse<any>) => {
        
      }),
      map((response: HttpResponse<any>) => {
        return response;
      }),
      catchError((error) => {
        console.error('DeckBuilder.AddItem error => ', error);
        return of(new HttpResponse({ status: error.status || 500, statusText: error.statusText || 'Internal Server Error' }));
      })
    );
  }
  removeDeck(deckid: number) {
    console.log('deck-builder > removeDeck(' , deckid , ')');
    return this.http.delete<any>(`${this.apiUrl}/${deckid}`, { observe: 'response'}).pipe(
      tap((response: HttpResponse<any>) => {
        console.log('DeckBuilder.removeDeck response => ', response);
        
      }),
      map((response: HttpResponse<any>) => {
        //return the full HttpResponse
        return response;
      }),
      catchError((error) => {
        console.error('DeckBuilder.AddItem error => ', error);
        return of(new HttpResponse({ status: error.status || 500, statusText: error.statusText || 'Internal Server Error' }));
      })
    );
  }
  removeItem(deckItem: DeckItem){
      this.http.delete<any>(`${this.apiUrl}/${deckItem.deckId}/${deckItem.cardId}`)
      .pipe(
        catchError(this.handleError)
      )
      .subscribe({
        next: (response) => {
          console.log('Delete successful', response);
        },
        error: (err) => {
          console.error('DeckBuilder.RemoveItem error => ', err);
          return of(new HttpResponse({ status: err.status || 500, statusText: err.statusText || 'Internal Server Error' }));
        },
        complete: () => {
          console.log('DeckBuilder.RemoveItem request completed.');
        }
      });
  }

}
