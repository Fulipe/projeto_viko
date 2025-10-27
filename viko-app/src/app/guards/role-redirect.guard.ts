import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleRedirectGuard: CanActivateFn = (route, state) => {
const router = inject(Router);
  const authService = inject(AuthService);

  const role = authService.getRole();

  switch (role) {
    case 'Student':
      router.navigate(['/private/dashboard/student']);
      break;
    case 'Teacher':
      router.navigate(['/private/dashboard/teacher']);
      break;
    case 'Admin':
      router.navigate(['private/admin']);
      break;
    default:
      router.navigate(['/unauthorized']);
      break;
  }

  return false; // impede o carregamento de /dashboard
};
