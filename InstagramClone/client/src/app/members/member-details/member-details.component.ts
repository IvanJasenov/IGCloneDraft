import { PresenceService } from './../../_services/presence.service';
import { Component, OnDestroy, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { setTime } from 'ngx-bootstrap/chronos/utils/date-setters';
import { TabDirective, TabsetComponent } from 'ngx-bootstrap/tabs';
import { NgxGalleryAnimation } from 'ngx-gallery';
import { NgxGalleryImage } from 'ngx-gallery/ngx-gallery-image.model';
import { NgxGalleryOptions } from 'ngx-gallery/ngx-gallery-options.model';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-details',
  templateUrl: './member-details.component.html',
  styleUrls: ['./member-details.component.css'],
})
export class MemberDetailsComponent implements OnInit {
  member: Member;
  userId: number;
  galleryOptions: NgxGalleryOptions[];
  galleryImages: NgxGalleryImage[];
  // in Angular 8 it has to be this way declared
  @ViewChild('memberTabs', { static: false }) memberTabs: TabsetComponent;
  tabSelected: string;
  loadMessages: boolean;
  loadPhotos: boolean;

  constructor(
    private route: ActivatedRoute,
    private memberService: MembersService,
    private activatedRoute: ActivatedRoute, private presence: PresenceService) {}

  ngOnInit() {
    // load the user and set the ngx-gallery in one function - my approach
    this.loadUser();
  }

  loadUser() {
    // i get those params from  [queryParams]="{tab: 3}" clikc in buttons
    const userId = this.route.snapshot.params.id;
    console.log('load user with id:', userId);
    if (userId && userId > 0) {
      this.memberService.getMemberById(userId).subscribe(
        (res: Member) => {
          this.member = res;
          console.log('user detail:', this.member);
          // fill and set ngx-gallery
          this.galleryImages = this.getImages(this.member);
          this.setGaleryOtions();
          // get  [queryParams]="{tab: 3}" from member-card.component.html
          this.selectMessageTab();
        },
        (error) => console.log('error laoding member:', error)
      );
    }
  }

  getImages(member: Member) {
    const imageUrls = [];
    member.photosDto.forEach(el => {
      imageUrls.push({
        small: el.url,
                medium: el.url,
                big: el.url,
                description: 'image description'
      });
    });

    return imageUrls;
  }

  setGaleryOtions() {
    this.galleryOptions = [
      {
            width: '500px',
            height: '400px',
            thumbnailsColumns: 4,
            imageAnimation: NgxGalleryAnimation.Slide,
            preview: false
        },
      ];
  }
  // the point of this is to load the messages, photos when tab is
  // not when the entire component is loaded
  onSelect(data: TabDirective): void {
    this.tabSelected = data.heading;
    console.log('selected tab:', this.tabSelected);
    if (this.tabSelected === 'Messages') {
      this.loadMessages = true;
    }
    if (this.tabSelected === 'Photos') {
      this.loadPhotos = true;
    }
  }

  selectTab(tabId: number) {
    // this is neat code, read the docs and notes in the word file for the task
    this.memberTabs.tabs[tabId].active = true;
  }

  selectMessageTab() {
    // subscribe to  [queryParams]="{tab: 3}" from member-card.component.html
    this.activatedRoute.queryParams.subscribe(response => {
      console.log('query params:', response.tab);
      if (response.tab !== undefined) {
        // had to add some timeout in order for the component templete to be rendered
         setTimeout(() => {
            this.selectTab(+response.tab);
          }, 300);
      }
    });
  }

}


