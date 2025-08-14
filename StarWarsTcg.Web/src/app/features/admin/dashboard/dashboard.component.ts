import { Component } from '@angular/core';
import { AuthService } from '../../../core/services/auth.service';
import { ActivatedRoute, Router} from '@angular/router'

@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.css']
})
export class DashboardComponent {
  constructor(private route: ActivatedRoute, private router: Router, public authService: AuthService) { }
  
  logout() {
    this.authService.logout();
    this.router.navigate(['/welcome']);
  }

}
