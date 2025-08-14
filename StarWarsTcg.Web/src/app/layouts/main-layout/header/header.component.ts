import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router} from '@angular/router'
import { RouterModule } from '@angular/router';
import { AuthService } from '../../../core/services/auth.service';
import { CommonModule } from '@angular/common';

import { LocalStorageService } from '../../../core/services/local-storage.service';
import { LoginComponent } from "../../../features/auth/login/login.component";
import { RegisterComponent } from '../../../features/auth/register/register.component';

@Component({
  imports: [CommonModule, LoginComponent, RegisterComponent, RouterModule],
  selector: 'app-header',
  templateUrl: './header.component.html',
  styleUrls: ['./header.component.css']
})
export class HeaderComponent implements OnInit {
  _authService: AuthService;
  userAvatar: string | undefined;

  constructor(
    private readonly route: ActivatedRoute, 
    private readonly router: Router, 
    public readonly authService: AuthService,
    public readonly localStorageService: LocalStorageService,
  ) {
    this._authService = authService;
  }
  
  ngOnInit(){
  }

  isLoggedIn(): boolean {
    return this.authService.isAuthenticated();
  }

  logout(): void {
    this.authService.logout();
    this.router.navigate(['/welcome']);
  }

  openModal(modalType: 'login' | 'register') {
    const modal = document.getElementById(modalType + 'Modal');
    if (modal) {
      modal.classList.add('show');
      modal.style.display = 'block';
      modal.setAttribute('aria-hidden', 'false');
    }
  }
  closeModal(modalType: 'login' | 'register', event?: MouseEvent) {
    const modal = document.getElementById(modalType + 'Modal');
    if (modal) {
      modal.classList.remove('show');
      modal.style.display = 'none';
      modal.setAttribute('aria-hidden', 'true');
    }
  }
  
}
