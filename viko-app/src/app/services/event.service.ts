import { inject, Injectable } from '@angular/core';
import { Observable, map, of, catchError, tap } from 'rxjs';
import { environment } from '../../environments/environment';
import { TeacherCreateEventDto, EventFetched, AdminCreateEventDto } from '../interfaces/interfaces';
import { HttpClient } from '@angular/common/http';
import { Router } from '@angular/router';

@Injectable({
  providedIn: 'root'
})
export class EventService {

  constructor() { }

  private http = inject(HttpClient);
  private router = inject(Router)

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

  teacherCreateEvent(dto: TeacherCreateEventDto): Observable<TeacherCreateEventDto>{
    return this.http.post<TeacherCreateEventDto>(`${environment.apiUrl}/CreateEventTeacher`, dto);
  }
  adminCreateEvent(dto: AdminCreateEventDto): Observable<AdminCreateEventDto>{
    return this.http.post<AdminCreateEventDto>(`${environment.apiUrl}/CreateEventAdmin`, dto)
  }

  GetEvent(guid: string | null): Observable<EventFetched | null>{
    return this.http.get<any>(`${environment.apiUrl}/GetEvent?guid=${guid}`).pipe(
      map((res) => {
        if (res?.eventFetched) {
          return res.eventFetched
        }

        return null
      }), 
    )
  }

  newRegistration(guid: string | null): Observable<any>{
    return this.http.post<any>(`${environment.apiUrl}/EventRegistration`, guid);
  }

  cancelRegistration(guid: string | null): Observable<any>{
    return this.http.post<any>(`${environment.apiUrl}/CancelEventRegistration`, guid);
  }

  getRegistrations(guid: string | null):Observable<any>{
    return this.http.get<any>(`${environment.apiUrl}/GetRegistrations?guid=${guid}`).pipe(
      map((res) => {
        if (res?.registrationsList){
          return res.registrationsList
        }

        return null
      })
    )
  }

  editEvent (guid: string | null, eventUpdate: EventFetched): Observable<any>{
    return this.http.post<any>(`${environment.apiUrl}/EditEvent`, {guid, eventUpdate})
  }

  deleteEvent (guid: string | null): Observable<any>{
    return this.http.delete<any>(`${environment.apiUrl}/DeleteEvent?guid=${guid}`)
  }

  republishEvent (guid: string | null): Observable<any>{
    return this.http.post<any>(`${environment.apiUrl}/RepublishEvent?guid=${guid}`, {})
  }

  eraseEvent (guid: string | null): Observable<any>{
    return this.http.delete<any>(`${environment.apiUrl}/EraseEvent?guid=${guid}`)
  }
  updateStatus (guid: string | null, eventStatus: number): Observable<any>{
    return this.http.post<any>(`${environment.apiUrl}/UpdateEventStatus`, {guid, eventStatus})
  }
}
