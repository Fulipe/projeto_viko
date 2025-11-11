import { Component, inject } from '@angular/core';
import { MyeventsComponent } from "../../myevents/myevents.component";
import { EventFetched } from '../../../../interfaces/interfaces';
import { EventService } from '../../../../services/event.service';

@Component({
  selector: 'app-my-events-student',
  imports: [MyeventsComponent],
  templateUrl: './my-events-student.component.html',
  styleUrl: './my-events-student.component.scss'
})
export class MyEventsStudentComponent {
  private eventsService = inject(EventService)

  upcomingEvents: EventFetched[] = [];
  finishedEvents: EventFetched[] = [];

  loading = true

  ngOnInit(): void {
    this.loadEvents()
  }

  private loadEvents() {
    this.eventsService.getStudentEvents().subscribe({
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
        this.upcomingEvents = events.filter(event => event.eventStatus != 3);

        //Filters not finished events
        this.finishedEvents = events
          .filter(event => event.eventStatus != 1)
          .filter(event => event.eventStatus != 2);

        this.loading = false;
      },
      error: (_) => {
        console.error("Error while loading registered events: ", _)
        this.loading = false;
      }
    })
  }

}
