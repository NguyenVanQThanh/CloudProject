import { bootstrapApplication } from '@angular/platform-browser';
import { importProvidersFrom } from '@angular/core';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptorsFromDi } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
import { provideToastr } from 'ngx-toastr';
import { provideAnimations } from '@angular/platform-browser/animations';
import { errorInterceptor } from './app/_interceptors/error.interceptor';

const additionalProviders = [
  importProvidersFrom(BsDropdownModule.forRoot()),
  provideHttpClient(withInterceptorsFromDi()),
  {provide: HTTP_INTERCEPTORS, useClass: errorInterceptor, multi: true},
  provideRouter([]),
  provideAnimations(),
  provideToastr({
      positionClass: 'toast-bottom-right',
      timeOut: 1000,
      progressBar: true,
      progressAnimation: 'decreasing',
    }) // Toastr providers,
];

bootstrapApplication(AppComponent, { ...appConfig, providers: [...appConfig.providers, ...additionalProviders] })
  .catch((err) => console.error(err));
