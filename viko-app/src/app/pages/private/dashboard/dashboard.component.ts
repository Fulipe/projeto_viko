import { Component, inject } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';


interface EventItem {
  title: string;
}

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})
export class DashboardComponent {
  authService = inject(AuthService)

   upcomingEvents: EventItem[] = [
    { title: 'AI Conference 2025' },
    { title: 'Tech Expo Lisbon' },
    { title: 'Cloud Summit' },
  ];

  registeredEvents: EventItem[] = [
    { title: 'Angular Dev Meetup' },
    { title: 'C# Workshop' },
  ];

  finishedEvents: EventItem[] = [
    { title: 'Spring Boot Camp' },
    { title: 'UX Design Sprint' },
  ];
}
