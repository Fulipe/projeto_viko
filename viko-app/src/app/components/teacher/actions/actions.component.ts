import { Component, EventEmitter, inject, Input, OnChanges, OnInit, Output, SimpleChanges } from '@angular/core';
import { EventFetched } from '../../../interfaces/interfaces';
import { EventService } from '../../../services/event.service';
import { Router } from '@angular/router';

@Component({
  selector: 'app-actions',
  standalone: true,
  imports: [],
  templateUrl: './actions.component.html',
  styleUrl: './actions.component.scss'
})
export class ActionsComponent implements OnChanges{
  @Input() event!: EventFetched;
  @Input() eventGuid!: string;

  // Edit
  @Output() editAside = new EventEmitter<boolean>();
  @Output() updated = new EventEmitter<void>();

  @Output() deletePopUp = new EventEmitter<boolean>();

  @Input() eventStatus!: number;

  private eventService = inject(EventService)

  ngOnChanges(changes: SimpleChanges) {
    const evt = changes['event'];
    // const guid = changes['eventGuid'];

    // se foi a primeira mudança (carregamento inicial), ignora
    if (evt?.firstChange) return;

    this.updateBackend(
      this.eventGuid,
      evt?.currentValue
    );
  }

  onEditClick() {
    this.editAside.emit(true)
  }

  updateBackend(valueGuid: any, valueEvt: any) {
    console.log(valueGuid)
    console.log(valueEvt)

    //Endpoint to backend

    this.eventService.editEvent(valueGuid, valueEvt).subscribe({
      next: () => {
        console.log('Evento guardado com sucesso!');
        this.updated.emit(); 
      },
      error: (err) => {
        console.error('Erro ao guardar evento:', err);
      }
    });
  }

  onDeleteClick(){
    this.deletePopUp.emit(true)
  }
}
