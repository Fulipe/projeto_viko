import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Observable } from 'rxjs';
import { UserInfo } from '../interfaces/interfaces';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor() { }

  private http= inject(HttpClient);

  userInfo():Observable<UserInfo>{
    return this.http.get<UserInfo>(`${environment.apiUrl}/Profile`)
  }
}
