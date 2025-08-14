import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { HttpResponse } from '@angular/common/http';
import { BaseService } from './base.service';

@Injectable({
  providedIn: 'root'
})
export class GameService extends BaseService {
  constructor(private http: HttpClient) {
    super('Games');
  }

  getGameStatistics(userId: string | undefined) {
    let params = new HttpParams();
    if (userId) {
      return this.http.get<any>(`${this.apiUrl}/?userId=${userId}`);
    }
    return this.http.get<any>(`${this.apiUrl}/?userId=`);

  }
  
}
