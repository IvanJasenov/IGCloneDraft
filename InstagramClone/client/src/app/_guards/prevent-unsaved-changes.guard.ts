import { Injectable } from '@angular/core';
import { CanDeactivate } from '@angular/router';
import { Observable } from 'rxjs';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';


@Injectable({
  providedIn: 'root'
})
export class PreventUnsavedChanges implements CanDeactivate<MemberEditComponent> {

  constructor(private confirmService: ConfirmService) {}

  canDeactivate(component: MemberEditComponent): Observable<boolean> | boolean {
    if (component.madeChanges) {
      // return confirm('Are you sure you want to continue? Any unsaved changes will be lost!');
      // try adding router to modal component
      return this.confirmService.confirm(); // in guards its gonna automatically subscribe for us, we dont need to explicitly subscribe
    }
    return true;
  }
}

