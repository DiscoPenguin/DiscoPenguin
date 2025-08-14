import { Component } from '@angular/core';
import { ActivatedRoute, Router} from '@angular/router'
import { CommonModule } from '@angular/common';

import { LocalStorageService } from '../../../../core/services/local-storage.service';
import { AuthService } from '../../../../core/services/auth.service';
import { LeaderboardComponent } from '../../../leaderboard/leaderboard.component';
import { IconSelectorComponent } from '../icon-selector/icon-selector.component';

@Component({
  selector: 'app-profile',
  imports: [ CommonModule, LeaderboardComponent, IconSelectorComponent ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.css'
})
export class ProfileComponent {
  constructor(
    private readonly route: ActivatedRoute, 
    private readonly router: Router, 
    public readonly authService: AuthService,
    public readonly localStorageService: LocalStorageService
  ) {
  }
  onIconSelected(event: any) {
    //TODO: Apply newly selected avatar to the User profile
    console.log('onIconSelected => ', event);
  }
}
