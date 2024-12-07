import { CanActivateFn } from '@angular/router';
import { AccountService } from '../_services/account.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';

export const vendorGuard: CanActivateFn = (route, state) => {
  const accountService = inject(AccountService);
  const toastr = inject(ToastrService);
  if (accountService.roles().includes('Vendor')){
    return true;
  }
  else {
    toastr.error('You need to be an admin or a moderator to access this page.');
    return false;
  }
};
