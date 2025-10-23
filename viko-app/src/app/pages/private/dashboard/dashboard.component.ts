import { Component, inject, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { CommonModule } from '@angular/common';
import { EventService } from '../../../services/event.service';
import { EventFetched, EventItem } from '../../../interfaces/interfaces'

@Component({
  selector: 'app-dashboard',
  imports: [
    CommonModule
  ],
  templateUrl: './dashboard.component.html',
  styleUrl: './dashboard.component.scss'
})

export class DashboardComponent implements OnInit {
  authService = inject(AuthService)
  eventsService = inject(EventService)

  loading = true;

  upcomingEvents: EventItem[] = [];
  registeredEvents: EventItem[] = [];
  finishedEvents: EventItem[] = [];

  constructor() { }

  ngOnInit(): void {
    this.loadUserEvents()
  }

  private loadUserEvents() { // => registeredEvents
    this.eventsService.getUserEvents().subscribe({
      next: (res) => {
        // console.log(res)
        if (res === false) {
          console.log("Nenhum evento registado.");
          this.loading = false;
          return;
        }

        const e: any = res;
        const events: EventFetched[] = e
        
        this.registeredEvents = events.map(event => ({ title: event.title }));
        // console.log(this.registeredEvents)

        this.loading = false;
      },
      error: (_) => {
        console.error("Error while loading registered events: ", _)
        this.loading = false;
      }
    })
  }

}
