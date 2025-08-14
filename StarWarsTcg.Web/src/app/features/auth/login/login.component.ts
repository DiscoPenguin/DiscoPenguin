import { Component, EventEmitter, Output } from '@angular/core';
import { Router } from '@angular/router';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { SharedModule } from '../../../shared/shared.module';
import { BusyIfDirective } from '../../../shared/directives/app-busy.directive';
import { AuthService, LoginCredentials } from '../../../core/services/auth.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';

@Component({
  imports: [CommonModule, SharedModule, BusyIfDirective, FormsModule],
  selector: 'app-login',
  templateUrl: './login.component.html',
  styleUrls: ['./login.component.css']
})
export class LoginComponent {
  @Output() loginSuccess = new EventEmitter<void>();

  username: string = '';
  password: string = '';
  errorMessage: string = '';
  isLoggingIn: boolean = false;

  constructor(
    private authService: AuthService, 
    private localStorageService: LocalStorageService,
    private router: Router
  ) {}

  login() {
    if (this.username && this.password) {
      this.isLoggingIn = true;
      const credentials : LoginCredentials = {
        email: this.username,
        password: this.password
      };
      this.authService.login(credentials).subscribe({
        next: (response) => {
          if (response.status === 200) {
            this.username = '';
            this.password = '';
            this.router.navigate(['/welcome']);
            this.loginSuccess.emit();
            this.isLoggingIn = false;
          }
          else {
            this.errorMessage = 'Invalid username or password.';
          }
        },
        error: (error) => {
console.error('Login error:', error);
          this.errorMessage = 'An error occurred during login. Please try again later.';
        }
      });
    } else {
      this.errorMessage = 'Please enter valid credentials.';
    }
    this.isLoggingIn = false;
  }

}
