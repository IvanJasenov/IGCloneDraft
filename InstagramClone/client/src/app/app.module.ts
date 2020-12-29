import { TabsModule } from 'ngx-bootstrap/tabs';
import { NgModule } from '@angular/core';
import { AppComponent } from './app.component';
import { NavComponent } from './nav/nav.component';
import { HomeComponent } from './home/home.component';
import { RegisterComponent } from './register/register.component';
import { MemberListComponent } from './members/member-list/member-list.component';
import { MemberDetailsComponent } from './members/member-details/member-details.component';
import { ListsComponent } from './lists/lists.component';
import { MessagesComponent } from './messages/messages.component';
import { SharedModule } from './_modules/shared.module';
import { TestErrorsComponent } from './errors/test-errors/test-errors.component';
import { ErrorInterceptor, ErrorInterceptorProvider } from './_interceptors/error.interceptor';
import { MemberCardComponent } from './members/member-card/member-card.component';
import { AuthInterceptor } from './_interceptors/auth-interceptor';
import { TimeAgoPipe } from 'time-ago-pipe';
import { HTTP_INTERCEPTORS } from '@angular/common/http';
import { MemberMessagesComponent } from './members/member-messages/member-messages.component';
import { MemberEditComponent } from './members/member-edit/member-edit.component';
import { PreventUnsavedChanges } from './_guards/prevent-unsaved-changes.guard';
import { LoadingInterceptor } from './_interceptors/loading-interceptor';
import { PhotoEditorComponent } from './members/photo-editor/photo-editor.component';
import { TextInputComponent } from './_forms/text-input/text-input.component';
import { DateInputsComponent } from './_forms/date-inputs/date-inputs.component';
import { AdminPanelComponent } from './admin/admin-panel/admin-panel.component';
import { HasRoleDirective } from './_directives/has-role.directive';
import { UserManagementComponent } from './admin/admin-components/user-management/user-management.component';
import { PhotoManagementComponent } from './admin/admin-components/photo-management/photo-management.component';
import { RolesModalComponent } from './modals/roles-modal/roles-modal.component';
import { ConfirmDialogComponent } from './modals/confirm-dialog/confirm-dialog.component';
import { ModalModule } from 'ngx-bootstrap/modal';
import { PhotoCardComponent } from './photos/photo-card/photo-card.component';
import { PhotoListComponent } from './photos/photo-list/photo-list.component';
import {ScrollingModule} from '@angular/cdk/scrolling';
import { InstagramMemberCardComponent } from './instagram-members/instagram-member-card/instagram-member-card.component';
import { InstagramMemberDetailsComponent } from './instagram-members/instagram-member-details/instagram-member-details.component';
import { InstagramMemberEditComponent } from './instagram-members/instagram-member-edit/instagram-member-edit.component';
import { InstagramPhotoCardComponent } from './instagram-members/instagram-photo-card/instagram-photo-card.component';
import { InstagramImageModalComponent } from './modals/instagram-image-modal/instagram-image-modal.component';
import { ImageModalPhotoComponent } from './instagram-members/image-modal-photo/image-modal-photo.component';
import { TooltipModule } from 'ngx-bootstrap/tooltip';
import { FormsModule } from '@angular/forms';


@NgModule({
  declarations: [
    AppComponent,
    NavComponent,
    HomeComponent,
    RegisterComponent,
    MemberListComponent,
    MemberDetailsComponent,
    ListsComponent,
    MessagesComponent,
    TestErrorsComponent,
    MemberCardComponent,
    TimeAgoPipe,
    MemberMessagesComponent,
    MemberEditComponent,
    PhotoEditorComponent,
    TextInputComponent,
    DateInputsComponent,
    AdminPanelComponent,
    HasRoleDirective,
    UserManagementComponent,
    PhotoManagementComponent,
    RolesModalComponent,
    ConfirmDialogComponent,
    PhotoCardComponent,
    PhotoListComponent,
    InstagramMemberCardComponent,
    InstagramMemberDetailsComponent,
    InstagramMemberEditComponent,
    InstagramPhotoCardComponent,
    InstagramImageModalComponent,
    ImageModalPhotoComponent
  ],
  imports: [
    SharedModule,
    ScrollingModule,
    // FormsModule,
    TabsModule.forRoot(),
    ModalModule.forRoot(),
    TooltipModule.forRoot()
  ],
  providers: [
    PreventUnsavedChanges,
    {provide: HTTP_INTERCEPTORS, useClass: AuthInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: ErrorInterceptor, multi: true},
    {provide: HTTP_INTERCEPTORS, useClass: LoadingInterceptor, multi: true},
  ],
  entryComponents: [
    RolesModalComponent,
    ConfirmDialogComponent,
    InstagramImageModalComponent
  ],
  bootstrap: [AppComponent]
})
export class AppModule { }
