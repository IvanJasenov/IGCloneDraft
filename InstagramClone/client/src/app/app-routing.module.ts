import { NgModule } from '@angular/core';
import { Routes, RouterModule } from '@angular/router';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { HomeComponent } from './home/home.component';
import { InstagramMemberDetailsComponent } from './instagram-members/instagram-member-details/instagram-member-details.component';
import { InstagramMemberEditComponent } from './instagram-members/instagram-member-edit/instagram-member-edit.component';
import { ListsComponent } from './lists/lists.component';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MessagesComponent } from './messages/messages.component';
import { PhotoListComponent } from './photos/photo-list/photo-list.component';
import { AdminGuard } from './_guards/admin.guard';
import { AuthGuard } from './_guards/auth.guard';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';


const routes: Routes = [
  {path: '', component: HomeComponent},
  {path: 'instagram-photos', component: PhotoListComponent},
  {path: 'members', component: MemberListComponent},
  {path: 'members/:id', component: MemberDetailsComponent},
  {path: 'member/edit', component: MemberEditComponent, canActivate: [AuthGuard], canDeactivate: [PreventUnsavedChanges]},
  {path: 'memberDetals', component: MemberDetailsComponent},

  // those are rendered in alphatecal order so edit must be first or modify
  {path: 'instagramMembers/:username', component: InstagramMemberDetailsComponent},
  {path: 'instagramMember/edit', component: InstagramMemberEditComponent, canActivate: [AuthGuard]},



  {path: 'lists', component: ListsComponent},
  {path: 'messages', component: MessagesComponent},
  {path: 'admin', component: AdminPanelComponent, canActivate: [AdminGuard]},
  // wildcard , if anythinh does not match here will be redirected to the home component, path
  // {path: '**', redirectTo: '', pathMatch: 'full'}
  {path: 'errors', component: TestErrorsComponent},
  {path: '**', component: HomeComponent, pathMatch: 'full'}
];

@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
