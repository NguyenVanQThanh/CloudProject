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
import { JwtInterceptor } from './app/_interceptors/jwt.interceptor';
import { NgxSpinnerModule } from 'ngx-spinner';
import { LoadingInterceptor } from './app/_interceptor/loading.interceptor';


const additionalProviders = [
  importProvidersFrom(BsDropdownModule.forRoot()),
  provideHttpClient(withInterceptorsFromDi()),
  {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
  {provide: HTTP_INTERCEPTORS, useClass: errorInterceptor, multi: true},
  {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
  provideRouter([]),
  provideAnimations(),
  provideToastr({
      positionClass: 'toast-bottom-right',
      timeOut: 1000,
      progressBar: true,
      progressAnimation: 'decreasing',
    }), // Toastr providers,
  importProvidersFrom(NgxSpinnerModule.forRoot({
    type: 'line-scale-party'
  }))
];

bootstrapApplication(AppComponent, { ...appConfig, providers: [...appConfig.providers, ...additionalProviders] })
  .catch((err) => console.error(err));
