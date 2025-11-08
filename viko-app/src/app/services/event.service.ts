import { inject, Injectable } from '@angular/core';
import { Observable, map, of, catchError } from 'rxjs';
import { environment } from '../../environments/environment';
import { CreateEventDto, EventFetched } from '../interfaces/interfaces';
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

  createEvent(dto: CreateEventDto): Observable<CreateEventDto>{
    return this.http.post<CreateEventDto>(`${environment.apiUrl}/CreateEvent`, dto);
  }

  GetEvent(guid: string | null): Observable<EventFetched | false>{
    return this.http.get<any>(`${environment.apiUrl}/GetEvent?guid=${guid}`).pipe(
      map((res) => {
        if (res?.eventFetched) {
          return res.eventFetched
        }

        return false
      })
    )
  }

  newRegistration(guid: string | null): Observable<any>{
    return this.http.post<any>(`${environment.apiUrl}/EventRegistration`, guid);
  }
}
