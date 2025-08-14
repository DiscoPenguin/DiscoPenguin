import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable } from 'rxjs';
import { Card } from '../../models/Card';
import { BaseService } from './base.service';

export interface PagedResultDto<T> {
  items: T[];
  totalCount: number;
  pageNumber: number;
  pageSize: number;
}

@Injectable({
  providedIn: 'root'
})
export class CardService extends BaseService {
  constructor(private http: HttpClient) { 
    super('Card');
  }

  //TODO: Add more search terms
  searchCards(criteria: any) {
    let params = new HttpParams()
      .set('searchTerm', criteria.searchTerm)
      .set('useLikeness', criteria.useLikeness)
      .set('sortField', criteria.sortField)
      .set('sortDirection', criteria.sortDirection)
      .set('pageNumber', criteria.pageNumber)
      .set('pageSize', criteria.pageSize);

      return this.http.get<PagedResultDto<any>>(`${this.apiUrl}/search`, { params });
  }
  getCards(page: number, pageSize: number) {
    const params = new HttpParams()
      .set('page', page.toString())
      .set('pageSize', pageSize.toString());

    return this.http.get<any>(`${this.apiUrl}/all`, { params });
  }
  getFrequentCards() {
    return this.http.get<any>(`${this.apiUrl}/frequent`);
  }
  getCardById(id: number): Observable<Card>{
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
}
