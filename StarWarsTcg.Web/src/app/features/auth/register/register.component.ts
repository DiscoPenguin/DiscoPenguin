import { Component, EventEmitter, OnInit, Output } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterModule } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { Router } from '@angular/router';

import { AuthService, RegisterCredentials } from '../../../core/services/auth.service';
import { LocalStorageService } from '../../../core/services/local-storage.service';
import { IconSelectorComponent } from "../../admin/users/icon-selector/icon-selector.component";
import { Asset } from '../../../core/services/asset.service';
//import { UserAuthService } from '../user-auth.service';
 
@Component({
  selector: 'app-register',
  imports: [CommonModule, RouterModule, FormsModule, IconSelectorComponent],
  templateUrl: './register.component.html',
  styleUrl: './register.component.css'
})
export class RegisterComponent implements OnInit{
  @Output() loginSuccess = new EventEmitter<void>();

  selectedAvatar: Asset = { id: 0, name: '', url: '', imageType: 'None' };
  firstname:string = ''
  lastname:string = ''
  email:string = ''
  password:string = ''
  confirmPassword:string = ''
  isSubmitting:boolean = false
  validationErrors:any = []
  
  constructor(
//    public userAuthService: UserAuthService, 
    private readonly authService: AuthService, 
    private readonly localStorageService: LocalStorageService,
    private readonly router: Router
  ) {}
  
  ngOnInit(): void {
    if(this.localStorageService.getItem('token') != "" && this.localStorageService.getItem('token') != null){
      this.router.navigateByUrl('/dashboard')
    }
  }
  
  clearForm() {
    this.firstname = '';
    this.lastname = '';
    this.email = '';
    this.password = '';
    this.confirmPassword = '';
    this.validationErrors = [];
    this.isSubmitting = false;
  }
  onIconSelected(event: any) {
    this.selectedAvatar = event;
  }
  registerAction() {
    this.isSubmitting = true;
    const credentials: RegisterCredentials = {
      email: this.email,
      firstname: this.firstname,
      lastname: this.lastname,
      password: this.password,
      confirmPassword: this.confirmPassword,
      avatarId: this.selectedAvatar.id
    };
    this.authService.register(credentials).subscribe({
      next: (response) => {
        if (response.status === 200) {

          this.authService.login(credentials).subscribe({
            next: (response) => {
              if (response.status === 200) {
                this.clearForm();
                this.router.navigate(['/welcome']);
                this.loginSuccess.emit();
              }
              else {
                this.validationErrors = 'Invalid username or password.';
              }
            },
            error: (error) => {
  console.error('Login error:', error);
              this.validationErrors = 'An error occurred during login. Please try again later.';
            }
          });

          this.router.navigateByUrl('/dashboard');
          this.isSubmitting = false;
        } else {
          this.isSubmitting = false;
          this.validationErrors = response.error.errors || [];
        }
      },
      error: (error) => {
        this.isSubmitting = false;
        console.error('Registration error:', error);
        if (error.error && error.error.errors) {
          this.validationErrors = error.error.errors;
        } else {
          this.validationErrors = ['An unexpected error occurred. Please try again later.'];
        }
      }
    });
 /*
    let payload = {
      firstname:this.firstname,
      lastname:this.lastname,
      email:this.email,
      password:this.password,
      confirmPassword:this.confirmPassword
    }
  
    this.userAuthService.register(payload)
    .then(({data}) => {
      this.localStorageService.setItem('token', data.token)
      this.router.navigateByUrl('/dashboard')
      return data
    }).catch(error => {
      this.isSubmitting = false;
      if (error.response.data.errors != undefined) {
        this.validationErrors = error.response.data.errors
      }
       
      return error
    })
*/
  }
}