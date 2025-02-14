import { HttpErrorResponse, HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from '@angular/common/http';

import { Injectable } from '@angular/core';
import { NavigationExtras, Router } from '@angular/router';
import { ToastrService } from 'ngx-toastr';
import { catchError, Observable } from 'rxjs';

@Injectable()
export class errorInterceptor implements HttpInterceptor{
  constructor(private router: Router, private toastr: ToastrService){}
  intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    return next.handle(req).pipe(
      catchError((error : HttpErrorResponse) => {
        if (error){
          console.log(error);
          switch (error.status){
            case 400:
              if (error.error.errors){
                const modelStateErrors = [];
                for (const key in error.error.errors){
                  if (error.error.errors[key]){
                    modelStateErrors.push(error.error.errors[key]);
                  }
                }
                throw modelStateErrors;
              } else {
                this.toastr.error(error.error, error.status.toString());
              }
              break;
            case 401:{
              // this.toastr.error('Unauthorized', error.status.toString());
              break;
            }
            case 404:
              this.router.navigateByUrl('/not-found');
              break;
            case 500:
              const navigationExtras: NavigationExtras = {state: {error: error.error}};
              this.router.navigateByUrl('/server-error', navigationExtras);
              break;
            default:
              this.toastr.error('An unexpected error occurred');
              console.log(error);
              break;
          }
        }
        throw error;
      })
    )
  }
}
