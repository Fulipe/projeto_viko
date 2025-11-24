import { Component, inject, Input, OnInit } from '@angular/core';
import { EventService } from '../../../services/event.service';
import { UserInfo } from '../../../interfaces/interfaces';

@Component({
  selector: 'app-registrations',
  imports: [],
  templateUrl: './registrations.component.html',
  styleUrl: './registrations.component.scss'
})
export class RegistrationsComponent implements OnInit {
  private eventService = inject(EventService)

  @Input() eventGuid!: string;

  loading = true

  registration: UserInfo[] = []

  ngOnInit(): void {
    this.getRegistrations()
  }

  private getRegistrations() {
    this.eventService.getRegistrations(this.eventGuid).subscribe({
      next: (res) => {
        if (res === null) {
          console.log("No registrations found.");
          this.loading = false;
          return;
        }
        
        const e: any = res.registrationsList
        this.registration = e

        this.loading = false;
      }
    })
  }
}
