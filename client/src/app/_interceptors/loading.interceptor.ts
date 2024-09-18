import { HttpEvent, HttpHandler, HttpInterceptor, HttpInterceptorFn, HttpRequest } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { BusyService } from '../_services/busy.service';
import { delay, finalize, identity, Observable } from 'rxjs';
import { environment } from '../../environments/environment';
@Injectable()
export class LoadingInterceptor implements HttpInterceptor{
  constructor(private busyService: BusyService){}
  intercept(req: HttpRequest<unknown>, next: HttpHandler): Observable<HttpEvent<unknown>> {
    this.busyService.busy();
    return next.handle(req).pipe(
      (environment.production ? identity : delay(1000)),
      finalize(() => this.busyService.idle())
    );
  }
}
