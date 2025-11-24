import { CommonModule, DatePipe } from '@angular/common';
import { Component, inject, OnInit } from '@angular/core';
import { FormsModule } from '@angular/forms';
import { ActivatedRoute, Router } from '@angular/router';
import { CategoriesObjectFormat, EventFetched, UserInfo } from '../../../interfaces/interfaces';
import { EventService } from '../../../services/event.service';
import { CATEGORIES } from '../../../interfaces/categories';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-view-user',
  imports: [
    DatePipe,
    FormsModule,
    CommonModule
  ],
  templateUrl: './view-user.component.html',
  styleUrl: './view-user.component.scss'
})
export class ViewUserComponent implements OnInit {
  private eventService = inject(EventService)
  
  userGuid!: string;
  user!: UserInfo;
  userRole!: string;

  eventGuid!: string;
  userEvents: EventFetched[] = [];

  loading = true


  // Sorting states
  sortColumnEvent: keyof EventFetched | "" = "";
  sortAscEvents: boolean = true;
  
  constructor(private route: ActivatedRoute, private router: Router) {
    this.userGuid = String(this.route.snapshot.paramMap.get('guid'))
    this.user = this.route.snapshot.data['user'];

    this.userRole = this.user.role

  }

  ngOnInit(): void {
    if (this.userRole != "Admin") {
      this.getEventsList()
    }
    this.loading = false
  }

  compare(a: any, b: any, column: string, asc: boolean) {
    let valA = (a[column] ?? "").toString().toLowerCase();
    let valB = (b[column] ?? "").toString().toLowerCase();

    return asc
      ? valA.localeCompare(valB)
      : valB.localeCompare(valA);
  }
  
  sortUserEvent(column: keyof EventFetched) {
    if (this.sortColumnEvent === column) {
      this.sortAscEvents = !this.sortAscEvents;
    } else {
      this.sortColumnEvent = column;
      this.sortAscEvents = true;
    }

    this.userEvents.sort((a, b) =>
      this.compare(a, b, column, this.sortAscEvents)
    );
  }

  getEventsList() {
    this.eventService.getEventOfUser(this.userGuid).subscribe(
      (res) => {
        if (res == null) {
          console.log("No Events were found")
          this.loading = false;
          return;
        }

        const e: any = res;
        const events: EventFetched[] = e

        this.userEvents = events;
        this.loading = false
      }
    )
  }


}
