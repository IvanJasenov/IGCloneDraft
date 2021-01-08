import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, ReplaySubject } from 'rxjs';
import {map} from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { likedPhotosByUser } from '../_models/InstagramPhotos/likedPhotosByUser';
import { LikeDto } from '../_models/InstagramPhotos/likesDto';
import { User } from '../_models/user';
import { PresenceService } from './presence.service';

@Injectable({
  providedIn: 'root'
})
export class AccountService {
  // angular will automaticallu import the right url depending on which environment we're working on
  baseUrl = environment.apiUrl;
  // this is private!
  private currentUserSource = new ReplaySubject<User>(1); // buffer can hold one item
  // this is public, hides the private observable!
  currentUser$ = this.currentUserSource.asObservable(); // make it observable

  // hadle the main photo as observable
  private mainPhotoUrl = new BehaviorSubject<string>(null);
  mainPhotoUrl$ = this.mainPhotoUrl.asObservable();

  private likedPhotosByLogedInUser = new BehaviorSubject<number[]>(null);
  likedPhotosByLogedInUser$ = this.likedPhotosByLogedInUser.asObservable();

  private likedUsersByLogedInUser = new BehaviorSubject<LikeDto[]>(null);
  likedUsersByLogedInUser$ = this.likedUsersByLogedInUser.asObservable();

  private notLikedUsersByLogedInUser = new BehaviorSubject<LikeDto[]>(null);
  notLikedUsersByLogedInUser$ = this.notLikedUsersByLogedInUser.asObservable();

  constructor(private http: HttpClient, private presence: PresenceService) { }

  login(model: any) {
    return this.http.post(this.baseUrl + 'account/login', model).pipe(
      map((response: User) => {
        const user = response;
        if (user) {
          this.setCurrentUser(user);
          this.presence.createHubConnection(user);
          setTimeout(() => {
            this.getLikedPhotosByUser();
            this.getLikedUsersByLogedinUser();
          }, 400);
        }
        // return from the observable so in nav,component line 48 can see the result
        return response;
      })
    );
  }

setCurrentUser(user: User) {
  user.roles = [];
  const roles = this.getDecodedToken(user.token).role;
  Array.isArray(roles) ? user.roles = roles : user.roles.push(roles);
  this.mainPhotoUrl.next(user.photoUrl);
  this.currentUserSource.next(user);
  // persisting the user in the localStorage
  localStorage.setItem('user', JSON.stringify(user));
}

  logout() {
    localStorage.removeItem('user');
    this.currentUserSource.next(null);
    this.presence.stopHubConnection();
  }

  // when a user registeres we gonna consider them as logged in
  register(registerModel: any) {
    return this.http.post(this.baseUrl + 'account/register', registerModel).pipe(
      map((user: User) => {
        if (user) {
          // this.login(user).subscribe();
          localStorage.setItem('user', JSON.stringify(user));
          this.currentUserSource.next(user);
          this.presence.createHubConnection(user);
        } // return from map as well so I can see the result in register.component.ts register method, outter return is observable
        return user;
      }, error => console.log('error in register:', error))
    );
  }

  getToken() {
    const user = JSON.parse(localStorage.getItem('user'));
    if (user) {
      const token = user.token;
      return token;
    }
    return null;
  }

  getMainPhoto(username: string) {
     this.http.get(this.baseUrl + 'users/get-main-photo' + username).subscribe((res: {main: string}) => {
       console.log('mainPhoto:', res.main);
       this.mainPhotoUrl.next(res.main);
    });
  }


  getDecodedToken(token) {
    // index 1, the payload in the token is at index 1
    return JSON.parse(atob(token.split('.')[1]));
  }

  getCurrentUser() {
    const myData = JSON.parse(localStorage.getItem('user'))
    console.log('localStorageUser:', myData);
    console.log('ltoken username:', myData.username);
    return myData;
  }

  getLikedPhotosByUser() {
    this.http.get(this.baseUrl + 'photo/get-liked-photos-by-logedInUser').subscribe((res: likedPhotosByUser) => {
      console.log('liked photos Ids by this user', res.likedPhotosIds)
      this.likedPhotosByLogedInUser.next(res.likedPhotosIds);
    })
  }

  addPhotoLike(photoId: number) {
    return this.http.post(this.baseUrl + 'photo/add-photo-like?photoId=' + photoId, {});
  }

  deletePhotoLike(photoId: number) {
    return this.http.delete(this.baseUrl + 'photo/delete-photo-like?photoId=' + photoId);
  }

  getLikedUsersByLogedinUser() {
    this.http.get<LikeDto[]>(this.baseUrl + 'likes?predicates=liked').pipe(
      map((res: LikeDto[]) => {
        console.log('liked users:', res);
        this.likedUsersByLogedInUser.next(res);
      })
    ).subscribe();
  }

  getNotLikedusersByLogedInUser() {
    this.http.get<LikeDto[]>(this.baseUrl + 'likes/users-not-followed').pipe(
      map((res: LikeDto[]) => {
        console.log('not liked users:', res);
        this.notLikedUsersByLogedInUser.next(res);
      })
    ).subscribe();
  }

}

