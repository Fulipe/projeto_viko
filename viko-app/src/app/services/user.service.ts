import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable, tap } from 'rxjs';
import { UserInfo } from '../interfaces/interfaces';
import { environment } from '../../environments/environment';
import { AuthService } from './auth.service';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor() { }

  private http= inject(HttpClient);
  private authService = inject(AuthService);
  
  userInfo():Observable<UserInfo>{
    return this.http.get<UserInfo>(`${environment.apiUrl}/GetUser`)
  }

  userUpdate(user: UserInfo):Observable<UserInfo>{
    const headers = new HttpHeaders({
      Authorization: `Bearer ${this.authService.getToken()}`
    }) 
    return this.http.post<any>(`${environment.apiUrl}/UpdateUser`, user)
  }
}
