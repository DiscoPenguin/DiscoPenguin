import { Injectable } from '@angular/core';
import { HttpErrorResponse } from '@angular/common/http';
import { Observable, throwError } from 'rxjs';
import { environment } from '../../../environments/environment';

/**
 * An abstract base service for HTTP client services.
 * Provides common functionality like error handling.
 */
@Injectable() // Needs @Injectable() if it will inject other services in the future,
              // or if it's meant to be extended by other services that are @Injectable()
export abstract class BaseService {
  protected apiUrl: string;
  constructor(resourcePath: string) {
    this.apiUrl = `${environment.apiBaseUrl}/${resourcePath}`;
  }
  /**
   * Generic error handling for HTTP requests.
   * This method is designed to be inherited and used by derived services.
   *
   * @param error The HttpErrorResponse received from the API.
   * @returns An Observable that emits an error.
   */
  protected handleError(error: HttpErrorResponse): Observable<never> {
    let errorMessage = 'An unknown error occurred!';

    if (error.error instanceof ErrorEvent) {
      // Client-side errors or network errors
      errorMessage = `Client Error: ${error.error.message}`;
    } else {
      // Server-side errors
      // The backend might return a specific error message in error.error
      errorMessage = `Server Error: ${error.status} - ${error.statusText || ''}`;
      if (error.error) {
        if (typeof error.error === 'string') {
          // Simple string error from backend
          errorMessage += `\nBackend Message: ${error.error}`;
        } else if (error.error instanceof Object) {
          // Structured error object from backend (e.g., validation errors)
          // You might need to adjust this based on your backend's error format
          errorMessage += `\nBackend Details: ${JSON.stringify(error.error)}`;
        }
      }
    }
    console.error(`HTTP Error caught by BaseService: ${errorMessage}`); // Log the error for debugging
    // It's good practice to re-throw the error as a new Error object
    // so that subscribers (e.g., components) can catch it with a clear message.
    return throwError(() => new Error(errorMessage));
  }
}