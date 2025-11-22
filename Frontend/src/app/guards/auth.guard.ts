import { CanActivateFn, CanActivateChildFn, Router } from '@angular/router';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';

const checkAuth = () => {
  const authService = inject(AuthService);
  const router = inject(Router);

  if (authService.getToken()) {
    return true;
  }

  router.navigate(['/login']);
  return false;
};

export const authGuard: CanActivateFn = () => checkAuth();
export const authChildGuard: CanActivateChildFn = () => checkAuth();
