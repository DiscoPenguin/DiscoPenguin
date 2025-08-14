import { ApplicationConfig, provideZoneChangeDetection } from '@angular/core';
import { provideRouter } from '@angular/router';

import { Routes } from '@angular/router';
import { routes } from './app-routing.module';

import { WelcomeComponent } from './features/welcome/welcome.component';
import { LoginComponent } from './features/auth/login/login.component';
import { RegisterComponent } from './features/auth/register/register.component';
import { DashboardComponent } from './features/admin/dashboard/dashboard.component';
import { AuthenticationGuard } from './core/guards/auth.guard';

import { provideHttpClient, withFetch } from '@angular/common/http';
import { provideClientHydration, withEventReplay } from '@angular/platform-browser';


export const appConfig: ApplicationConfig = {
  providers: [
    provideZoneChangeDetection({ eventCoalescing: true }), 
    provideRouter(routes),
    provideHttpClient(withFetch()),
    provideClientHydration(withEventReplay())
  ]
};

