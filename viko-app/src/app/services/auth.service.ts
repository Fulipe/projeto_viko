import { HttpClient } from '@angular/common/http';
import { inject, Injectable } from '@angular/core';
import { Router } from '@angular/router';
import { environment } from '../../environments/environment';
import { Observable, tap } from 'rxjs';
import { LoginRequest, LoginResponse } from '../interfaces/interfaces';
// import { JwtHelperService } from '@auth0/angular-jwt';

@Injectable({
  providedIn: 'root'
})
export class AuthService {

  private http = inject(HttpClient);
  private router = inject(Router);

  decodeToken(token: string | null): any | null {
    try {
      const payloadBase64 = token!.split('.')[1];
      const payloadDecoded = atob(payloadBase64);
      return JSON.parse(payloadDecoded);

    } catch (e) {
      console.error('Erro ao decodificar o token:', e);
      return null;
    }
  }

  login(credentials: LoginRequest): Observable<LoginResponse> {
    return this.http.post<LoginResponse>(`${environment.apiUrl}/Login`, credentials).pipe(
      tap((res) => {
        if (res.token) {
          localStorage.setItem('auth_token', res.token);

          const decoded = this.decodeToken(res.token);
          if (decoded && decoded.userRole) {
            localStorage.setItem('role', decoded.userRole);
          }

        }
      })
    );
  }

  logout(): void {
    localStorage.clear()
    console.log("dados apagados!")
    this.router.navigate(['/login'])
  }

  getToken(): string | null {
    return localStorage.getItem('auth_token');
  }
  
  getRole(): string | null {
    return localStorage.getItem('role');
  }
  
  getName(): string | null {
    return localStorage.getItem('name');
  }

  isLoggedIn(): boolean {
    return !!this.getToken();
  }
}
