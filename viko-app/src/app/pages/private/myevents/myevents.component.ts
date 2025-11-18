import { Component, inject, input, Input, OnInit } from '@angular/core';
import { AuthService } from '../../../services/auth.service';
import { EventFetched } from '../../../interfaces/interfaces';
import { EventService } from '../../../services/event.service';
import { Router, RouterLink, RouterLinkActive } from "@angular/router";

@Component({
  selector: 'app-myevents',
  standalone: true,
  imports: [],
  templateUrl: './myevents.component.html',
  styleUrl: './myevents.component.scss'
})

export class MyeventsComponent {
  authService = inject(AuthService)
  eventsService = inject(EventService)
  router = inject(Router)
  
  deletePopup: boolean = false;
  eventGuid!: string

  loading = true;

  @Input() upcomingEvents: EventFetched[] = [];
  @Input() finishedEvents: EventFetched[] = [];
  @Input() isTeacher!: boolean;

  onDeleteClick(guid: string){
    this.deletePopup = true
    this.eventGuid = guid

  }

  onCancelPopup(){
    this.deletePopup = false
  }

  confirmDelete(){
    this.eventsService.deleteEvent(this.eventGuid).subscribe({
      next: (res)=>{
        window.location.reload()
        // this.router.navigate(['/private/myevents'])
        this.deletePopup = false

      },
      error: (_)=>{
        console.log("Event wasn't deleted! ")
      }
    })
  }
}
