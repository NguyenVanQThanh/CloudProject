
import { CommonModule } from '@angular/common';
import { HttpClient, provideHttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
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
@Component({
    selector: 'app-root',
    standalone: true,
    templateUrl: './app.component.html',
    styleUrls: ['./app.component.css'],
    imports: [RouterOutlet, CommonModule, NavComponent,HomeComponent,
      RegisterComponent, NgbDropdownModule,ToastrModule],
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
    provideHttpClient(),
    provideAnimations(), // required animations providers
    provideToastr({
      positionClass: 'toast-bottom-right',
    }), // Toastr providers
  ]
});
