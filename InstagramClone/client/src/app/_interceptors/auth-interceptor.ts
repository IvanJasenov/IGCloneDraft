import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { AccountService } from '../_services/account.service';
import { AlertifyService } from '../_services/alertify.service';

@Injectable()

export class AuthInterceptor implements HttpInterceptor {
  constructor(private accountService: AccountService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler) {
    const authToken = this.accountService.getToken();

    const authRequest = req.clone({
      headers: req.headers.set('Authorization', 'Bearer ' + authToken),
    });

    return next.handle(authRequest);
  }
}
