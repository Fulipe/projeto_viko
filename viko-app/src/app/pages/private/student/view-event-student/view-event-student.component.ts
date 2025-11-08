import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ViewEventComponent } from "../../view-event/view-event.component";
import { AuthService } from '../../../../services/auth.service';
import { ActivatedRoute } from '@angular/router';
import { EventService } from '../../../../services/event.service';
import { EventFetched } from '../../../../interfaces/interfaces';
import { ContentObserver } from '@angular/cdk/observers';

@Component({
  selector: 'app-view-event-student',
  standalone: true,
  imports: [
    ViewEventComponent
  ],
  templateUrl: './view-event-student.component.html',
  styleUrl: './view-event-student.component.scss'
})
export class ViewEventStudentComponent implements OnInit {
  eventGuid!: string;

  isHidden = false;
  showRegisterButton = true;
  isRegistered = false;

  constructor(private eventsService: EventService, private route: ActivatedRoute) {
    this.route.paramMap.subscribe(params => {
      this.eventGuid = params.get('guid')!;
    });
  }

  ngOnInit(): void {
    this.checkRegistration()
  }

  checkRegistration() {
    this.eventsService.getStudentEvents().subscribe({
      next: (res) => {
        // console.log(res)
        if (res === false) {
          console.log("Nenhum evento registado.");
          return false;
        }

        const e: any = res;
        const events: EventFetched[] = e

        if (events.some(e => e.guid == this.eventGuid)) {
          this.showRegisterButton = false;
          this.isRegistered = true

          return true

        } else {
          this.showRegisterButton = true;
          this.isRegistered = false

          return false
        
        }
      },
      error: (_) => {
        console.error("Error while loading registered events: ", _)
      }
    })
  }

  registerToEvent() {
    this.eventsService.newRegistration(this.eventGuid).subscribe({
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