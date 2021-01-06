import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { DeleteUserLike } from 'src/app/_models/InstagramPhotos/deleteUserLike';
import { Member } from 'src/app/_models/member';
import { AccountService } from 'src/app/_services/account.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-member-card',
  templateUrl: './member-card.component.html',
  styleUrls: ['./member-card.component.css']
})
export class MemberCardComponent implements OnInit {

  @Input() member: Member;
  @Input() showLikeButton: boolean;
  @Input() canUnlikeMember: boolean;
  constructor(private router: Router, private memberService: MembersService,
              private alertify: AlertifyService, private accountService: AccountService, private presence: PresenceService,) { }

  ngOnInit() {}


  selectMember(id: number) {
    this.router.navigate([`instagramMembers/${this.member.username}`]);
    // this.memberService.userId.next(id);
    // this.router.navigate(['memberDetals']);

  }

  addLike(member: Member) {
    this.memberService.addLike(member.username).subscribe(res => {
      console.log('res:', res);
      this.alertify.success('You liked user:' + member.knownAs);
      console.log('Users I follow:', res);
    }, error => console.log('error:', error));
  }

  removeLike() {
    console.log('remove like for memeber:', this.member);
    this.memberService.deleteLike(this.member.username).subscribe((res: DeleteUserLike) => {
      console.log('res from unlike:', res);
      if (res.success) {
        console.log('unliked member with username:', this.member.username);
        // fire up the emiter
        this.accountService.getLikedUsersByLogedinUser();
      }
    }, error => console.log('error:', error));
  }
}
