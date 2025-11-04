import { inject } from '@angular/core';
import { CanActivateFn, Router } from '@angular/router';
import { AuthService } from '../services/auth.service';

export const roleRedirectGuard: CanActivateFn = (route, state) => {
  const router = inject(Router);
  const authService = inject(AuthService);

  const role = authService.getRole();

  if (!role) {
    router.navigate(['/login']);
    return false;
  }

  // Permite definir redirecionamentos específicos via data
  const redirectMap = route.data?.['redirectMap'] as Record<string, string> | undefined;

  // Se existir um redirectMap, tenta aplicar o caminho definido para o role
  if (redirectMap && redirectMap[role]) {
    router.navigate([redirectMap[role]]);
    return false;
  }

  // Caso contrário, aplica o comportamento padrão
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
  return false; // impede o carregamento de /dashboard
};
