import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Observable } from 'rxjs';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';
import { AlertifyService } from '../_services/alertify.service';


@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  constructor(private accountService: AccountService, private router: Router, private alertify: AlertifyService) { }
  navbarOpen: boolean;
  toggleDropdown: boolean;
  displayMenu: boolean;
  model: any = {};
  loggedIn: boolean;
  username: string;
  // local obesrvable, for async pipe
  currentUser$: Observable<User>;

  mainPhotoUrl: string;
  mainPhoto$: Observable<string>;

  ngOnInit() {
    // persisting the user from localStorage
    this.getCurrentUser();
    // this is neat approach as we did in skinet app
    this.currentUser$ = this.accountService.currentUser$;
    this.mainPhoto$ = this.accountService.mainPhotoUrl$;
  }

  toggleNavbar() {
    console.log('navbar toggle');
    this.navbarOpen = !this.navbarOpen;
    console.log('navbar is:', this.navbarOpen);
  }

  showDropdown() {
    console.log('show dropdown');
    this.toggleDropdown = !this.toggleDropdown;
  }

  login() {
    console.log('login from:', this.model);
    // subscribe to account method login
    this.accountService.login(this.model).subscribe(res => {
      console.log('res login:', res);
      this.loggedIn = true;
      // navigate to memebers once when logged in
      this.router.navigate(['instagram-photos']);
      this.alertify.success('You\'re now logged in');
    }, error => {
      console.log('error:', error);
      // this.alertify.error(error.error);
    });
  }

  logout() {
    this.loggedIn = false;
    setTimeout( () => { // temp solution added by me
      this.accountService.logout();
      // here I just used navgateByUrl coule be just navigate
      this.router.navigateByUrl('');
      // clear up the input form
      this.model.username = '';
      this.model.password = '';
    }, 300);

  }
  // get the current user grom the observable which is a buffer of type ReplaySubject
  getCurrentUser() {
    this.accountService.currentUser$.subscribe((user: User) => {
      if (user) {
        // this !! tunrs it into a boolean
        this.loggedIn = !!user;
        this.username = user.username;
        this.mainPhotoUrl = user.photoUrl;
      }
    }, error => console.log('error', error.error));
  }

  menuDropdown() {
    // this.displayMenu = !this.displayMenu;
  }

}
