import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { LoginComponent } from './auth/login/login.component';
import { LeaderboardComponent } from './leaderboard/leaderboard.component';

@NgModule({
  declarations: [
  ],
  imports: [
    CommonModule, 
    FormsModule,
    LoginComponent,
    LeaderboardComponent
  ],
  exports: [
    CommonModule,
    FormsModule,
    LoginComponent
  ]
})
export class AuthModule { }
