import { User } from './../../_models/user';
import { Component, Input, OnInit } from '@angular/core';
import { Member } from 'src/app/_models/member';
import { FileUploader } from 'ng2-file-upload';
import { environment } from 'src/environments/environment';
import { AccountService } from 'src/app/_services/account.service';
import { Photo } from 'src/app/_models/photo';
import { MembersService } from 'src/app/_services/members.service';
import { take } from 'rxjs/operators';
import { MainPhoto } from 'src/app/_models/mainPhoto';
import { Observable, of } from 'rxjs';
import { Router } from '@angular/router';

@Component({
  selector: 'app-photo-editor',
  templateUrl: './photo-editor.component.html',
  styleUrls: ['./photo-editor.component.css']
})
export class PhotoEditorComponent implements OnInit {

  @Input() member: Member;
  uploader: FileUploader;
  hasBaseDropzoneOver = false;
  baseUrl = environment.apiUrl;
  user: User;

  constructor(private accountService: AccountService, private memberService: MembersService, private router: Router) { }

  ngOnInit() {
    this.initializeUploader();
    this.getCurrentUser();
  }

  getCurrentUser() {
    // subscribe to any changes of the user
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => this.user = user);
  }

  fileOverBase(e: any): void {
    this.hasBaseDropzoneOver = e;
  }

  initializeUploader() {
    this.uploader = new FileUploader({
      url: this.baseUrl + 'users/add-photo',
      // we need to pass the token in Bearer,
      // we need to add authorization header in order for the backend to work properly
      authToken: 'Bearer ' + this.accountService.getToken(),
      isHTML5: true,
      allowedFileType: ['image', 'pdf'], // I added this pdf just to see if I can upload pdf
      removeAfterUpload: true, // remove from the dropzone after the upload completes
      autoUpload: false, // this upload the file rigt when it gets into the dropdown zone, we disa this
      maxFileSize: 10 * 1024 * 1024 // 10MB
    });

    this.uploader.onAfterAddingFile = (file) => {
      file.withCredentials = false;
    };

    this.uploader.onSuccessItem = (item, response, status, headers) => {
      if (response) {
         // JSON.parse gonna convert it into an object
         const photo: Photo = JSON.parse(response);
         this.member.photosDto.push(photo);
         if (photo.isMain) {
            this.user.photoUrl = photo.url;
            this.member.photoUrl = photo.url;
            this.accountService.setCurrentUser(this.user);
         }
      }
    };
  }

  setMainPhoto(photoId: number) {
    console.log('photo id to main:', photoId);
    this.memberService.setMainPhoto(photoId).subscribe((res: MainPhoto) => {
      console.log('setmain:', res.photoUrl);
      this.user.photoUrl = res.photoUrl;
      // update the logegd in user
      this.accountService.setCurrentUser(this.user);
      // edit the member used for this component
      this.member.photoUrl = res.photoUrl;
      this.member.photosDto.forEach(el => {
        if (el.isMain) {
          el.isMain = false;
        }
        if (el.id === photoId) {
          el.isMain = true;
        }
      });
    });
  }

  deletePhoto(photoId: number) {
    console.log('deletePhoto id:', photoId);
    this.memberService.deletePhoto(photoId).subscribe(res => {
      console.log('delete phot result:', res);
      // delete the photo in memeber's photo list
      this.member.photosDto = this.member.photosDto.filter(el => el.id !== photoId);
    });
  }

}
