import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { BehaviorSubject } from 'rxjs';
import { environment } from 'src/environments/environment';
import { Message } from '../_models/message';
import { User } from '../_models/user';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root'
})
export class MessageService {
  baseUrl = environment.apiUrl;
  hubUrl = environment.hubUrl;
  private hubConnection: HubConnection;
  private messageThreadSource = new BehaviorSubject<Message[]>([]);
  messageThread$ = this.messageThreadSource.asObservable();

  constructor(private http: HttpClient) { }

  getMessages(pageNumber: number, pageSize: number, container: string) {
    // set up the query for the api
    let params = getPaginationHeaders(pageNumber, pageSize);
    params = params.append('Container', container);
    // make a call to the api
    return getPaginatedResult<Message[]>(this.baseUrl + 'message', params, this.http);
  }

  createHubConnection(user: User, otherUsername: string) {
    this.hubConnection = new HubConnectionBuilder().withUrl(this.hubUrl + 'message?user=' + otherUsername, {
      accessTokenFactory: () => user.token
    })
    .withAutomaticReconnect()
    .build();

    this.hubConnection.start().catch(err => console.log(err));

    this.hubConnection.on('RecieveMessageThread', messages => {
      this.messageThreadSource.next(messages);
    });
  }

  stopHubConnection() {
    this.hubConnection.stop();
  }

  getMessageThread(username: string) {
    // this way I specify what the return type will be so I dont have to do it when I subsctribe in the component
    return this.http.get<Message[]>(this.baseUrl + 'message/thread/' + username);
  }

  sendMessage(username: string, content: string) {
    return this.http.post(this.baseUrl + 'message', { recipientUsername: username, content });
  }

  deleteMessage(messageId: number) {
    return this.http.delete(this.baseUrl + 'message/' + messageId);
  }
}
