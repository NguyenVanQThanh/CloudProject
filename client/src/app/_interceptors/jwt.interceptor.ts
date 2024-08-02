import { HttpEvent, HttpHandler, HttpInterceptor, HttpRequest } from "@angular/common/http";
import { Injectable } from "@angular/core";
import { Observable, take } from "rxjs";
import { AccountService } from "../_services/account.service";

@Injectable()
export class JwtInterceptor implements HttpInterceptor {
  constructor(private accountService : AccountService){}
  intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next: user => {
        if (user){
          console.log("Yes");
          req = req.clone({
            setHeaders: {
              Authorization: `Bearer ${user.token}`
            }
          })
        }
      }
    })
    return next.handle(req);
  }


}
