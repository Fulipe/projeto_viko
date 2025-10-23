import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable, of, tap } from 'rxjs';
import { UserInfo } from '../interfaces/interfaces';
import { environment } from '../../environments/environment';

@Injectable({
  providedIn: 'root'
})
export class UserService {

  constructor() { }

  private http= inject(HttpClient);

  //Signup
  createUser(dto: any):Observable<any> {
    return this.http.post<any>(`${environment.apiUrl}/Signup`, dto)
  }
  
  //Profile
  userInfo():Observable<UserInfo | false>{
    return this.http.get<any>(`${environment.apiUrl}/GetUser`).pipe(
      map((response) => {
        if(response?.userLogged){
          return response.userLogged;
        }

        return of<false>(false);

      })
    )
  }

  userUpdate(user: UserInfo):Observable<UserInfo>{
    return this.http.post<any>(`${environment.apiUrl}/UpdateUser`, user)
  }
}
