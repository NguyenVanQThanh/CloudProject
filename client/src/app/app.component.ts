
import { CommonModule } from '@angular/common';
import { HTTP_INTERCEPTORS, HttpClient, provideHttpClient, withInterceptors, withInterceptorsFromDi } from '@angular/common/http';
import { Component, importProvidersFrom, OnInit } from '@angular/core';
import { RouterOutlet, provideRouter } from '@angular/router';
import { NavComponent } from "./nav/nav.component";
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap'
import { AccountService } from './_services/account.service';
import { User } from './_models/user';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { bootstrapApplication } from '@angular/platform-browser';
import { provideAnimations } from '@angular/platform-browser/animations';

import { provideToastr, ToastrModule } from 'ngx-toastr';
import { routes } from './app.routes';
import { errorInterceptor } from './_interceptors/error.interceptor';
import { NotFoundComponent } from './errors/not-found/not-found.component';
import { JwtInterceptor } from './_interceptors/jwt.interceptor';
import { LoadingInterceptor } from './_interceptor/loading.interceptor';
import { NgxSpinnerModule } from 'ngx-spinner';
import { PaginationModule } from 'ngx-bootstrap/pagination';

@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    imports: [RouterOutlet, CommonModule, NavComponent,HomeComponent,
      RegisterComponent, NgbDropdownModule,ToastrModule,NotFoundComponent,NgxSpinnerModule],
    providers: []
  })
export class AppComponent implements OnInit {
  title = 'client';

  constructor(private http:HttpClient, private accountService : AccountService) {}
  ngOnInit(): void {
    this.setCurrentUser();
  }
  setCurrentUser(){
    const userString = localStorage.getItem('user');
    if (!userString) return;
    const user:User = JSON.parse(userString);
    this.accountService.setCurrentUser(user);
  }
}
bootstrapApplication(AppComponent, {
  providers: [
    provideRouter(routes),
    provideHttpClient(withInterceptorsFromDi()),
    {provide: HTTP_INTERCEPTORS, useClass: errorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: JwtInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
    provideAnimations(), // required animations providers
    provideToastr({
      positionClass: 'toast-bottom-right',
      timeOut: 1000,
      progressBar: true,
      progressAnimation: 'decreasing',
    }), // Toastr providers
    importProvidersFrom(NgxSpinnerModule.forRoot({
      type: 'line-scale-party'
    })),
    importProvidersFrom(PaginationModule.forRoot()),
  ]
});
