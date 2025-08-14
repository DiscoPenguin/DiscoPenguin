import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { RouterLink, RouterOutlet, RouterLinkActive } from '@angular/router';
import { HeaderComponent } from './layouts/main-layout/header/header.component';
import { FooterComponent } from './layouts/main-layout/footer/footer.component';
import { BusyIfDirective } from './shared/directives/app-busy.directive';

@Component({
  selector: 'app-root',
  imports: [
    CommonModule, 
    RouterLink, 
    RouterOutlet, 
    RouterLinkActive,
    HeaderComponent,
    FooterComponent,
    BusyIfDirective
  ],
  templateUrl: './app.component.html',
  styleUrl: './app.component.css'
})
export class AppComponent {
  title = 'StarWarsTcg.Web';
}
