import { HttpEvent, HttpHandler, HttpInterceptor, HttpInterceptorFn, HttpRequest } from "@angular/common/http";
import { inject, Injectable } from "@angular/core";
import { Observable, take } from "rxjs";
import { AccountService } from "../_services/account.service";

export const JwtInterceptor : HttpInterceptorFn = (req, next) =>{
  const accountService = inject(AccountService);
  if (accountService.currentUser()) {
    req = req.clone({
      setHeaders: {
        Authorization: `Bearer ${accountService.currentUser()?.token}`
      }
    });
  }
  return next(req);
};
