import { Injectable } from '@angular/core';
import { HttpClient, HttpParams } from '@angular/common/http';
import { Observable, of } from 'rxjs';
import { tap, catchError, map } from 'rxjs/operators';
import { HttpResponse } from '@angular/common/http';
import { LocalStorageService } from './local-storage.service';
import { EMPTY_GUID, isEmptyGuid } from '../../shared/constants/app.constants';
import { BaseService } from './base.service';
import { AssetService } from './asset.service';

@Injectable({
  providedIn: 'root'
})
export class AuthService extends BaseService {
  
  constructor(
    private http: HttpClient, 
    private assetService: AssetService,
    private localStorageService : LocalStorageService) {
      super('Auth');
    }
  // https://danielk.tech/home/angular-login-page-and-complete-authentication-demo

  isAuthenticated(): boolean {
    return (this.localStorageService.getItem("token") != null);
  }

  private async getAvatarUrl(avatarId: number) {
    let assetUrl: string = '';
      let asset = this.assetService.getAssetById(avatarId).subscribe({
        next: (response: any) => { 
          assetUrl = response.url;
          this.localStorageService.setItem("avatarUrl", JSON.stringify(assetUrl));
        },
        error: (e) => console.error(e),
      });
  }

  getAuthenticatedUser(): UserInfo | undefined {
    const token = this.localStorageService.getItem("token");
    if (token) {
      const id = this.localStorageService.getItem("id");
      const userName : string | null = this.localStorageService.getItem("userName");
      const avatarId : number = +(this.localStorageService.getItem("avatarId") ?? 0);
      let avatarUrl: string | null = this.localStorageService.getItem("avatarUrl");
      return {
          id: id ?? EMPTY_GUID,
          userName: userName ?? '',
          avatarId: avatarId,
          avatarUrl: (avatarUrl ?? '').replace(/^["']|["']$/g, '')
        };
    }
    return undefined;
  }
  
  login(credentials: LoginCredentials): Observable<any> {
    return this.http.post<any>(this.apiUrl + '/login', credentials, { observe: 'response' }).pipe(
      tap((response: HttpResponse<any>) => {
        if (response.status === 200 && response.body && response.body.token) {
          console.log('Login successful:', response.body);
          this.localStorageService.setItem("token", response.body.token);
          this.localStorageService.setItem("id", JSON.stringify(response.body.id));
          this.localStorageService.setItem("userName", JSON.stringify(response.body.userName));
          this.localStorageService.setItem("avatarId", JSON.stringify(response.body.avatarId));
          this.getAvatarUrl(response.body.avatarId);
        } else {
          console.error('Login failed:', response.body);
          // You might want to throw an error or handle specific non-200 responses here
        }
      }),
      map((response: HttpResponse<any>) => {
        // Return the full HttpResponse
        return response;
      }),
      catchError((error) => {
console.error('Login error:', error);
        // Return an Observable of HttpResponse with the error status
        return of(new HttpResponse({ status: error.status || 500, statusText: error.statusText || 'Internal Server Error' }));
      })
    );
  }

  register(credentials: RegisterCredentials): Observable<any> {
    return this.http.post<any>(this.apiUrl + '/register', credentials, { observe: 'response' }).pipe(
      tap((response: HttpResponse<any>) => {
console.log('Register response:', response);
        if (response.status === 200 && response.body && response.body.token) {
          console.log('Registration successful:', response.body);
          this.localStorageService.setItem("token", response.body.token);
          this.localStorageService.setItem("id", JSON.stringify(response.body.id));
          this.localStorageService.setItem("userName", JSON.stringify(response.body.userName));
          this.localStorageService.setItem("avatarId", JSON.stringify(response.body.avatarId));
          this.getAvatarUrl(response.body.avatarId);
        } else {
          console.error('Registration failed:', response.body);
          // You might want to throw an error or handle specific non-200 responses here
        }
      }),
      map((response: HttpResponse<any>) => {
        // Return the full HttpResponse
        return response;
      }),
      catchError((error) => {
console.error('Registration error:', error);
        // Return an Observable of HttpResponse with the error status
        return of(new HttpResponse({ status: error.status || 500, statusText: error.statusText || 'Internal Server Error' }));
      })
    );
  } 

  logout() {
    this.localStorageService.clear();
    this.localStorageService.removeItem("token");
  }
  
}
export interface UserInfo {
  id: string;
  userName: string;
  avatarId: number;
  avatarUrl: string;
}
export interface LoginCredentials {
  email: string;
  password: string;
}
export interface RegisterCredentials {
  firstname: string;
  lastname: string;
  email: string;
  password: string;
  confirmPassword: string;
  avatarId: number;
}