import { Component, Input, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { photoLikesDto } from 'src/app/_models/InstagramPhotos/photoLikesDto';

@Component({
  selector: 'app-photo-likes-modal',
  templateUrl: './photo-likes-modal.component.html',
  styleUrls: ['./photo-likes-modal.component.css']
})
export class PhotoLikesModalComponent implements OnInit {
  @Input() photoLikesDto: photoLikesDto[] = [];

  constructor(private bsModalRef: BsModalRef, private router: Router) { }

  ngOnInit() {
    console.log('passed down photoLikesDto:', this.photoLikesDto);
  }

  navigateToUser(username: string) {
    console.log('navigate to user:', username);
    this.router.navigate(['/instagramMembers', username]);
    this.bsModalRef.hide();
  }
}
