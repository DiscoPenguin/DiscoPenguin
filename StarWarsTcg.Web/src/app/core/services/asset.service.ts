import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { BaseService } from './base.service';

export interface Asset {
  id: number;
  name: string;
  url: string;
  imageType: string;
}

@Injectable({
  providedIn: 'root'
})
export class AssetService extends BaseService {

  constructor(private http: HttpClient) { 
    super('Assets');
  }
  getAssets() {
    return this.http.get<any>(`${this.apiUrl}/all`);
  }
  getAssetById(id: number): Observable<Asset> {
    return this.http.get<any>(`${this.apiUrl}/${id}`);
  }
  /*
  getAssets(): Observable<Asset[]> {
    return this.http.get<string[]>(this.apiUrl).pipe(
      map(imageUrls => imageUrls.map(url => ({
        name: url.substring(url.lastIndexOf('/') + 1), // Extract filename from URL
        url: url
      })))
    );
  }
  */
}