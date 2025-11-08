import { Component, inject, input, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, ɵEmptyOutletComponent } from '@angular/router';
import { EventService } from '../../../services/event.service';
import { EventFetched } from '../../../interfaces/interfaces';
import { CommonModule } from '@angular/common';

@Component({
  selector: 'app-view-event',
  standalone: true,
  imports: [
    CommonModule,
  ],
  templateUrl: './view-event.component.html',
  styleUrl: './view-event.component.scss'
})
export class ViewEventComponent implements OnInit {
  private eventService = inject(EventService)
  
  eventGuid!: string;
  
  event: EventFetched;

  constructor(private route: ActivatedRoute) {
    this.eventGuid = String(this.route.snapshot.paramMap.get('guid'))

    this.event = {
      title: '',
      description: '',
      category: '',
      teacher: '',
      language: '',
      city: '',
      image: '',
      location: '',
      startDate: '',
      endDate: '',
      registrationDeadline: '',
      guid: '',
      eventStatus: 0,
    }
  }

  ngOnInit(): void {
    this.loadEvent()

  }

  loadEvent() {
    this.eventService.GetEvent(this.eventGuid).subscribe({
      next:
        (res) => {
          if (res == false) {
            console.log("Event not found")
            return;
          }

          const e: any = res
          const event: EventFetched = e

          this.event = event
        },
    })
  }
}
