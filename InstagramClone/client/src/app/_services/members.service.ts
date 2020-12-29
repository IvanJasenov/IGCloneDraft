import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BehaviorSubject, Observable, of, ReplaySubject, Subject } from 'rxjs';
import { map } from 'rxjs/operators';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { PaginatedResult } from '../_models/pagination';
import { UserParams } from '../_models/userParams';
import { AccountService } from './account.service';
import { getPaginatedResult, getPaginationHeaders } from './paginationHelper';

@Injectable({
  providedIn: 'root',
})
export class MembersService {
  baseUrl = environment.apiUrl;
  members: Member[] = [];
  // paginatedResult: PaginatedResult<Member[]> = new PaginatedResult<Member[]>();

  constructor(
    private http: HttpClient,
    private accountService: AccountService
  ) {}

  getMembers(userParams: UserParams) {
    let params = getPaginationHeaders(userParams.pageNumber, userParams.itemsPerPage);
    params = params.append('minAge', userParams.minAge.toString());
    params = params.append('maxAge', userParams.maxAge.toString());
    params = params.append('gender', userParams.gender.toString());
    params = params.append('orderBy', userParams.orderBy);

    return getPaginatedResult<Member[]>(this.baseUrl + 'users', params, this.http);
  }
  // those two private methods will be in a separate file because Ill need pagination for other data as well
  // for messages
  // private getPaginatedResult<T>(url, params) {
  //   const paginatedResult: PaginatedResult<T> = new PaginatedResult<T>();

  //   return this.http
  //     .get<T>(url, { observe: 'response', params })
  //     .pipe(
  //       map((response) => {
  //         paginatedResult.result = response.body;
  //         // if there is a header
  //         if (response.headers.get('Pagination') !== null) {
  //           paginatedResult.pagination = JSON.parse(
  //             response.headers.get('Pagination')
  //           );
  //         }
  //         // this means return the paginatedResult which is of type PaginatedResult<T> as observable
  //         return paginatedResult;
  //       })
  //     );
  // }

  // private getPaginationHeaders(pageNumber: number, itemsPerPage: number) {
  //   let params = new HttpParams();
  //   // set up the query params
  //   params = params.append('pageNumber', pageNumber.toString());
  //   params = params.append('itemsPerPage', itemsPerPage.toString());

  //   return params;
  // }

  getMemberById(id: number) {
    const member = this.members.find((m) => m.id === +id);
    // if there is no such item in the members array, call the api
    if (member === undefined) {
      return this.http.get<Member>(this.baseUrl + `users/${id}`);
    }
    // I added this of, creates an observable
    return of(member);
  }

  getMemberByUsername(username: string) {
    const member = this.members.find((m) => m.username === username);
    if (member === undefined) {
      return this.http.get<Member>(this.baseUrl + `users/${username}`);
    }
    // I added this of, creates an observable from localy saved memebers in list
    return of(member);
  }

  updateMember(member: Member) {
    // update on the server
    return this.http.put(this.baseUrl + 'users/update', member).pipe(
      map(() => {
        // update the cashe
        const index = this.members.indexOf(member);
        this.members[index] = member;
      })
    );
  }

  setMainPhoto(photoId: number) {
    return this.http.put(this.baseUrl + 'users/set-main-photo/' + photoId, {});
  }

  deletePhoto(photoId: number) {
    return this.http
      .delete(this.baseUrl + 'users/delete-photo/' + photoId)
      .pipe(
        map((res: { success: boolean }) => {
          if (res.success) {
            // return the result so i can see console.log in place where i subscribe
            return res;
          }
        })
      );
  }

  addLike(username: string) {
    return this.http.post(this.baseUrl + 'likes/' + username, {});
  }

  getLikes(predicates: string) {
    return this.http.get(this.baseUrl + 'likes?predicates=' + predicates);
  }

}
