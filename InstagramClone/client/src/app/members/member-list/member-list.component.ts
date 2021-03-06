import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
import { LikeDto } from 'src/app/_models/InstagramPhotos/likesDto';
import { Member } from 'src/app/_models/member';
import { IPagination, PaginatedResult } from 'src/app/_models/pagination';
import { User } from 'src/app/_models/user';
import { UserParams } from 'src/app/_models/userParams';
import { AccountService } from 'src/app/_services/account.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit {

  members: Member[] = [];
  pagination: IPagination;
  pageNumber = 1;
  itemsPerPage = 5;
  totalItems: number;
  userParams: UserParams;
  user: User;
  genderList = [{value: 'male', displayName: 'Male'}, {value: 'female', displayName: 'Female'}];
  likedUsersByLogedInUser: LikeDto[] = [];
  likedUserNames: string[] = [];

  constructor(private memberService: MembersService, private accountService: AccountService) {
    // takes the current user in the ctor
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    });

    this.accountService.likedUsersByLogedInUser$.subscribe((res: LikeDto[]) => {
      if (res) {
        this.likedUsersByLogedInUser = res;
        console.log('Users I follow:', res);
        // clean list
        this.likedUserNames = [];
        this.likedUsersByLogedInUser.forEach(el => {
          if (this.likedUserNames.findIndex(k => k === el.username) === -1) {
            this.likedUserNames.push(el.username);
          }
        });
        console.log('likedUsernames:', this.likedUserNames);
      }
    }, error => console.log('error:', error));
   }

  ngOnInit() {
    this.loadMembers();
    this.loadUsersIFollow();
  }

  loadMembers() {
    this.memberService.getMembers(this.userParams).subscribe((response: PaginatedResult<Member[]>) => {
      this.members = response.result;
      this.pagination = response.pagination;
      this.totalItems = this.pagination.totalItems;
      console.log('total items:', response);
    }, error => console.log(error));
  }

  pageChanged(event: any) {
    this.userParams.pageNumber = event.page;
    // get the next batch of members
    this.loadMembers();
  }

  resetFilters() {
    this.userParams = new UserParams(this.user);
    this.loadMembers();
  }

  loadUsersIFollow() {
    this.accountService.likedUsersByLogedInUser$.subscribe((res: LikeDto[]) => {
      if (res) {
        this.likedUsersByLogedInUser = res;
        console.log('Users I follow:', res);
        this.likedUsersByLogedInUser.forEach(el => {
          if (this.likedUserNames.findIndex(k => k === el.username) === -1) {
            this.likedUserNames.push(el.username);
          }

          console.log('likedUsernames:', this.likedUserNames);
        });
      }
    }, error => console.log('error:', error));
  }

  canBeLiked(username: string): boolean {
    return this.likedUserNames.findIndex(el => el === username) !== -1 ? true : false;
  }

}
