import { Component, inject, OnInit } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';
import { EventService } from '../../../services/event.service';
import { EventFetched } from '../../../interfaces/interfaces';
import { AuthService } from '../../../services/auth.service';

@Component({
  selector: 'app-event-search',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './search-event.component.html'
})
export class SearchEventComponent implements OnInit {
  private eventsService = inject(EventService)
  private authService = inject(AuthService)


  events: EventFetched[] = [];
  filteredEvents: EventFetched[] = [];
  displayedEvents: EventFetched[] = [];

  loading = true

  role = this.authService.getRole()

  searchTerm = '';
  categoryFilter = '';
  cityFilter = '';
  locationFilter = '';

  currentPage = 1;
  itemsPerPage = 10;
  totalPages = 1;
  sortColumn: keyof EventFetched | "" = "";
  sortAsc = true;

  constructor() { }

  ngOnInit() {
    this.eventsService.getAllEvents().subscribe({
      next: (res) => {
        if (res === false) {
          console.log("Nenhum evento registado.");
          this.loading = false;
          return;
        }

        const e: any = res;
        this.events = e

        if(this.role == "Student"){  
          this.events = this.events
              .filter(event => event.eventStatus != 3)
              .filter(event => event.eventStatus != 2);
        }          

        this.applyFilters()

        this.loading = false;

      },
    })
  }

  //Function to apply filters
  applyFilters() {
    this.loading = true

    const term = this.searchTerm.toLowerCase();

    //Aplies base filters (Category, City, Location)
    const filteredBase = this.events.filter(event =>
      (this.categoryFilter ? event.category === this.categoryFilter : true) &&
      (this.cityFilter ? event.city === this.cityFilter : true) &&
      (this.locationFilter ? event.location === this.locationFilter : true)
    );

    // Tries to filter events that start with the term 
    let filtered = filteredBase.filter(event =>
      !term || event.title.toLowerCase().startsWith(term)
    );

    // If no event was found starting with the term, try to find the term in some event title 
    if (filtered.length === 0 && term) {
      filtered = filteredBase.filter(event =>
        event.title.toLowerCase().includes(term)
      );
    }

    // Stores final result and aplies sorting
    this.filteredEvents = filtered;

    this.loading = false
    this.sortEvents();
  }

  // Sort Events based on its collumn and selected direction
  sortEvents() {
    if (this.sortColumn) {
      this.filteredEvents.sort((a, b) => {
        const column = this.sortColumn as keyof EventFetched

        const valA = a[column];
        const valB = b[column];

        // Alphabetic comparison 
        return this.sortAsc
          ? valA.toString().localeCompare(valB.toString())
          : valB.toString().localeCompare(valA.toString());
      });
    }

    this.updateDisplayedEvents();
  }

  // Updates visible event list based on the current page
  updateDisplayedEvents() {
    this.totalPages = Math.ceil(this.filteredEvents.length / this.itemsPerPage);

    //Calculates item interval between pages
    const start = (this.currentPage - 1) * this.itemsPerPage;
    const end = start + this.itemsPerPage;

    //Slices array with filtered event to show 10 events per page
    this.displayedEvents = this.filteredEvents.slice(start, end);
  }

  // Function to change in between pages
  changePage(page: number) {

    // Certifies that the page will be valid
    if (page >= 1 && page <= this.totalPages) {
      this.currentPage = page;
      this.updateDisplayedEvents();
    }
  }

  // Changes sorting by clicking in table header
  sortBy(column: keyof EventFetched) {

    // If its already sorted by selected collumn, invertes direction
    if (this.sortColumn === column) {
      this.sortAsc = !this.sortAsc;
    } else {
      // If its a new collumn being sorted defines as Asc.
      this.sortColumn = column;
      this.sortAsc = true;
    }

    // Applies sorting
    this.sortEvents();
  }
}
