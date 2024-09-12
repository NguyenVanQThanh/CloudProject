import { inject, Injectable, signal } from '@angular/core';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr'; // Import HubConnection from '@microsoft/signalr'
import { environment } from '../../environments/environment';
import { ToastrService } from 'ngx-toastr';
import { User } from '../_models/user';
import { take } from 'rxjs';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubsUrl;
  private hubConnection? : HubConnection; // Declare 'hubConnection' as 'HubConnection'
  private toastr = inject(ToastrService);
  private router = inject(Router);

  onlineUsers = signal<string[]>([]);

  createHubConnection(user : User){
    this.hubConnection = new HubConnectionBuilder()
    .withUrl(this.hubUrl + 'presence', {
      accessTokenFactory: () => user.token
    }).withAutomaticReconnect().build();

    this.hubConnection.start().catch(error => console.log(error));
    this.hubConnection.on('UserIsOnline', username => {
      this.onlineUsers.update(users => [...users, username])
      // this.toastr.info(username +'has connected');
    })
    this.hubConnection.on('UserIsOffline', username => {
      this.onlineUsers.update(users=>users.filter(x => x!== username))
      // this.toastr.warning(username +'has disconnected');
    });
    this.hubConnection.on('GetOnlineUsers', usernames => {
      this.onlineUsers.set(usernames);
    })
    this.hubConnection.on('NewMessageReceived', ({userName, knowAs})=> {
      this.toastr.info(knowAs+ " has sent you a new message! click me to see it!")
      .onTap
      .pipe(take(1))
      .subscribe(()=> {
        this.router.navigateByUrl('/members/' + userName + '?tab=messages');
      })
    })
  }
  stopHubConnection(){
    if (this.hubConnection?.state === HubConnectionState.Connected){
      this.hubConnection.stop().catch(error => console.log(error));
    }
  }
}

