import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { Photo } from 'src/app/_models/photo';

@Component({
  selector: 'app-instagram-image-modal',
  templateUrl: './instagram-image-modal.component.html',
  styleUrls: ['./instagram-image-modal.component.css']
})
export class InstagramImageModalComponent implements OnInit {
  title: string;
  closeBtnName: string;
  list: any[] = [];
  photoDto: Photo;
  // this input property can be accessed in the "parent", from where we rended this component as a modal
  // the same approach was done in admin section, user-management.component which loads roles-modal
  @Input() reloadComponent = new EventEmitter<boolean>();
  constructor(public bsModalRef: BsModalRef) { }

  ngOnInit() {
  }

  closeModalInParent(close: boolean) {
    if (close == true){
      console.log('close:', close);
      this.reloadComponent.emit(true);
      this.bsModalRef.hide();
    }
  }

}
