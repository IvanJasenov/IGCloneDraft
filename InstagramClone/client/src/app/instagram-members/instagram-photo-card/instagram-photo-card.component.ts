import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { InstagramImageModalComponent } from 'src/app/modals/instagram-image-modal/instagram-image-modal.component';
import { photoLikesDto } from 'src/app/_models/InstagramPhotos/photoLikesDto';
import { Member } from 'src/app/_models/member';
import { Photo } from 'src/app/_models/photo';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { MembersService } from 'src/app/_services/members.service';
import { PhotosService } from 'src/app/_services/photos.service';

@Component({
  selector: 'app-instagram-photo-card',
  templateUrl: './instagram-photo-card.component.html',
  styleUrls: ['./instagram-photo-card.component.css']
})
export class InstagramPhotoCardComponent implements OnInit {
  @Input() photo: Partial<Photo>;
  @Input() showLikeButton: boolean;
  bsModalRef: BsModalRef;
  @Output()reloadUsers = new EventEmitter<boolean>();
  numberOfLikesPerPhoto: number;
  photoLikesDto: photoLikesDto[] = [];

  constructor(private router: Router, private memberService: MembersService, private alertify: AlertifyService,
     private modalService: BsModalService, private photoService: PhotosService) {

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
    this.numberOfLikes(this.photo.id);
  }

  addLike(photoId: number) {
    console.log('add like to photoId: ' + photoId);
  }

  commentPhoto(photoId: number) {
    console.log('comment modal fro photoId: ' + photoId);
  }

  showModal(photoId: number) {
    console.log('show modal for photoId: ' + photoId);
    const initialState = {
      class: 'modal-dialog-centered modal-dialog modal-lg',
      photoDto: this.photo,
      title: 'Modal with component'
    };

    this.bsModalRef = this.modalService.show(InstagramImageModalComponent,
                                              Object.assign({initialState}, { class: 'modal-dialog-centered modal-dialog modal-xl' }));
    this.bsModalRef.content.reloadComponent.subscribe(res => {
      console.log('reload component:', res)
      this.reloadUsers.emit(true);
    }, error => console.log('error:', error));
    this.bsModalRef.content.closeBtnName = 'Close';
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
