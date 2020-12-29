import { Component, OnInit, TemplateRef } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { Photo } from 'src/app/_models/photo';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';
import { AlertifyService } from 'src/app/_services/alertify.service';

@Component({
  selector: 'app-photo-management',
  templateUrl: './photo-management.component.html',
  styleUrls: ['./photo-management.component.css']
})
export class PhotoManagementComponent implements OnInit {

  unapprovedPhotos: Photo[] = [];
  deleteModalRef: BsModalRef;
  photoIdToDelete: number;

  constructor(private adminService: AdminService, private modalService: BsModalService, private alertify: AlertifyService) { }

  ngOnInit() {
    this.loadUnApprovedPhotos();
  }

  loadUnApprovedPhotos() {
    this.adminService.getUnapprovedPhotos().subscribe((res: Photo[]) => {
      this.unapprovedPhotos = res;
      console.log('UnApproved photos:', res);
    });
  }

  aprovePhoto(photoId: number) {
    console.log('photo id to approve:', photoId);
    this.adminService.approvePhoto(photoId).subscribe(res => {
      const indexOf = this.unapprovedPhotos.indexOf(res);
      this.unapprovedPhotos.splice(indexOf);
      this.alertify.success('Photo approved');
    });
  }

  deletePhoto() {
    console.log('photo id to delete:', this.photoIdToDelete);
    // hide the modal when exectied the API
    this.adminService.deletePhoto(this.photoIdToDelete).subscribe(res => {
      console.log('res from delete', res);
      if (res['message'] === true) {
        // remove from the unapprovedPhotos array....
        this.unapprovedPhotos = this.unapprovedPhotos.filter(el => el.id !== this.photoIdToDelete);
        this.deleteModalRef.hide();
        this.alertify.success('Photo deleted');
      }

    }, error => console.log('error:', error));
  }

  openModal(template: TemplateRef<any>, photoId: number) {
    this.deleteModalRef = this.modalService.show(template);
    this.photoIdToDelete = photoId;
  }

  hideChildModal() {
    this.deleteModalRef.hide();
  }

}
