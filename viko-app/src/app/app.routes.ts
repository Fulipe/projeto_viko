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
import { HomeComponent } from './pages/private/home/home.component';
import { DashboardStudentComponent } from './pages/private/student/dashboard-student/dashboard-student.component';
import { roleRedirectGuard } from './guards/role-redirect.guard';

export const routes: Routes = [
    { path: 'login', component: LoginComponent },
    { path: 'signup', component: SignupComponent },
    { path: 'unauthorized', component: UnauthorizedComponent },
    {
        path: 'private',
        canActivate: [authGuard],
        component: LayoutComponent,
        children: [
            //General use pages
            {
                path: 'home',
                component: HomeComponent
            },

            {
                path: 'profile',
                component: ProfileComponent,
            },

            {
                path: 'dashboard',
                component: DashboardComponent, 
                canActivate: [roleRedirectGuard],
            },
            //Student
            {
                path: 'dashboard/student',
                component: DashboardStudentComponent,
                canActivate: [roleGuard],
                data: { roles: ['Student'] }
            },

            //Teacher
            {
                path: 'dashboard/teacher',
                component: DashboardTeacherComponent,
                canActivate: [roleGuard],
                data: { roles: ['Teacher'] }
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
    { path: '**', redirectTo: 'private/home', pathMatch: 'full' },
];
