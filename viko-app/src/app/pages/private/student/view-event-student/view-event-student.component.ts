import { Component, EventEmitter, Input, OnInit, Output } from '@angular/core';
import { ViewEventComponent } from "../../view-event/view-event.component";
import { AuthService } from '../../../../services/auth.service';

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
 
  showRegisterButton = true;
  isRegistered = false;

  constructor(private authService: AuthService) { }

  ngOnInit(): void {
  
  }
  
  registerToEvent() {
    console.log('Registered!');
  }
}