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

  getStudentEvents(): Observable<EventFetched | false> {
    return this.http.get<any>(`${environment.apiUrl}/GetStudentEvents`).pipe(
      map((res) => {
        // console.log(res)
        if (res?.eventsFetched) {
          return res.eventsFetched;
        }

        return false;
      })
    )
  }

  getTeacherEvents():Observable<EventFetched | false>{
    return this.http.get<any>(`${environment.apiUrl}/GetTeacherEvents`).pipe(
      map((res) => {
        // console.log(res)
        if (res?.eventsFetched) {
          return res.eventsFetched;
        }

        return false;
      })
    )
  }

  getAllEvents(): Observable<EventFetched | false> {
    return this.http.get<any>(`${environment.apiUrl}/GetAllEvents`).pipe(
      map((res) => {
        // console.log(res)
        if (res?.eventsFetched) {
          return res.eventsFetched;
        }

        return false;
      })
    )
  }
}
