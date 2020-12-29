import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { delay, finalize } from 'rxjs/operators';
import { AccountService } from '../_services/account.service';
import { AlertifyService } from '../_services/alertify.service';
import { SpinnerService } from '../_services/spinner.service';

@Injectable()

export class LoadingInterceptor implements HttpInterceptor {
  constructor(private spinnerService: SpinnerService) {}


  intercept(req: HttpRequest<any>, next: HttpHandler): Observable<HttpEvent<any>> {
    this.spinnerService.loadSpinner();
    return next.handle(req).pipe(
      delay(1000), // delay the request for some time
      finalize(() => { // after the delay, execute this
        this.spinnerService.hideSpinner();
      })
    );
  }


}
