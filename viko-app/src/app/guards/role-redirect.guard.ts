import { inject } from '@angular/core';
import { CanActivateFn, Router, ActivatedRouteSnapshot } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleRedirectGuard: CanActivateFn = (route: ActivatedRouteSnapshot, state) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  const role = authService.getRole();

  if (!role) {
    router.navigate(['/login']);
    return false;
  }

  // Gets redirect map defined in the routes (if it exists)
  const redirectMap = route.data?.['redirectMap'] as Record<string, string> | undefined;

  // Gets GUID (if exists) 
  const guid = route.params?.['guid'];

  //If theres a redirect map, aplies it with a dynamic substituition of the :guid
  if (redirectMap && redirectMap[role]) {
    const redirectPath = redirectMap[role].replace(':guid', guid ?? '');
    router.navigateByUrl(redirectPath);
    return false;
  }

  //If theres not a redirectMap, does the default behavior
  switch (role) {
    case 'Student':
      router.navigate(['/private/student']);
      break;
    case 'Teacher':
      router.navigate(['/private/teacher']);
      break;
    case 'Admin':
      router.navigate(['/private/admin']);
      break;
    default:
      router.navigate(['/unauthorized']);
      break;
  }

  // Stops the loading of the original routes (because theres a redirect)
  return false;
};
