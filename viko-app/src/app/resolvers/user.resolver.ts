import { ResolveFn, Router } from '@angular/router';
import { UserInfo } from '../interfaces/interfaces';
import { UserService } from '../services/user.service';
import { inject } from '@angular/core';
import { catchError, map, of } from 'rxjs';

export const userResolver: ResolveFn<UserInfo | null> = (route) => {
  const guid = route.paramMap.get('guid');
  const userService = inject(UserService);
  const router = inject(Router);

  // Verifies format of guid before making a request
  if (!guid || guid.length !== 36) {
    router.navigate(['/notfound']);
    return of(null);
  }

  //Loads data from opened event
  //If response != null, returns event and shows its page loaded
  //If response IS null, prevents page from loading and sends to error page
  return userService.viewUser(guid).pipe(
    map((user) => {
      if (!user) {
        router.navigate(['/notfound']);
        return null;
      }
      return user;
    }),
    catchError(() => {
      router.navigate(['/notfound']);
      return of(null);
    })
  );
};
