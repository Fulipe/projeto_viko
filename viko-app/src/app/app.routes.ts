import { Routes } from '@angular/router';
import { LoginComponent } from './pages/login/login.component';
import { authGuard } from './guards/auth.guard';
import { LayoutComponent } from './pages/private/layout/layout.component';
import { DashboardComponent } from './pages/private/dashboard/dashboard.component';
import { ProfileComponent } from './pages/private/profile/profile.component';
import { SignupComponent } from './pages/signup/signup.component';
import { UnauthorizedComponent } from './pages/unauthorized/unauthorized.component';
import { DashboardTeacherComponent } from './pages/private/teacher/dashboard-teacher/dashboard-teacher.component';
import { roleGuard } from './guards/role.guard';

export const routes: Routes = [
    {
        path: 'private',
        canActivate: [authGuard],
        component: LayoutComponent,
        children: [
            //General use pages
            {
                path: 'profile',
                component: ProfileComponent,
            },
            { 
                //Public events
                path: 'dashboard',
                component: DashboardComponent,
            },

            //Student

            //Teacher
            {
                path: 'teacher-dashboard',
                component: DashboardTeacherComponent,
                canActivate: [roleGuard],
                data: {roles: ['Teacher']}
            }

            //Admin
            /**
             * path: 'dashboardAdmin'
             * component: DashboardAdminComponent
             * canActivate: [RoleGuard]
             * data: Roles.Admin
             * children: [
             *      {
             *          path: children1
             *          component: childComponent
             *          canActivate: [RoleGuard]
             *          data: Roles.Admin
             *      }
             * ]
             */
        ]
    },
    { path: 'login', component: LoginComponent },
    { path: 'signup', component: SignupComponent },
    { path: 'unauthorized', component: UnauthorizedComponent},

    { path: '**', redirectTo: 'private/dashboard', pathMatch: 'full' },
];
