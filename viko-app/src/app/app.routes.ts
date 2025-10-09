import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { authGuard } from './guards/auth.guard';
import { LayoutComponent } from './pages/private/layout/layout.component';
import { DashboardComponent } from './pages/private/dashboard/dashboard.component';
import { ProfileComponent } from './pages/private/profile/profile.component';

export const routes: Routes = [
    {
        path: 'private',
        canActivate: [authGuard],
        component: LayoutComponent,
        children:[
            {
                path: 'dashboard',
                component: DashboardComponent,
            },
            {
                path: 'profile',
                component: ProfileComponent,
            }
        ]
    },
    { path: 'login', component: LoginComponent },
    { path: '**', redirectTo: 'private/dashboard', pathMatch: 'full'},
];
