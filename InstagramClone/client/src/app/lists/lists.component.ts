import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { LikeDto } from '../_models/InstagramPhotos/likesDto';
import { Member } from '../_models/member';
import { AccountService } from '../_services/account.service';
import { MembersService } from '../_services/members.service';

@Component({
  selector: 'app-lists',
  templateUrl: './lists.component.html',
  styleUrls: ['./lists.component.css']
})
export class ListsComponent implements OnInit {
  // Partial is new, makes all properties option
  members: Partial<Member[]>;
  predicates = 'liked';
  likedUsersByLogedInUser: LikeDto[] = [];
  canUnlikeMembers = false;


  constructor(private memeberService: MembersService, private router: Router, private accountService: AccountService) {
    // listen for changes
    this.accountService.likedUsersByLogedInUser$.subscribe((res: LikeDto[]) => {
      if (res) {
        this.likedUsersByLogedInUser = res;
        this.loadLikes();
        console.log('Users I follow after change:', res);
      }
    }, error => console.log('error:', error));
   }

  ngOnInit() {
    this.loadLikes();
  }

  loadLikes() {
    this.memeberService.getLikes(this.predicates).subscribe((response: Partial<Member[]>) => {
      if (this.predicates === 'liked') {
        this.canUnlikeMembers = true;
      }
      if (this.predicates === 'likedBy') {
        this.canUnlikeMembers = false;
      }
      this.members = response;
      this.members.forEach(el => {
        console.log('knowns as:', el.knownAs);
      });
    }, error => console.log(error))
  }

  selectMember(memberId: number) {
    return this.router.navigate(['/members/' + memberId]);
  }

  // subscribes to the stream from login
  loadUsersIFollow() {
    this.accountService.likedUsersByLogedInUser$.subscribe((res: LikeDto[]) => {
      if (res) {
        this.likedUsersByLogedInUser = res;
        console.log('Users I follow:', res)
      }
    }, error => console.log('error:', error));
  }

}
