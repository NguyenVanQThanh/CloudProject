import { bootstrapApplication } from '@angular/platform-browser';
import { importProvidersFrom } from '@angular/core';
import { provideHttpClient } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
import { provideToastr } from 'ngx-toastr';
import { provideAnimations } from '@angular/platform-browser/animations';

const additionalProviders = [
  importProvidersFrom(BsDropdownModule.forRoot()),
  provideHttpClient(),
  provideRouter([]),
  provideAnimations(),
  provideToastr(),
];

bootstrapApplication(AppComponent, { ...appConfig, providers: [...appConfig.providers, ...additionalProviders] })
  .catch((err) => console.error(err));
