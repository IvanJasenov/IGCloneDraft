import { Injectable } from '@angular/core';
import { HttpInterceptor, HttpErrorResponse, HTTP_INTERCEPTORS, HttpRequest, HttpHandler, HttpEvent } from '@angular/common/http';
import { throwError, Observable } from 'rxjs';
import { catchError } from 'rxjs/operators';
import { AlertifyService } from '../_services/alertify.service';
import { Router } from '@angular/router';

@Injectable()
export class ErrorInterceptor implements HttpInterceptor {

  constructor(private router: Router, private alertify: AlertifyService) {}

  intercept(req: HttpRequest<any>, next: HttpHandler ): Observable<HttpEvent<any>> {

    return next.handle(req).pipe(
      // catching the error from the Response
      catchError(error => {
        // now lets deal with error types
        if (error.status === 401) {
          this.alertify.error('Unauthorized');
          return throwError(error.statusText);
        }
        // deals with 500 types of error
        if (error instanceof HttpErrorResponse){
          this.alertify.error('Something went wrong');
          const applicationError = error.headers.get('Application-Error');
          if (applicationError) {
            return throwError(applicationError);
          }
          // lets handle models state errors
          const serverError = error.error;
          let modelStateErrors = '';
          if (serverError.error && typeof serverError.errors === 'object') {
            for (const key in serverError.errors) {
              if (serverError.errors[key]) {
                modelStateErrors += serverError.errors[key] + '\n';
                this.alertify.error(key);
              }
            }
          }
          return throwError(modelStateErrors || serverError || 'Server Error');
        }
      })
    );
  }
}


export const ErrorInterceptorProvider = {
  provide: HTTP_INTERCEPTORS,
  useClass: ErrorInterceptor,
  multi: true
};
