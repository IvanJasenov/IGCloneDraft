import { Component, EventEmitter, Input, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';
import { User } from 'src/app/_models/user';

@Component({
  selector: 'app-roles-modal',
  templateUrl: './roles-modal.component.html',
  styleUrls: ['./roles-modal.component.css']
})
export class RolesModalComponent implements OnInit {

  // this I see for first time, input is also an emitter
  // the input is called updateSelectedRoles and is of type EventEmitter
   @Input() updateSelectedRoles = new EventEmitter();
   user: User;
   roles: any[] = [];

  constructor(public bsModalRef: BsModalRef) {}

  ngOnInit() {
  }

  updateRoles() {
    this.updateSelectedRoles.emit(this.roles);
    this.bsModalRef.hide();
  }

}
