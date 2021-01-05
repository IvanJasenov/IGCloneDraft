import { PhCommentCreatorDto } from './../../_models/photoWithComments';
import { Component, Input, OnInit, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Router } from '@angular/router';
import { PhotoComment } from 'src/app/_models/creteComment';
import { PhotoWithComments } from 'src/app/_models/photoWithComments';
import { PhotosService } from 'src/app/_services/photos.service';
import { AccountService } from 'src/app/_services/account.service';
import { photoLikesDto } from 'src/app/_models/InstagramPhotos/photoLikesDto';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { PhotoCommentObject } from 'src/app/_models/InstagramPhotos/photoComment';
import { PhotoLikesModalComponent } from 'src/app/modals/photo-likes-modal/photo-likes-modal.component';

@Component({
  selector: 'app-photo-card',
  templateUrl: './photo-card.component.html',
  styleUrls: ['./photo-card.component.css']
})
export class PhotoCardComponent implements OnInit {

  @Input() photoElement: PhotoWithComments;
  likedPhtosIds: number[] = [];
  @Input() likedPhotosIdByUser: number[] = [];
  photoComment: string;
  photoCommentForDelete: string;
  photoCommentForEditOriginal: string;
  @ViewChild('photoCommentForm', {static : false}) photoCommentForm: NgForm;
  @Input() currentUsername: string;

  showAllPhotoComments: boolean = false;
  modalRef: BsModalRef;
  message: string;
  editMode: boolean = false;
  numberOfLikesPerPhoto: number;
  photoLikesDto: photoLikesDto[] = [];
  showLikesData = false;
  bsModalRef: BsModalRef;

  constructor(private router: Router, private photoService: PhotosService, private accountService: AccountService,
    private alertify: AlertifyService, private modalService: BsModalService) {
    // listen for updates in liked photos
    this.accountService.likedPhotosByLogedInUser$.subscribe((res: number[]) => {
      this.likedPhtosIds = res;
      console.log('likedPhotosByLogedInUser:', this.likedPhtosIds);
    });

    this.photoService.photoLikes$.subscribe((res: photoLikesDto[]) => {
      if (res) {
        console.log('likes:', res.length)
        this.numberOfLikesPerPhoto = res.length;
      }
      if (res === null) {
        this.numberOfLikesPerPhoto = 0;
      }
    }, error => console.log('error:', error));

   }

  ngOnInit() {
    this.showAllLikes(this.photoElement.id);
  }


  openModalForOptions() {
    console.log('open modal for options');
  }

  showModalForUser(username: string) {
    console.log('open modal with user info for:', username);
  }

  hideModalForUser(username: string) {
    console.log('hide modal for user info for:', username);
  }

  navigateToUser(username: string) {
    console.log('go to user info for:', username);
    this.router.navigate(['instagramMembers/' + username]);
  }

  navigate() {
    console.log('navigate');
  }

  createComment() {
    // in create mode
    if (!this.editMode) {
      console.log(`comment created:  ${this.photoComment} for photoId: ${this.photoElement.id}`);
      const photoComment: PhotoComment = { photoComment: this.photoComment};
      this.photoService.createPhotoComment(photoComment, this.photoElement.id).subscribe((res:PhCommentCreatorDto) => {
      this.photoElement.phCommentCreatorDtos.push(res);
      this.photoCommentForm.reset();
      this.editMode = !this.editMode;
      console.log('photo commment:', res);
    }, error => console.log('error:', error));
    }
    // edit model
    else {
      console.log(`comment edited:  ${this.photoComment} for photoId: ${this.photoElement.id}`);
      const photoCommentObject = new PhotoCommentObject( this.photoCommentForEditOriginal,this.photoComment);
      this.photoService.editPhotoComment(this.photoElement.id, photoCommentObject).subscribe((res: PhCommentCreatorDto) => {
        if (res) {
          // replace the old phtoComment with the new one
          console.log('edited comment:', res);
          let commentObhectForEdit =  this.photoElement.phCommentCreatorDtos.find(el => el.comment === this.photoCommentForEditOriginal);
          commentObhectForEdit.comment = res.comment;
          this.alertify.success('Comment edited');
          this.editMode = !this.editMode;
          this.photoCommentForm.reset();
        }
      }, error => console.log('error:', error))
    }

  }

   showAllComments() {
    console.log('show all comments');
    this.showAllPhotoComments = !this.showAllPhotoComments;
  }

  canBeLiked(photoId: number) {
    // return photoId in this.likedPhtosIds;
    return this.likedPhotosIdByUser.find(el => el === photoId);
  }

  likePhoto(photoId: number) {
    console.log('like photo:', photoId);
    this.accountService.addPhotoLike(photoId).subscribe((res: photoLikesDto) => {
      if (res !== null) {
        // fire up the emiter
        this.accountService.getLikedPhotosByUser();
        this.showAllLikes(this.photoElement.id);
        this.alertify.success('Liked a photo');
      }
    }, error => console.log('error:', error));
  }

  deletePhotoLike(photoId: number) {
    console.log('delete photo like:', photoId);
    this.accountService.deletePhotoLike(photoId).subscribe((res: boolean) => {
      if (res) {
        // fire up the emiter
        this.accountService.getLikedPhotosByUser();
        this.showAllLikes(this.photoElement.id);
        this.alertify.error('Unliked photo');
      }
    }, error => console.log('error:', error));
  }

  deleteCommentModal(comment: string, photoId: number, template: TemplateRef<any>) {
    console.log(`deleteComment: ${comment} in photoId: ${photoId}`);
    this.photoCommentForDelete = comment;
    this.modalRef = this.modalService.show(template, {class: 'modal-lg modal-dialog modal-dialog-centered'});
  }

  confirm(): void {
    this.message = 'Confirmed!';
    this.photoService.deletePhotoComment(this.photoElement.id).subscribe((res: boolean) => {
      if (res) {
        var filtered = this.photoElement.phCommentCreatorDtos.filter(el => el.comment !== this.photoCommentForDelete);
        this.photoElement.phCommentCreatorDtos = filtered;
        this.alertify.success('Comment Deleted');
      }
    }, error => console.log('error:', error));

    this.modalRef.hide();
  }

  decline(): void {
    this.message = 'Declined!';
    this.modalRef.hide();
  }

  editComment(comment: string, photoId: number) {
    this.photoCommentForEditOriginal = comment;
    this.photoComment = comment;
    this.editMode = true;
  }

  cancelEdit() {
    this.editMode = false;
    this.photoCommentForm.reset();
  }

  numberOfLikes(photoId: number) {
    // fire up the emiter
    this.photoService.getNumberOfLikesPerPhoto(photoId);
    this.photoService.photoLikes$.subscribe((res: photoLikesDto[]) => {
      if (res) {
        this.photoLikesDto = res;
        this.numberOfLikesPerPhoto = res.length;
        console.log('likes:', res.length)
        // console.log(`photo id: ${this.photoElement.id} is liked: ${res}`)
      }
      if (res === null) {
        this.numberOfLikesPerPhoto = 0;
      }
    }, error => console.log('error:', error))
  }

  showAllLikes(photoId: number) {
    console.log('Likes Objects for photoId:', photoId);
    const photoLikesUsers = this.photoService.getLikesForPhotoId(photoId).subscribe((res: photoLikesDto[]) => {
      if (res) {
        this.photoLikesDto = res; // jus assign
        console.log('photo likes(users):', this.photoLikesDto);
        this.numberOfLikesPerPhoto = this.photoLikesDto.length;
        this.showLikesData = true;
      }
    }, error => console.log('error:', error));
  }

  showLikesModal() {
    console.log('photo likes(users):', this.photoLikesDto);
    const initialState = {
      class: 'modal-dialog-centered modal-dialog modal-lg',
      photoLikesDto: this.photoLikesDto,
      title: 'Likes'
    };

    this.bsModalRef = this.modalService.show(PhotoLikesModalComponent,
        Object.assign({initialState}, { class: 'modal-dialog-centered modal-dialog modal-md'}));

    this.bsModalRef.content.closeBtnName = 'Close';

  }


}
