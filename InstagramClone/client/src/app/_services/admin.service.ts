import { Roles } from './../_models/roles';
import { HttpClient } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { User } from '../_models/user';
import { BehaviorSubject } from 'rxjs/internal/BehaviorSubject';
import { map } from 'rxjs/operators';
import { Photo } from '../_models/photo';

@Injectable({
  providedIn: 'root'
})
export class AdminService {
  baseUrl = environment.apiUrl;
  private allRoles = new BehaviorSubject<string[]>([]);
  public roles$ = this.allRoles.asObservable();
  constructor(private http: HttpClient) { }

  private allUserRoles: string[] = [];

  getUsersWithRoles() {
    return this.http.get<Partial<User[]>>(this.baseUrl +  'admin/users-with-roles');
  }

  getAllRoles() {
    return this.http.get<string[]>(this.baseUrl + 'admin/get-all-roles').pipe(
      map(response => {
        this.allRoles.next(response);
        this.allUserRoles = response;
        return response;
      })
    );
  }

  getRolesForUser(username: string) {
    return this.http.get<string[]>(this.baseUrl + 'admin/roles-for-user/' + username);
    // do the filtering od active/inactive roles as you did in roles-modal.component.ts
  }

  getActiveRolesForUser(username: string, allRoles: string[]) {

  }

  updateUserRoles(username: string, roles: string[]) {
    return this.http.post(this.baseUrl + 'admin/edit-roles/' + username + '?roles=' + roles, {});
  }

  getUnapprovedPhotos() {
    return this.http.get(this.baseUrl + 'admin/get-unapproved-photos');
  }

  approvePhoto(photoId: number) {
    return this.http.put<Photo>(this.baseUrl + 'admin/approve-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http.delete(this.baseUrl + 'admin/delete-photo/' + photoId);
  }

}
