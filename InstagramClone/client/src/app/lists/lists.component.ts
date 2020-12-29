import { Component, OnInit } from '@angular/core';
import { Router } from '@angular/router';
import { Member } from '../_models/member';
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

  constructor(private memeberService: MembersService, private router: Router) { }

  ngOnInit() {
    this.loadLikes();
  }

  loadLikes() {
    this.memeberService.getLikes(this.predicates).subscribe((response: Partial<Member[]>) => {
      this.members = response;
      this.members.forEach(el => {
        console.log('knowns as:', el.knownAs);
      });
    }, error => console.log(error))
  }

  selectMember(memberId: number) {
    return this.router.navigate(['/members/' + memberId]);
  }

}
