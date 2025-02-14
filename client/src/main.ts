import { TimeagoModuleConfig } from './../node_modules/ngx-timeago/timeago.module.d';
import { bootstrapApplication } from '@angular/platform-browser';
import { importProvidersFrom } from '@angular/core';
import { HTTP_INTERCEPTORS, provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { provideRouter } from '@angular/router';
import { BsDropdownModule } from 'ngx-bootstrap/dropdown';
import { AppComponent } from './app/app.component';
import { appConfig } from './app/app.config';
import { provideToastr } from 'ngx-toastr';
import { provideAnimations } from '@angular/platform-browser/animations';
import { errorInterceptor } from './app/_interceptors/error.interceptor';
import { JwtInterceptor } from './app/_interceptors/jwt.interceptor';
import { NgxSpinnerModule } from 'ngx-spinner';
import { LoadingInterceptor } from './app/_interceptors/loading.interceptor';
import { BsDatepickerModule } from 'ngx-bootstrap/datepicker'
import { PaginationModule } from 'ngx-bootstrap/pagination'
import { ButtonsModule } from 'ngx-bootstrap/buttons'
import { TimeagoCustomFormatter, TimeagoModule } from "ngx-timeago";
import { ModalModule } from 'ngx-bootstrap/modal';


const additionalProviders = [
  importProvidersFrom(BsDropdownModule.forRoot()),
  provideHttpClient(withInterceptorsFromDi()),
  {provide: HTTP_INTERCEPTORS, useClass: errorInterceptor, multi: true},
  {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
  provideHttpClient(withInterceptors([JwtInterceptor])),
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
  })),
  importProvidersFrom(BsDatepickerModule.forRoot(), ModalModule.forRoot()),
  importProvidersFrom(PaginationModule.forRoot()),
  importProvidersFrom(ButtonsModule.forRoot()),
  importProvidersFrom(TimeagoModule.forRoot()),
];

bootstrapApplication(AppComponent, { ...appConfig, providers: [...appConfig.providers, ...additionalProviders] })
  .catch((err) => console.error(err));
