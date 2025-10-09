import { HttpErrorResponse, HttpInterceptorFn } from '@angular/common/http';
import { inject } from '@angular/core';
import { AuthService } from '../services/auth.service';
import { catchError, throwError } from 'rxjs';
import { Router } from '@angular/router';

export const authInterceptor: HttpInterceptorFn = (req, next) => {
    const router = inject(Router)
    const authService = inject(AuthService)

    if (authService.isLoggedIn()) {
        const cloned = req.clone({
            setHeaders: { Authorization: `Bearer ${authService.getToken()}` },
        });
        return next(cloned).pipe(
            catchError((error: HttpErrorResponse) => {
                if (error.status === 401) {
                    // Unauthorized
                    // e.g. force logout and redirect to login
                    authService.logout();
                } else if (error.status === 403) {
                    // Forbidden
                    // e.g. navigate to a "no access" page or just log
                    console.error('Access denied – 403');
                    router.navigate(['/login'])
                    // router.navigate(['/forbidden']);
                }
                // Propagate the error so the component or other interceptors can handle it
                return throwError(() => error);
            })
        );
    }

    return next(req);
};
