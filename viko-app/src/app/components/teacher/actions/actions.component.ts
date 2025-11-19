import { Component, EventEmitter, inject, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { EventFetched, EventStatus } from '../../../interfaces/interfaces';
import { EventService } from '../../../services/event.service';
import { Router } from '@angular/router';
import { FormBuilder, FormGroup, FormsModule, ReactiveFormsModule } from '@angular/forms';

@Component({
  selector: 'app-actions',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './actions.component.html',
  styleUrl: './actions.component.scss'
})
export class ActionsComponent implements OnChanges, OnInit {
  private eventService = inject(EventService)
  private fb = inject(FormBuilder)

  @Input() event!: EventFetched;
  @Input() eventGuid!: string;

  // Edit
  @Output() editAside = new EventEmitter<boolean>();
  @Output() updated = new EventEmitter<void>();

  // Delete
  @Output() deletePopUp = new EventEmitter<boolean>();
  @Input() eventIsViewed!: boolean;

  // Change Status
  EventStatus: EventStatus[] = [
    { id: 1, status: "Open" },
    { id: 2, status: "Closed" },
    { id: 3, status: "Finished" }
  ]
  statusForm!: FormGroup;
  @Input() eventStatus!: number;
  originalStatus!: number;          // to compare
  statusChanged = false;

  ngOnInit(): void {
    this.originalStatus = this.eventStatus;

    this.statusForm = this.fb.group({
      status: [this.eventStatus]
    });    

  }

  ngOnChanges(changes: SimpleChanges) {
    const evt = changes['event'];
    // const guid = changes['eventGuid'];

    // if it was the first change (init), ignores
    if (evt?.firstChange) return;

    // this.updateBackend(
    //   this.eventGuid,
    //   evt?.currentValue
    // );
  }

  //#region Edit
  onEditClick() {
    this.editAside.emit(true)
  }

  // updateBackend(valueGuid: any, valueEvt: any) {
  //   console.log(valueGuid)
  //   console.log(valueEvt)

  //   //Endpoint to backend

  //   this.eventService.editEvent(valueGuid, valueEvt).subscribe({
  //     next: () => {
  //       console.log('Evento guardado com sucesso!');
  //       this.updated.emit();
  //     },
  //     error: (err) => {
  //       console.error('Erro ao guardar evento:', err);
  //     }
  //   });
  // }
  //#endregion

  //#region Delete
  onDeleteClick() {
    this.deletePopUp.emit(true)
  }
  //#endregion

  //#region Status Change

  // Checks if selected status, is equal to the original
  onStatusChange(): boolean {
    return this.statusForm.get('status')!.value !== this.originalStatus;
  }

  // Sends status update
  saveStatus() {
    const newStatus = this.statusForm.get('status')!.value;

    this.eventService.updateStatus(this.eventGuid, newStatus).subscribe({
      next: () => {
        console.log("Status atualizado com sucesso!");
        this.originalStatus = newStatus;  // newStatus turns to be the "original"
        this.statusChanged = false;

        console.log(this.originalStatus)
        console.log(this.statusChanged)

        this.updated.emit()
      },
      error: (err) => console.error(err)
    });
  }

  //#endregion
}
