import { Component, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { TabsetComponent, TabDirective } from 'ngx-bootstrap/tabs';
import { Member } from 'src/app/_models/member';
import { AccountService } from 'src/app/_services/account.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-instagram-member-details',
  templateUrl: './instagram-member-details.component.html',
  styleUrls: ['./instagram-member-details.component.css']
})
export class InstagramMemberDetailsComponent implements OnInit {
  username: string;
  member: Member;
  followers: Member[];
  followingPeople: Member[];
  @ViewChild('memberTabs', { static: false }) memberTabs: TabsetComponent;
  tabSelected: string;
  loadPhotos: boolean;
  localStorageUsername: string;

  constructor(private activatedRoute: ActivatedRoute, private memberService: MembersService,
    private alertify: AlertifyService, private accountService: AccountService) { }

  ngOnInit() {

    setTimeout(() => {this.loadMember();this.getCurrentUser();}, 400)

  }

  loadMember() {
    this.username = this.activatedRoute.snapshot.params.username;
    console.log('username:', this.username)
    this.memberService.getMemberByUsername(this.username).subscribe((res: Member) => {
      this.member = res;
      console.log('loaded memeber:', this.member);
      this.getFollowers('likedBy');
      this.getFollowers('liked');
    }, error => console.log('error:', error));

  }

  followUser(memberUsername: string) {
    this.memberService.addLike(memberUsername).subscribe(res => {
      console.log('res:', res);
      this.alertify.success('You liked user:' + this.member.knownAs);
    }, error => console.log('error:', error));
  }

  getFollowers(predicates: string) {
    this.memberService.getLikes(predicates).subscribe((res: Member[]) => {

      if (predicates === 'likedBy') {
        console.log('liked by:', res.length);
        this.followers = res;
        console.log('followers:', this.followers);
      }
      else if (predicates === 'liked') {
        console.log('Following:', res.length);
        this.followingPeople = res;
        console.log('following:', this.followingPeople);
      }

    }, error => console.log('error:', error))
  }

  getCurrentUser() {
    this.localStorageUsername =  this.accountService.getCurrentUser().username;
  }

  onSelect(data: TabDirective): void {
    this.tabSelected = data.heading;
    console.log('selected tab:', this.tabSelected);
    if (this.tabSelected === 'Photos') {
      this.loadPhotos = true;
    }
  }

  reloadInstagramMember(reload: boolean) {
    console.log('reload memeber:', reload);
    if (reload) {
      this.ngOnInit();
    }
  }

}
