import { Component, inject, input, Input, OnInit, ViewChild } from '@angular/core';
import { ActivatedRoute, ɵEmptyOutletComponent } from '@angular/router';
import { EventService } from '../../../services/event.service';
import { EventFetched } from '../../../interfaces/interfaces';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';

// type role = "Student" | "Teacher" | "Admin"

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
  private authService = inject(AuthService)

  eventGuid!: string;
  event: EventFetched;
  
  roleSaved: string | null = this.authService.getRole()
  
  //Registration 
  eventStatus: number; 
  isHidden = false;
  showRegisterButton = true;
  isRegistered = false;

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
    this.eventStatus = 0
  }

  ngOnInit(): void {
    this.loadEvent()
    this.checkRegistration()
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
          this.eventStatus = event.eventStatus

        },
    })
  }

  //Checks registration, by checking if GUID of opened event, is in the Students events list
  private checkRegistration() {
    this.eventService.getStudentEvents().subscribe({
      next: (res) => {
        // console.log(res)
        if (res === false) {
          console.log("Nenhum evento registado.");
          // return false;
        }

        const e: any = res;
        const events: EventFetched[] = e

        if (events.some(e => e.guid == this.eventGuid)) {
          this.showRegisterButton = false;
          this.isRegistered = true

          console.log('AA')
          // return true

        } else {
          this.showRegisterButton = true;
          this.isRegistered = false

          console.log('BB')

          // return false

        }
      },
      error: (_) => {
        console.error("Error while loading registered events: ", _)
      }
    })
  }

  //Function to register Student in the event
  registerToEvent() {
    this.eventService.newRegistration(this.eventGuid).subscribe({
      next: (res) => {
        console.log('Registered!');

        this.checkRegistration()

        console.log(this.showRegisterButton)
        console.log(this.isRegistered)
      },
      error: (err) => {
        console.log("Couldn't register in this event")
      }
    })
  }
}
