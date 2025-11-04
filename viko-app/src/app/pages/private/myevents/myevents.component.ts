import { Component, inject, Input, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { EventFetched } from '../../../interfaces/interfaces';
import { EventService } from '../../../services/event.service';

@Component({
  selector: 'app-myevents',
  imports: [],
  templateUrl: './myevents.component.html',
  styleUrl: './myevents.component.scss'
})

export class MyeventsComponent {
  authService = inject(AuthService)
  eventsService = inject(EventService)

  loading = true;

  @Input() upcomingEvents: EventFetched[] = [];
  @Input() finishedEvents: EventFetched[] = [];

}
