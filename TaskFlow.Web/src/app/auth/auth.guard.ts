import { inject } from '@angular/core';
import { CanMatchFn, RedirectCommand, Router } from '@angular/router';
import { AuthService } from './auth.service';
import { map, take } from 'rxjs';

export const AuthGuard: CanMatchFn = (route, segments) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  return authService.user.pipe(
    take(1),
    map((user) => {
      if (user) {
        return true; // User is authenticated, allow access
      }
      return new RedirectCommand(router.parseUrl('/auth'));
    })
  );
};
