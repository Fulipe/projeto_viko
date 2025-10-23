import { inject, Injectable } from '@angular/core';
import { Observable, map, of, catchError } from 'rxjs';
import { environment } from '../../environments/environment';
import { EventFetched } from '../interfaces/interfaces';
import { HttpClient } from '@angular/common/http';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  constructor() { }

  private http = inject(HttpClient);

  getUserEvents(): Observable<EventFetched | false> {
    return this.http.get<any>(`${environment.apiUrl}/GetUserEvents`).pipe(
      map((res) => {
        // console.log(res)
        if (res?.eventFetched) {
          return res.eventFetched;
        }

        return false;
      })
    )
  }
}
