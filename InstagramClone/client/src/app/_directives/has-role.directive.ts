import { OnInit } from '@angular/core';
import { Directive, Input, TemplateRef, ViewContainerRef } from '@angular/core';
import { take } from 'rxjs/operators';
import { User } from '../_models/user';
import { AccountService } from '../_services/account.service';

@Directive({
  selector: '[appHasRole]' // *appHasRole='["Admin","Moderator"]', this will be structural directive
})
export class HasRoleDirective implements OnInit{
  @Input() appHasRole: string[] = [];
  user: User;

  constructor(private viewContainerRef: ViewContainerRef, private templateRef: TemplateRef<any>,
              private accountService: AccountService) {
                this.accountService.currentUser$.pipe(take(1)).subscribe(user => {
                  this.user = user;
                });
               }

  ngOnInit(): void {
    // clear the view if the user has no roles
    if (!this.user.roles || this.user == null) {
      this.viewContainerRef.clear();
      return;
    }

    // if the user has some role that is in the list
    if (this.user.roles.some(r => this.appHasRole.includes(r))) {
      this.viewContainerRef.createEmbeddedView(this.templateRef);
    } else {
      this.viewContainerRef.clear();
    }
  }

}
// read this on how to use structural directives with multiple inputs, from question section on the lecture
// https://stackoverflow.com/questions/41789702/how-to-use-angular-structural-directive-with-multiple-inputs
