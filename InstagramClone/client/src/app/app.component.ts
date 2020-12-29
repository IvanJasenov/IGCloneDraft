import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { Title } from '@angular/platform-browser';
import { NgxSpinnerService } from 'ngx-spinner';
import { User } from './_models/user';
import { AccountService } from './_services/account.service';
import { PresenceService } from './_services/presence.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css'],
})
export class AppComponent implements OnInit {
  title = 'client';
  users: any;

  constructor(private http: HttpClient, private accountService: AccountService,
              private spinner: NgxSpinnerService, private presence: PresenceService) {}

  ngOnInit(): void {
    // this.getUsers();
    this.setCurrentUser();
  }

  setCurrentUser() {
    const user: User = JSON.parse(localStorage.getItem('user'));
    this.spinner.show();
    if (user) {
      this.accountService.setCurrentUser(user);
      // for the SignalR
      this.presence.createHubConnection(user);
      this.spinner.hide();
    }
  }

  // getUsers() {
  //   this.http.get('https://localhost:44308/api/users').subscribe(
  //     (response) => {
  //       console.log(response);
  //       this.users = response;
  //     },
  //     (error) => console.log('error:', error.error)
  //   );
  // }
}
