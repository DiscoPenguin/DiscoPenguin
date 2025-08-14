import { Component, OnInit } from '@angular/core';
import { ActivatedRoute, Router, RouterModule } from '@angular/router'
import { CommonModule } from '@angular/common';

import { AuthService } from '../../core/services/auth.service';
import { LeaderboardComponent } from '../leaderboard/leaderboard.component';
import { FrequentCardsComponent } from "../frequent-cards/frequent-cards.component";
import { CardSearchComponent } from "../cards/card-search/card-search.component";
import { CardDisplayComponent } from "../../shared/components/card-display/card-display.component";
import { Card } from '../../models/Card';

@Component({
  selector: 'app-welcome',
  imports: [CommonModule, RouterModule, LeaderboardComponent, FrequentCardsComponent],
  templateUrl: './welcome.component.html',
  styleUrl: './welcome.component.css'
})
export class WelcomeComponent {

  constructor(
    private readonly route: ActivatedRoute, 
    private readonly router: Router, 
    public readonly authService: AuthService
  ) {
  }

  login() {
    this.router.navigate(['/login'], {relativeTo:this.route});
  }
  logout() {
    this.authService.logout();
    this.router.navigate(['/'], {relativeTo:this.route});
  }

}
