import { Component, inject } from '@angular/core';
import { EventItem, EventFetched } from '../../../../interfaces/interfaces';
import { AuthService } from '../../../../services/auth.service';
import { EventService } from '../../../../services/event.service';

@Component({
  selector: 'app-dashboard-teacher',
  imports: [],
  templateUrl: './dashboard-teacher.component.html',
  styleUrl: './dashboard-teacher.component.scss'
})
export class DashboardTeacherComponent {
 authService = inject(AuthService)
  eventsService = inject(EventService)

  loading = true;

  upcomingEvents: EventItem[] = [];
  registeredEvents: EventItem[] = [];
  finishedEvents: EventItem[] = [];
  filteredEvents: any[] = []

  constructor() { }

  ngOnInit(): void {
    this.loadUserEvents()
    this.loadUpcomingEvents()
    this.loadFinishedEvents()
  }

  private loadUserEvents() { // => registeredEvents | Only events where the users are registered
    this.eventsService.getTeacherEvents().subscribe({
      next: (res) => {
        // console.log(res)
        if (res === false) {
          console.log("Nenhum evento registado.");
          this.loading = false;
          return;
        }

        const e: any = res;
        const events: EventFetched[] = e

        //Filters finished events
        this.registeredEvents = events.filter(event => event.eventStatus != 3);

        this.loading = false;
      },
      error: (_) => {
        console.error("Error while loading registered events: ", _)
        this.loading = false;
      }
    })
  }

  private loadUpcomingEvents() { // => upcomingEvents | All public not closed or finished events
    this.eventsService.getAllEvents().subscribe({
      next: (res) => {
        if (res === false) {
          console.log("Nenhum evento registado.");
          this.loading = false;
          return;
        }

        const e: any = res;
        const events: EventFetched[] = e
        
        //Filters finished events out
        this.filteredEvents =
          events
            .filter(event => event.eventStatus != 3)
            .filter(event => event.eventStatus != 2);

        console.log(this.filteredEvents)
        this.upcomingEvents = this.filteredEvents.slice(0, 10)

        this.loading = false;

      },
      error: (_) => {
        console.error("Error while loading upcoming events: ", _)
        this.loading = false;
      }
    })
  }

  private loadFinishedEvents() { // => finishedEvents | Only finished events
    this.eventsService.getAllEvents().subscribe({
      next: (res) => {
        if (res === false) {
          console.log("Nenhum evento registado.");
          this.loading = false;
          return;
        }

        const e: any = res;
        const events: EventFetched[] = e

        //Filters opened and closed events out
        this.finishedEvents =
          events
            .filter(event => event.eventStatus != 1)
            .filter(event => event.eventStatus != 2);

        this.loading = false;

      },
      error: (_) => {
        console.error("Error while loading finished events: ", _)
        this.loading = false;
      }
    })
  }
}
