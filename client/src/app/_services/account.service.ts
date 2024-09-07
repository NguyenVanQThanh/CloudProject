import { HttpClient } from '@angular/common/http';
import { computed, Injectable, signal } from '@angular/core';
import { BehaviorSubject, map } from 'rxjs';
import { User } from '../_models/user';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  baseUrl = environment.apiUrl;
  // private currentUserSource = new BehaviorSubject<User | null>(null);
  // currentUser$ = this.currentUserSource.asObservable();
  currentUser = signal<User | null> (null);
  roles = computed(()=>{
    const user = this.currentUser();
    if (user && user.token){
      const role =  JSON.parse(atob(user.token.split('.')[1])).role
      return Array.isArray(role) ? role : [role];
    }
    return [];
  })
  constructor(private http: HttpClient) {}
  login(model: any){
    return this.http.post<User>(`${this.baseUrl}account/login`, model).pipe(
      map((response: User) => {
        // store user details and jwt token in local storage to keep user logged in between page refreshes
        const user = response;
        if (user){
          this.setCurrentUser(user);
        }
      })
    );
  }
  register(model: any){
    return this.http.post<User>(this.baseUrl + 'account/register', model).pipe(
      map((response: User) => {
        // store user details and jwt token in local storage to keep user logged in between page refreshes
        const user = response;
        if (user){
          this.setCurrentUser(user);
        }
        // return user;
      })
    )
  }
  setCurrentUser(user: User){
    localStorage.setItem('user', JSON.stringify(user));
    this.currentUser.set(user);
    // this.like
  }
  logout(){
    localStorage.removeItem('user');
    this.currentUser.set(null);
  }
}
