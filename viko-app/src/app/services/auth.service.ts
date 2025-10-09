import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { Observable, tap } from 'rxjs';
import { LoginRequest, LoginResponse } from '../interfaces/interfaces';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);
  private router = inject(Router);

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/Login`, credentials).pipe(
      tap((res) => {
        localStorage.setItem('auth_token', res.token);

      })
    );
  }
  
  logout(): void {
    localStorage.removeItem('auth_token');
    console.log("dados apagados!")
    this.router.navigate(['/login'])
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }

  isLoggedIn(): boolean {
    return !!this.getToken(); 
  }
}
