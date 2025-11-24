import { ResolveFn, Router } from '@angular/router';
import { EventFetched } from '../interfaces/interfaces';
import { EventService } from '../services/event.service';
import { inject } from '@angular/core';
import { catchError, map, of } from 'rxjs';
import { AuthService } from '../services/auth.service';

export const eventResolver: ResolveFn<EventFetched | null> = (route) => {
  const guid = route.paramMap.get('guid');
  const eventsService = inject(EventService);
  const authService = inject(AuthService)
  const router = inject(Router);

  // Verifies format of guid before making a request
  if (!guid || guid.length !== 36) {
    router.navigate(['/notfound']);
    return of(null);
  }

  //Loads data from opened event
  //If response != null, returns event and shows its page loaded
  //If response IS null, prevents page from loading and sends to error page
  return eventsService.GetEvent(guid).pipe(
    map((event) => {
      if (!event) {
        router.navigate(['/notfound']);
        return null;
      } 
      if (!event.isViewed && authService.getRole() == "Student") {
        router.navigate(['/forbidden']);
        return null;        
      }
      return event;
    }),
    catchError(() => {
      router.navigate(['/notfound']);
      return of(null);
    })
  );
};