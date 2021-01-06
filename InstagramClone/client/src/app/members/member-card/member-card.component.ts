import { Component, Input, OnDestroy, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Member } from 'src/app/_models/member';
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
              private alertify: AlertifyService, private presence: PresenceService) { }

  ngOnInit() {}



  // this is my way, programticlly navigate to a route so I dont use the ActivatedRooute,
  // and then taka a snapshot of the url
  selectMember(id: number) {
    this.router.navigate([`instagramMembers/${this.member.username}`]);
    // this.memberService.userId.next(id);
    // this.router.navigate(['memberDetals']);

  }

  addLike(member: Member) {
    this.memberService.addLike(member.username).subscribe(res => {
      console.log('res:', res);
      this.alertify.success('You liked user:' + member.knownAs);
    }, error => console.log('error:', error));
  }

  removeLike() {
    console.log('remove like for memeber:', this.member);
  }
}
