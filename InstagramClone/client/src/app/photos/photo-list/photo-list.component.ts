import { PhotoWithComments } from './../../_models/photoWithComments';
import { Component, OnInit } from '@angular/core';
import { PhotosService } from 'src/app/_services/photos.service';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/user';
import { Router } from '@angular/router';
import { MembersService } from 'src/app/_services/members.service';
import { LikeDto } from 'src/app/_models/InstagramPhotos/likesDto';

@Component({
  selector: 'app-photo-list',
  templateUrl: './photo-list.component.html',
  styleUrls: ['./photo-list.component.css']
})
export class PhotoListComponent implements OnInit {
  photosWithComments: PhotoWithComments[] = [];
  user: User;
  likedPhotoIds: number[] = [];
  notLikedUsersByLogedInUser: LikeDto[] = [];

  constructor(private photoService: PhotosService, private accountService: AccountService,
    private memberService: MembersService, private router: Router) {
    // detect any changes in loged in user
    this.accountService.likedPhotosByLogedInUser$.subscribe(res => {
      this.likedPhotoIds = res;
      console.log('likedPhotos:', res);
    })
   }

  ngOnInit() {
    this.loadPhotos();
    this.accountService.currentUser$.subscribe((res: User) => {
      this.user = res;
      console.log('Current user:', this.user);
      this.getLikedPhotos();
      this.getUsersNotLiked();
    }, error => console.log('error:', error))
  }

  loadPhotos() {
    console.log('loading photos');
    this.photoService.getPhotos().subscribe(res => {
      // console.log('loaded photos:', res);
      this.photosWithComments = res.filter(el => el.isMain === false);
      console.log('Photos with comments:', this.photosWithComments);
    }, error => console.log('error:', error));
  }
  // fire up the emitter
  getLikedPhotos() {
    this.accountService.getLikedPhotosByUser();
  }

  getUsersNotLiked() {
    this.accountService.getNotLikedusersByLogedInUser();
    this.accountService.notLikedUsersByLogedInUser$.subscribe((res: LikeDto[]) => {
      this.notLikedUsersByLogedInUser = res;
      console.log('not liked users:', this.notLikedUsersByLogedInUser);
    }, error => console.log('error:', error));
  }

  navigateToUser(username: string) {
    this.router.navigate([`instagramMembers/${username}`])
  }

  followUser(username: string) {
    this.memberService.addLike(username).subscribe((res: {success: boolean}) => {
      if (res.success) {
        console.log('liked user', username);
        this.getUsersNotLiked();
        // just remove from the DOM
        // this.notLikedUsersByLogedInUser = this.notLikedUsersByLogedInUser.filter(el => el.username !== username);
      }
    }, error => console.log('error:', error))
  }

}
