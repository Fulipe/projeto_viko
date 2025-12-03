import { HttpClient, HttpHeaders } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { map, Observable, of, tap } from 'rxjs';
import { PasswordPayload, UserInfo } from '../interfaces/interfaces';
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
    return this.http.get<any>(`${environment.apiUrl}/GetUserProfile`)
    .pipe(
      map((response) => {
        if(response?.userLogged){
          return response.userLogged;
        }

        return of<false>(false);

      })
    )
  }

  changePassword(dto: PasswordPayload){
    return this.http.post<any>(`${environment.apiUrl}/ChangePassword`, dto)
  }

  userUpdate(user: UserInfo):Observable<UserInfo>{
    return this.http.post<any>(`${environment.apiUrl}/UpdateUser`, user)
  }
  getAllTeachers():Observable<any>{
    return this.http.get<any>(`${environment.apiUrl}/GetTeachers`)
    .pipe(
      map((res)=>{
        if(res?.teachers){
          return res.teachers
        }

        return false;
      })
    )
  }
  getAllUsers():Observable<any>{
    return this.http.get<any>(`${environment.apiUrl}/GetAllUsers`)
    .pipe(
      map((res) => {
        if(res?.users){
          return res.users
        }

        return false;
      })
    )
  }
  updateUserRole(username: string, role: string): Observable<any>{
    return this.http.post<any>(`${environment.apiUrl}/UpdateUserRole`, {username, role})
  }
  viewUser(guid: string | null):Observable<UserInfo | null>{
    return this.http.get<any>(`${environment.apiUrl}/GetUserViewUser?guid=${guid}`)
    .pipe(
      map((res) => {
        if (res?.userFetched){
          return res.userFetched
        }

        return null
      })
    )
  }
}
