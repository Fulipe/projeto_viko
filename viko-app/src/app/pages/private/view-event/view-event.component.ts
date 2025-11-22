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
import { UserService } from '../../../services/user.service';

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
  private userService = inject(UserService)

  eventGuid!: string;
  event!: EventFetched;

  roleSaved: string | null = this.authService.getRole()
  nameSaved: string | null = this.authService.getName()

  compareNames!: boolean;

  //Edit
  editAside: boolean = false;
  eventEdit!: FormGroup; // Clone for event editing
  eventEdited: any = {};
  originalEvent!: any;

  selectedFile?: File;
  previewUrl: string | ArrayBuffer | null = null;

  minDate!: string;

  //Teachers
  teachers: any[] = []

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
  
  eventIsViewed: boolean = true

  //Republish
  republishPopup: boolean = false
  toRepublish: boolean = false

  //Erase
  erasePopup: boolean = false
  toErase: boolean = false

  //Registration 
  eventStatus: number;
  isHidden = false;
  showRegisterButton = true;
  isRegistered = false;

  constructor(private route: ActivatedRoute, private router: Router) {
    this.eventGuid = String(this.route.snapshot.paramMap.get('guid'))
    this.event = this.route.snapshot.data['event'];
    
    this.eventStatus = this.event.eventStatus;

    this.eventIsViewed = this.event.isViewed;

    // Checks if event opened, is from teacher logged in, to show, or not, Actions component 
    this.compareNames = this.nameSaved === this.event.teacher;

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

      // Prohibit selection of date before tomorrow date
      const tomorrow = new Date();
      tomorrow.setDate(tomorrow.getDate() + 1);

      this.minDate = tomorrow.toISOString().slice(0, 16);

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
      registrationDeadline: new FormControl('', Validators.required),
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

    // Only activates function getAllTeachers if role is Admin
    if(this.roleSaved == "Admin"){
      this.getAllTeachers()
    }
    
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

  getAllTeachers(){
    this.userService.getAllTeachers().subscribe({
      next: (res)=>{
        const e:any = res
        const teachers = e

        this.teachers = teachers
        console.log(this.teachers)
      }
    })
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

    this.eventService.editEvent(this.eventGuid, updatedEvent).subscribe({
      next: () => {
        console.log('Evento guardado com sucesso!');
        this.reloadEvent()
      },
      error: (err) => {
        console.error('Erro ao guardar evento:', err);
      }
    });
  }
  // After editing, reloads page
  reloadEvent() {
    return window.location.reload()
  }

  //#endregion

  //#region Delete
  onDeletePopUp(value: boolean) {
    this.deletePopup = value;
  }

  onCancelPopup() {
    this.deletePopup = false
  }

  confirmDelete() {
    this.toDelete = true

    this.eventService.deleteEvent(this.eventGuid).subscribe({
      next: (res) => {
        console.log("DELETED EVENT", this.toDelete)
        this.router.navigate(['/private/dashboard'])
        this.deletePopup = false

      },
      error: (_) => {
        console.log("Event wasn't deleted! ")
      }
    })
  }
  //#endregion

  //#region Republish
  onRepublishPopUp(value: boolean) {
    this.republishPopup = value;
  }

  onCancelRepublishPopup() {
    this.republishPopup = false
  }

  confirmRepublish() {
    this.republishPopup = true

    this.eventService.republishEvent(this.eventGuid).subscribe({
      next: (res) => {
        console.log("Republished EVENT", this.toRepublish)
        this.router.navigate(['/private/dashboard'])
        this.republishPopup = false

      },
      error: (_) => {
        console.log("Event wasn't republished! ")
      }
    })
  }
  //#endregion

  //#region Erase
  onErasePopUp(value: boolean) {
    this.erasePopup = value;
  }

  onCancelErasePopup() {
    this.erasePopup = false
  }

  confirmErase() {
    this.erasePopup = true

    this.eventService.eraseEvent(this.eventGuid).subscribe({
      next: (res) => {
        console.log("Erase EVENT", this.toErase)
        this.router.navigate(['/private/dashboard'])
        this.erasePopup = false

      },
      error: (_) => {
        console.log("Event wasn't Erased! ")
      }
    })
  }
  //#endregion

  //#region Registration
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
  //#endregion
}
