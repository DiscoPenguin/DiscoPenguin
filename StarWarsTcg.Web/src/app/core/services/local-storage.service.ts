/* Sample usage in a component:
    import { Component } from '@angular/core';
    import { LocalStorageService } from '../../core/services/local-storage.service';

    @Component({
        selector: 'app-register',
        templateUrl: './register.component.html',
        styleUrls: ['./register.component.css']
    })
    export class RegisterComponent {
        constructor(private localStorageService: LocalStorageService) {}

        someMethod() {
            this.localStorageService.setItem('key', 'value');
        }
    }
*/
import { Injectable, Inject, PLATFORM_ID } from '@angular/core';
import { isPlatformBrowser } from '@angular/common';

@Injectable({
  providedIn: 'root'
})
export class LocalStorageService {
  constructor(@Inject(PLATFORM_ID) private platformId: Object) {}

  setItem(key: string, value: string): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.setItem(key, value);
    }
  }

  getItem(key: string): string | null {
    if (isPlatformBrowser(this.platformId)) {
      return localStorage.getItem(key);
    }
    return null;
  }

  removeItem(key: string): void {
    if (isPlatformBrowser(this.platformId)) {
      localStorage.removeItem(key);
    }
  }
  clear(): void {
    localStorage.clear();
  }
}
