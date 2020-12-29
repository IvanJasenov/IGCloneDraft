import { AfterViewInit, Component, ElementRef, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm, NgModel } from '@angular/forms';
import { Router } from '@angular/router';
import { NgxSpinnerService } from 'ngx-spinner';
import { take } from 'rxjs/operators';
import { Member } from 'src/app/_models/member';
import { User } from 'src/app/_models/user';
import { AccountService } from 'src/app/_services/account.service';
import { AlertifyService } from 'src/app/_services/alertify.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  member: Member;
  user: User;
  madeChanges: boolean;
  originalintroduction: string;
  originallookingFor: string;
  originalinterests: string;
  originalcity: string;
  originalcountry: string;

  introductionChange: boolean;
  lookingForChange: boolean;
  interestsChange: boolean;
  cityChange: boolean;
  countryChange: boolean;
  // in order to access the form's elements we need to use the ViewChild decorator
  @ViewChild('editForm', {static: true}) editForm: NgForm; // a reference to an NgForm element

  // prevents from closing up the browser, Angular has control just over the root
  // this works smootly in Chrome, but in Opera developers tools has to be opened in order to work
  @HostListener('window:beforeunload', ['$event'])
  unloadNotification($event: any) {
    if (this.madeChanges === true) {
     $event.returnValue = true;
    }
  }

  constructor(private accountService: AccountService, private memberService: MembersService,
              private route: Router, private alertify: AlertifyService) { }

  ngOnInit() {
    this.getCurrentUser();
  }

  getCurrentUser() {
    // this is nice, pipe(take(1)), just take one value from the ReplaySubject buffer
    this.accountService.currentUser$.pipe(take(1)).subscribe((user: User) => {
      this.user = user;
      this.loadMember(user);
    }, error => console.log('error:', error));
  }

  // ping the api
  loadMember(user: User) {
   this.memberService.getMemberByUsername(user.username).subscribe((member: Member) => {
     this.member = member;
     console.log('member for edit:', member);
     // save the original values in a local variable
     this.loadOriginalValues();
   }, error => {
     console.log('error:', error.error);
   });
  }


  updateMember() {
    console.log('update memeber', this.member);
    // call the api and update member
    this.memberService.updateMember(this.member).subscribe(res => {
      console.log('response:', res);
      this.madeChanges = false;
      this.route.navigate([`members/${this.member.id}`]);
      this.alertify.success('Profile Updated successfully');
    }, error => console.log('error:', error));
  }

  // this get invoked on every ngModelChange of the input, textarea in this case
  detectInputChange(event: string, origin: string) {
    if (event.length > 0) {
      this.madeChanges = true;
      // tslint:disable-next-line: no-eval
      eval(`this.${origin}Change = true`);
      // console.log('changes detected', event);
      const original = this.member[`${origin}`];
      console.log('targer:', origin);
      // if there is Something in the object property, memeber.<something>
      if (original.length >= 0 && event.length >= 0) {
        // detecte changes
        if (original.length > 0) {
        const command = `this.${origin} = true`;
        // tslint:disable-next-line: no-eval
        eval(command);
        }
      }
    }
  }

  loadOriginalValues() {
    this.originalintroduction = this.member.introduction;
    this.originallookingFor = this.member.lookingFor;
    this.originalinterests = this.member.interests;
    this.originalcity = this.member.city;
    this.originalcountry = this.member.country;
  }

  cancelChanges(origin: string) {
    console.log('cancel changes at origin:', origin);
    // tslint:disable-next-line: no-eval
    console.log('original value:', eval(`this.original${origin}`));
    // replace the changed value with the recoreded one when component is loaded
    // tslint:disable-next-line: no-eval
    this.member[`${origin}`] = eval(`this.original${origin}`);
    // change the boolen for individual changes
    // tslint:disable-next-line: no-eval
    eval(`this.${origin}Change = false`);
    this.madeChanges = false;
  }

}
