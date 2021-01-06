import { PhotoComment } from 'src/app/_models/creteComment';
import { Component, EventEmitter, Input, OnInit, Output, TemplateRef, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { Photo } from 'src/app/_models/photo';
import { PhCommentCreatorDto, PhotoWithComments } from 'src/app/_models/photoWithComments';
import { PhotosService } from 'src/app/_services/photos.service';
import { ActivatedRoute, Router } from '@angular/router';
import { AccountService } from 'src/app/_services/account.service';
import { photoLikesDto } from 'src/app/_models/InstagramPhotos/photoLikesDto';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { User } from 'src/app/_models/user';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { PhotoCommentObject } from 'src/app/_models/InstagramPhotos/photoComment';

@Component({
  selector: 'app-image-modal-photo',
  templateUrl: './image-modal-photo.component.html',
  styleUrls: ['./image-modal-photo.component.css']
})
export class ImageModalPhotoComponent implements OnInit {
  @Input() photoId: number;
  photoWithComment: PhotoWithComments;
  photoComment: string;
  photoCommentForEditOriginal: string;
  photoCommentForDelete: string;
  @ViewChild('photoCommentForm', {static : false}) photoCommentForm: NgForm;
  // this talks to the parent component
  @Output() closeModal = new EventEmitter<boolean>();

  likedPhotosIds: number[] = [];
  currentUsername: string;
  modalRef: BsModalRef;
  message: string;
  editMode: boolean = false;
  photoLikesDto: photoLikesDto[] = [];
  numberOfLikesPerPhoto: number;

  constructor(private photoService: PhotosService, private router: Router, private route: ActivatedRoute,
     private accountService: AccountService, private alertify: AlertifyService, private modalService: BsModalService) {
    // detect any changes
    this.accountService.likedPhotosByLogedInUser$.subscribe((res: number[]) => {
      this.likedPhotosIds = res;
    }, error => console.log('error:', error));

    this.accountService.currentUser$.subscribe((res: User) => {
      this.currentUsername = res.username;
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
    this.accountService.currentUser$.subscribe((res: User) => {
      this.currentUsername = res.username;
    });
    this.loadComments(this.photoId);
    // fire up the emiter
    this.accountService.getLikedPhotosByUser();
    this.getLikedPhotosByUser();
    this.numberOfLikes(this.photoId);
  }

  loadComments(photoId: number) {
    this.photoService.getCommentsForPhoto(photoId).subscribe((res: PhotoWithComments) => {
      this.photoWithComment = res;
      console.log('Photo with comments:', this.photoWithComment);
    }, error => console.log('error:', error))
  }

  createComment() {
    console.log(`comment created:  ${this.photoComment} for photoId: ${this.photoWithComment.id}`);
    if (!this.editMode) {
      console.log(`comment created:  ${this.photoComment} for photoId: ${this.photoId}`);
      const photoComment: PhotoComment = { photoComment: this.photoComment};
      this.photoService.createPhotoComment(photoComment, this.photoId).subscribe((res:PhCommentCreatorDto) => {
        this.photoWithComment.phCommentCreatorDtos.push(res);
        this.photoCommentForm.reset();
        this.loadComments(this.photoId);
        this.editMode = false;
        console.log('photo commment:', res);
  }, error => console.log('error:', error));
  }
  // edit model
  else {
    console.log(`comment edited:  ${this.photoComment} for photoId: ${this.photoId}`);
    const photoCommentObject = new PhotoCommentObject( this.photoCommentForEditOriginal,this.photoComment);
    this.photoService.editPhotoComment(this.photoId, photoCommentObject).subscribe((res: PhCommentCreatorDto) => {
      if (res) {
        // replace the old phtoComment with the new one
        console.log('edited comment:', res);
        let commentObhectForEdit =  this.photoWithComment.phCommentCreatorDtos.find(el => el.comment === this.photoCommentForEditOriginal);
        commentObhectForEdit.comment = res.comment;
        this.alertify.success('Comment edited');
        this.editMode = false;
        this.photoCommentForm.reset();
      }
    }, error => console.log('error:', error))
  }
  }

  navigateToUser(username: string) {
    // close the modal in which this component lives, bubble up event
    this.closeModal.emit(true);

    console.log('navigateToUser:', username);
    this.router.navigate([`instagramMembers/${username}`], {relativeTo: this.route});
  }

  canBeLiked(photoId: number) {
    // return photoId in this.likedPhtosIds;
    return this.likedPhotosIds.find(el => el === photoId);
  }

  getLikedPhotosByUser() {
    this.accountService.likedPhotosByLogedInUser$.subscribe((res: number[]) => {
      this.likedPhotosIds = res;
      console.log('likedPhotos in modal:', this.likedPhotosIds)
    }, error => console.log('error:', error));
  }

  likePhoto(photoId: number) {
    console.log('likePhoto:', photoId);
    console.log('like photo:', photoId);
    this.accountService.addPhotoLike(photoId).subscribe((res: photoLikesDto) => {
      if (res !== null) {
        // fire up the emiter
        this.accountService.getLikedPhotosByUser();
        this.photoService.getNumberOfLikesPerPhoto(photoId);
        this.alertify.success('Liked a photo');
      }
    }, error => console.log('error:', error));
    // fire up the emitter
    this.photoService.getNumberOfLikesPerPhoto(photoId);
  }

  deletePhotoLike(photoId: number) {
    console.log('delete photo like:', photoId);
    this.accountService.deletePhotoLike(photoId).subscribe((res: boolean) => {
      if (res) {
        // fire up the emiter
        this.accountService.getLikedPhotosByUser();
        this.photoService.getNumberOfLikesPerPhoto(photoId);
        this.alertify.error('Unliked photo');
      }
    }, error => console.log('error:', error));
    // fire up the emitter
    this.photoService.getNumberOfLikesPerPhoto(photoId);
  }

  deleteCommentModal(comment: string, photoId: number, template: TemplateRef<any>) {
    console.log(`deleteComment: ${comment} in photoId: ${photoId}`);
    this.photoCommentForDelete = comment;
    this.modalRef = this.modalService.show(template, {class: 'modal-lg modal-dialog modal-dialog-centered'});
  }

  confirm(): void {
    this.message = 'Confirmed!';
    this.photoService.deletePhotoComment(this.photoId).subscribe((res: boolean) => {
      if (res) {
        var filtered = this.photoWithComment.phCommentCreatorDtos.filter(el => el.comment !== this.photoCommentForDelete);
        this.photoWithComment.phCommentCreatorDtos = filtered;
        this.alertify.success('Comment Deleted');
        // this.closeModal.emit(true);
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



}
