import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { AppRoutingModule, routingComponents } from './app-routing.module';
import { AppComponent } from './app.component';
import { FormsModule } from '@angular/forms';

import { HeaderComponent } from './layouts/main-layout/header/header.component';

import { AuthModule } from './features/auth.module';
import { AuthService } from './core/services/auth.service';
import { AuthGuard } from './core/guards/auth.guard';
import { SharedModule } from './shared/shared.module';

@NgModule({
  declarations: [
  ],
  imports: [
    AppComponent,
    FormsModule,
    AuthModule,
    HeaderComponent,
    AppRoutingModule,
    routingComponents,
    SharedModule
  ],
  exports: [
  ],
  providers: [AuthService, AuthGuard],
//  bootstrap: [AppComponent]
})
export class AppModule { }
