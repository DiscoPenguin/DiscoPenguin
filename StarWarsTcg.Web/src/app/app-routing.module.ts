import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { WelcomeComponent } from './features/welcome/welcome.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { DashboardComponent } from './features/admin/dashboard/dashboard.component';
import { ProfileComponent } from './features/admin/users/profile/profile.component';
import { AuthenticationGuard } from './core/guards/auth.guard';
import { CardListComponent } from './features/cards/card-list/card-list.component';
import { DeckBuilderComponent } from './features/decks/deck-builder/deck-builder.component';
/* 
  https://www.tektutorialshub.com/angular/angular-child-routes-nested-routes/ 
  https://angular.dev/guide/routing/define-routes
*/
export const routes: Routes = [
  { path: '', component: WelcomeComponent }, // Default route
  { path: 'welcome', component: WelcomeComponent, title: 'Welcome' }, // Default route
  { path: 'login', component: LoginComponent, title: 'Log In' }, // Login route
  { path: 'register', component: RegisterComponent, title: 'Register' }, // Registration route
  {
    path: 'cards/cardlist',
    component: CardListComponent,
    title: 'Card List',
  },
  {
    path : 'decks/deckbuilder',
    component: DeckBuilderComponent,
    title: 'Deck Builder'
  },
  { path: 'admin', component: DashboardComponent, title:'Admin', canActivate: [AuthenticationGuard] }, // Admin route with guard
  { 
    path: 'profile', 
    component: ProfileComponent, 
    title: 'User Profile', 
    canActivate: [AuthenticationGuard] 
  },
  { path: '**', redirectTo: '' } // Redirect any unknown routes to the welcome page
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }

export const routingComponents = [
  WelcomeComponent,
  LoginComponent,
  RegisterComponent,
  DashboardComponent,
  CardListComponent,
  ProfileComponent
];

