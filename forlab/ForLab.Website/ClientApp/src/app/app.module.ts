import { BrowserModule } from '@angular/platform-browser';
import { NgModule } from '@angular/core';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { HttpClientModule } from '@angular/common/http';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
// Services
import { DatePipe } from '@angular/common';
import { HttpService } from 'src/@core/services/http.service';
import { AlertService } from 'src/@core/services/alert.service';
// Core Module
import { CoreModule } from 'src/@core/core.module';
import { LayoutModule } from 'src/app/+layout/layout.module';

@NgModule({
  declarations: [
    AppComponent,
  ],
  imports: [
    BrowserAnimationsModule,
    HttpClientModule,
    CoreModule,
    BrowserModule,
    AppRoutingModule,
    // Modules
    LayoutModule
  ],
  providers: [
    // Services
    HttpService,
    AlertService,
    DatePipe
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
