import { Component, Input, OnInit, TemplateRef } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';

import { AuthService } from '../../core/services/auth.service';
import { GameService } from '../../core/services/game.service';
import { GameStatistics } from '../../models/GameStatistics';

@Component({
  imports: [CommonModule, FormsModule],
  selector: 'app-leaderboard',
  templateUrl: './leaderboard.component.html',
  styleUrls: ['./leaderboard.component.css']
})
export class LeaderboardComponent implements OnInit {
  @Input() userId: string | undefined;
  constructor(
    private gameService: GameService,
    private readonly authService : AuthService
  ) {
  }

  title: string = '';
  leaderboardData: GameStatistics[] = [];
  isLoading : boolean = false;

  ngOnInit(): void {
    this.getStats();
    this.title = this.userId ? 'Your Stats' : 'Leaderboard';
  }

  getStats() {
    this.isLoading = true;
    this.gameService.getGameStatistics(this.userId?.replace(/"/g, '')).subscribe({
      next: (response) => {
        this.leaderboardData = response;
      },
      error: (e) => console.error(e),
      //complete: () => console.info('complete')
    });
    this.isLoading = false;
  }


}
