import { Component, ElementRef, EventEmitter, inject, input, Input, OnInit, Output, ViewChild } from '@angular/core';
import { ActivatedRoute, Route, Router, ɵEmptyOutletComponent } from '@angular/router';
import { EventService } from '../../../services/event.service';
import { CategoriesObjectFormat, EventFetched, LanguagesObjectFormat } from '../../../interfaces/interfaces';
import { CommonModule } from '@angular/common';
import { AuthService } from '../../../services/auth.service';
import { CATEGORIES } from '../../../interfaces/categories';
import { RegistrationsComponent } from "../../../components/teacher/registrations/registrations.component";
import { ActionsComponent } from "../../../components/teacher/actions/actions.component";
import { FormControl, FormGroup, FormsModule, Validators, ReactiveFormsModule } from '@angular/forms';
import { LANGUAGES } from '../../../interfaces/languages';

@Component({
  selector: 'app-view-event',
  standalone: true,
  imports: [
    CommonModule,
    RegistrationsComponent,
    ActionsComponent,
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './view-event.component.html',
  styleUrl: './view-event.component.scss'
})
export class ViewEventComponent implements OnInit {
  private eventService = inject(EventService)
  private authService = inject(AuthService)

  eventGuid!: string;
  event!: EventFetched;

  roleSaved: string | null = this.authService.getRole()

  //Edit
  editAside: boolean = false;
  eventEdit!: FormGroup; // Clone for event editing
  eventEdited: any = {};
  originalEvent!: any;

  selectedFile?: File;
  previewUrl: string | ArrayBuffer | null = null;

  //Language
  languages = [...LANGUAGES];
  selectedLanguages: any[] = [];
  @ViewChild('inputEl') inputEl!: ElementRef<HTMLInputElement>;
  searching = false
  originalLanguages!: any[];

  //Category
  categories = [...CATEGORIES]
  category?: CategoriesObjectFormat;

  //Delete
  deletePopup: boolean = false
  toDelete: boolean = false

  //Registration 
  eventStatus: number;
  isHidden = false;
  showRegisterButton = true;
  isRegistered = false;

  constructor(private route: ActivatedRoute, private router: Router) {
    this.eventGuid = String(this.route.snapshot.paramMap.get('guid'))
    this.event = this.route.snapshot.data['event'];

    this.eventStatus = this.event.eventStatus;

    //Sets the category of the event as an Category object
    this.category = this.categories.find(c => c.name == this.event.category)

  }

  ngOnInit(): void {
    //Checks if role from logged user is Student, so it can check registration
    if (this.roleSaved == "Student") {
      this.checkRegistration()
    }

    if (this.roleSaved == "Teacher" || this.roleSaved == "Admin") {
      this.initEditForm()
      this.originalEvent = this.cleanFormValue(this.eventEdit.getRawValue());
    }
  }
  //#region Edit - Image

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    this.selectedFile = input.files[0];

    // Show Image preview
    const reader = new FileReader();
    reader.onload = () => {
      const base64 = reader.result as string
      this.previewUrl = base64
      this.eventEdit.patchValue({ image: this.previewUrl });

    };
    reader.readAsDataURL(this.selectedFile);
  }

  //#endregion

  //#region Edit - Languages

  onSearch() {
    this.searching = true

    const query = this.eventEdit.get('language')?.value || '';

    if (query.trim() === '') {
      this.languages = LANGUAGES.filter(
        l => !this.selectedLanguages.includes(l.name)
      );
    } else {
      this.languages = LANGUAGES.filter(
        l => l.name.toLowerCase().startsWith(query) &&
          !this.selectedLanguages.includes(l.name));
    }
  }

  selectLanguage(lang: LanguagesObjectFormat) {
    this.searching = false

    this.selectedLanguages.push(lang.name)

    this.eventEdit.markAsDirty();
    this.inputEl.nativeElement.value = ''

  }

  removeLanguage(lang: any): void {
    this.selectedLanguages = this.selectedLanguages.filter(l => l !== lang);
    this.eventEdit.markAsDirty();
  }

  onInputBlur() {
    setTimeout(() => (this.searching = false), 250);
  }
  //#endregion

  //#region Edit
  initEditForm() {
    this.eventEdit = new FormGroup({
      title: new FormControl('', [Validators.required, Validators.minLength(3)]),
      teacher: new FormControl('', Validators.required),
      category: new FormControl('', Validators.required),
      language: new FormControl('', Validators.required),
      startDate: new FormControl('', Validators.required),
      endDate: new FormControl('', Validators.required),
      location: new FormControl('', Validators.required),
      city: new FormControl('', Validators.required),
      description: new FormControl('', Validators.required),
      image: new FormControl(''),
      // imageFile: new FormControl(null),
    })

    // If theres data, fill the form
    if (this.event) {
      this.eventEdit.patchValue({ ...this.event });
    }

    if (this.eventEdit.value.language) this.eventEdit.value.language.split(',').forEach((langs: any) => this.selectedLanguages.push(langs))

    this.eventEdit.markAsPristine();
    this.originalEvent = { ...this.event }; //Sets the originalEvent as the event from db
    this.originalLanguages = [...(this.selectedLanguages || [])];
  }

  // Get to compare edited form with the original, to see if a value is still the same
  // Automatically verifies if image changed
  get formUnchanged(): boolean {
    const current = this.cleanFormValue(this.eventEdit.getRawValue());
    const original = this.originalEvent;

    const formEqual = JSON.stringify(current) === JSON.stringify(original);

    const languagesEqual =
      this.selectedLanguages.length === this.originalLanguages.length &&
      this.selectedLanguages.every(lang => this.originalLanguages.includes(lang));

    return formEqual && languagesEqual;
  }

  // Cleans values before comparison
  cleanFormValue(value: any) {
    const clone = structuredClone(value);

    if (clone.languages) clone.languages = this.selectedLanguages
    if (clone.startDate) clone.startDate = new Date(clone.startDate).toISOString();
    if (clone.endDate) clone.endDate = new Date(clone.endDate).toISOString();

    return clone;
  }

  // Activates Aside
  onEditAside(value: boolean) {
    this.editAside = value;
  }

  // Deactivates Aside
  closeEditAside() {
    this.editAside = false;
  }

  saveEventChanges() {
    if (this.eventEdit.invalid) {
      this.eventEdit.markAllAsTouched();
      return;
    }

    this.editAside = false; //Closes aside

    // Prepares data to be sent to ActionsComponent
    const updatedEvent = {
      ...this.eventEdit.getRawValue(), // Form fields
      language: this.selectedLanguages.join(',') // Send languages selected as string
    };

    // Updates "original state"
    this.originalEvent = { ...this.eventEdit.value };
    this.originalLanguages = [...this.selectedLanguages];

    // Send to ActionsComponent

    this.eventEdited = updatedEvent

  }
  // After editing, reloads page
  reloadEvent() {
    this.eventService.GetEvent(this.eventGuid).subscribe(
      (e) => {
        if (!e) return;
        
        this.event = e;
        this.eventStatus = e.eventStatus;
        this.category = this.categories.find(c => c.name == e.category);
        
        // Atualiza também o form se estiver aberto
        if (this.eventEdit) {
          this.eventEdit.patchValue({ ...e });
        }
      });
    }
    
  //#endregion
    
  //#region Delete
  onDeletePopUp(value: boolean){
    this.deletePopup = value;
  }

  onCancelPopup(){
    this.deletePopup = false
  }

  confirmDelete(){
    this.toDelete = true

    this.eventService.deleteEvent(this.eventGuid).subscribe({
      next: (res)=>{
        console.log("DELETED EVENT", this.toDelete)
        this.router.navigate(['/private/dashboard'])
        this.deletePopup = false

      },
      error: (_)=>{
        console.log("Event wasn't deleted! ")
      }
    })
  }
  //#endregion
  
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

        } else {
          this.showRegisterButton = true;
          this.isRegistered = false

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
