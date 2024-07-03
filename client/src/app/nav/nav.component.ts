import { AccountService } from './../_services/account.service';
import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { FormsModule } from '@angular/forms';
import { NgbDropdownModule } from '@ng-bootstrap/ng-bootstrap';
import { Observable, of } from 'rxjs';
import { User } from '../_models/user';
@Component({
  selector: 'app-nav',
  standalone: true,
  templateUrl: './nav.component.html',
  styleUrl: './nav.component.css',
  imports: [RouterOutlet, CommonModule, NavComponent, FormsModule, NgbDropdownModule],
})
export class NavComponent implements OnInit {
  model: any = {};
  // currentUser$: Observable<User | null> = of(null);
  constructor(public accountService : AccountService) { }
  ngOnInit(): void {
    // this.currentUser$ = this.accountService.currentUser$;
  }
  // getCurrentUser(){
  //   this.accountService.currentUser$.subscribe({
  //     next: user => this.loggedIn = !!user,
  //     error: error => console.log(error)
  //   });
  // }
  login(){
    // Call your login service here
    this.accountService.login(this.model).subscribe({
      next: (response) => {
        console.log(response);
      },
      error: (error) => {
        console.log(error);
      }
    });
  }
  logout(){
    this.accountService.logout();
  }
}

