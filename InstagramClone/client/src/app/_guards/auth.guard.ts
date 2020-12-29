import { Injectable } from '@angular/core';
import { CanActivate, ActivatedRouteSnapshot, RouterStateSnapshot, UrlTree } from '@angular/router';
import { Observable } from 'rxjs';
import { map } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';
import { AlertifyService } from '../_services/alertify.service';

@Injectable({
  providedIn: 'root'
})
export class AuthGuard implements CanActivate {
  constructor(private accountService: AccountService, private alertify: AlertifyService) {}
  // CanActivate automatically subscribes to an observable
  // I dont like this approach with Observable<boolean>
  // prefer the one with bool in skinet and previous version dating app
  canActivate(): Observable<boolean> {
    return this.accountService.currentUser$.pipe(
      map(user => {
        if (user) {
          return true;
        }
        this.alertify.error('You shall not pass');
      })
    );
  }

}
