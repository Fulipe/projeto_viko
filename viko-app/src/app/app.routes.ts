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
import { MyeventsComponent } from './pages/private/myevents/myevents.component';
import { MyEventsStudentComponent } from './pages/private/student/my-events-student/my-events-student.component';
import { MyEventsTeacherComponent } from './pages/private/teacher/my-events-teacher/my-events-teacher.component';
import { NewEventComponent } from './pages/private/teacher/new-event/new-event.component';
import { ViewEventComponent } from './pages/private/view-event/view-event.component';
import { ViewEventStudentComponent } from './pages/private/student/view-event-student/view-event-student.component';
import { ViewEventTeacherComponent } from './pages/private/teacher/view-event-teacher/view-event-teacher.component';

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
                path: 'myevents',
                component: MyeventsComponent,
                canActivate: [roleRedirectGuard],
                data: {
                    redirectMap: {
                        Student: '/private/student/myevents',
                        Teacher: '/private/teacher/myevents',
                        // Admin: '/private/admin/dashboard'
                    }
                }
            },

            {
                path: 'dashboard',
                component: DashboardComponent,
                canActivate: [roleRedirectGuard],
                data: {
                    redirectMap: {
                        Student: '/private/student/dashboard',
                        Teacher: '/private/teacher/dashboard',
                        // Admin: '/private/admin/dashboard'
                    }
                }
            },

            {
                path: 'event/newevent',
                component: NewEventComponent,
                canActivate: [roleGuard],
                data: { roles: ['Admin', 'Teacher'] }
            },

            {
                path: 'event/:guid',
                component: ViewEventComponent,
            },

            //Student
            {
                path: 'student/dashboard',
                component: DashboardStudentComponent,
                canActivate: [roleGuard],
                data: { roles: ['Student'] }
            },

            {
                path: 'student/myevents',
                component: MyEventsStudentComponent,
                canActivate: [roleGuard],
                data: { roles: ['Student'] }
            },
            
            //Teacher
            {
                path: 'teacher/dashboard',
                component: DashboardTeacherComponent,
                canActivate: [roleGuard],
                data: { roles: ['Teacher'] }
            },

            {
                path: 'teacher/myevents',
                component: MyEventsTeacherComponent,
                canActivate: [roleGuard],
                data: { roles: ['Teacher'] }
            },

            {
                path: 'teacher/event/:guid',
                component: ViewEventTeacherComponent,
                canActivate: [roleGuard],
                data: { roles: ['Teacher'] }
            },

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
