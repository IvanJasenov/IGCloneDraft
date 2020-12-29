import { HttpClient } from '@angular/common/http';
import { Component, OnInit } from '@angular/core';
import { take } from 'rxjs/operators';
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

  constructor(private memberService: MembersService, private accountService: AccountService) {
    // takes the current user in the ctor
    this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
      this.user = user;
      this.userParams = new UserParams(user);
    });
   }

  ngOnInit() {
    this.loadMembers();
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

}
