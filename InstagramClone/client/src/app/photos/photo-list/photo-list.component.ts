import { PhotoWithComments } from './../../_models/photoWithComments';
import { Component, OnInit } from '@angular/core';
import { PhotosService } from 'src/app/_services/photos.service';
import { AccountService } from 'src/app/_services/account.service';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-photo-list',
  templateUrl: './photo-list.component.html',
  styleUrls: ['./photo-list.component.css']
})
export class PhotoListComponent implements OnInit {
  photosWithComments: PhotoWithComments[] = [];
  user: User;
  likedPhotoIds: number[] = [];

  constructor(private photoService: PhotosService, private accountService: AccountService) {
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

}
