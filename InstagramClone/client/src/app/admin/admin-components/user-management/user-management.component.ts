import { Component, OnInit } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { RolesModalComponent } from 'src/app/modals/roles-modal/roles-modal.component';
import { Roles } from 'src/app/_models/roles';
import { User } from 'src/app/_models/user';
import { AdminService } from 'src/app/_services/admin.service';

@Component({
  selector: 'app-user-management',
  templateUrl: './user-management.component.html',
  styleUrls: ['./user-management.component.css']
})
export class UserManagementComponent implements OnInit {
  users: Partial<User[]>;
  bsModalRef: BsModalRef;
  list: any[] = [];
  currentRoles: string[] = [];
  constructor(private adminService: AdminService, private modalService: BsModalService) { }

  ngOnInit() {
    this.loadUsersWIthRoles();
    this.getAllRoles();
  }

  loadUsersWIthRoles() {
    this.adminService.getUsersWithRoles().subscribe(res => this.users = res);
  }

  getAllRoles() {
    this.adminService.getAllRoles().subscribe(res => {
      this.list = res;
      console.log('all roles:', this.list);
    });
  }

  // for this modal, we use "Modal with Component"
  // openRolesModal(username: string) {
  //   this.adminService.getRolesForUser(username).subscribe(res => {
  //     this.currentRoles = res;
  //     console.log('roles for current user:', this.currentRoles);
  //     this.adminService.getActiveRolesForUser(username, this.currentRoles);
  //     const initialState = {
  //       list: this.list, // list of all roles
  //       currentRoles: this.currentRoles, // list of user's roles
  //       title: 'Assing Role',
  //       username
  //     };
  //     if (initialState.list.length > 0) {
  //       this.bsModalRef = this.modalService.show(RolesModalComponent, {initialState});
  //     }

  //   });

  //   this.bsModalRef.content.closeBtnName = 'Close'
  //   ;
  // }

  openRolesModal(user: User) {
    const config = {
      class: 'modal-dialog-centered',
      initialState: {
        user,
        roles: this.getRolesArray(user)
      }
    };
    this.bsModalRef = this.modalService.show(RolesModalComponent, config);
    // listen for changes in child component
    this.bsModalRef.content.updateSelectedRoles.subscribe(values => {
      const rolesToUpdate = {
        roles: [...values.filter(el => el.checked === true).map(el => el.name)]
      };
      if (rolesToUpdate) {
        this.adminService.updateUserRoles(user.username, rolesToUpdate.roles).subscribe(() => {
          user.roles = [...rolesToUpdate.roles];
        });
      }
    });
  }

  private getRolesArray(user: User) {
    const roles = [];
    const userRoles = user.roles;
    // get the availableRoles from the API(my extra work, todo!)
    const availableRoles: any[] = [
      {name: 'Admin', value: 'Admin'},
      {name: 'Moderator', value: 'Moderator'},
      {name: 'Member', value: 'Member'}
    ];

    availableRoles.forEach(role => {
      let isMatch = false;
      for (const userRole of userRoles) {
        if (role.name === userRole) {
          isMatch = true;
          role.checked = true;
          roles.push(role);
          break;
        }
      }
      if (!isMatch) {
        role.checked = false;
        roles.push(role);
      }
    });

    return roles;
  }

}

