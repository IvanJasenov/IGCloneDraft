import { Injectable } from '@angular/core';
import { HubConnection } from '@microsoft/signalr/dist/esm/HubConnection';
import { HubConnectionBuilder } from '@microsoft/signalr/dist/esm/HubConnectionBuilder';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { AlertifyService } from './alertify.service';

@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private onlineUserSource = new BehaviorSubject<string[]>([]);
  onlineUsers$ = this.onlineUserSource.asObservable();

  constructor(private alertifyService: AlertifyService) { }

  // this gets called when we set the current user
  createHubConnection(user: User) {
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl + 'presence', {
      accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build();

    // start thr hub connection
    this.hubConnection.start().catch(err => console.log(err));

    // listen for server eventsource
    // in on('...') has to exactly match what is in PresenceHub.cs in the API side
    this.hubConnection.on('UserIsOnline', username => {
      this.alertifyService.success(username + ' connected');
    });

    this.hubConnection.on('UserIsOffline', username => {
      this.alertifyService.warning(username + ' has disconnected');
    });

    this.hubConnection.on('GetOnlineUsers', (usernames: string[]) => this.onlineUserSource.next(usernames));
  }

  stopHubConnection() {
    this.hubConnection.stop().catch(error => console.log(error));
  }
}
