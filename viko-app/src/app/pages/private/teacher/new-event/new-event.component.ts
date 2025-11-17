import { Component, ElementRef, ViewChild } from '@angular/core';
import { FormGroup, FormBuilder, Validators, ReactiveFormsModule } from '@angular/forms';
import { Router } from '@angular/router';
import { EventService } from '../../../../services/event.service';
import { LANGUAGES } from '../../../../interfaces/languages';
import { CreateEventDto, LanguagesObjectFormat } from '../../../../interfaces/interfaces';
import { Title } from '@angular/platform-browser';
import { CATEGORIES } from '../../../../interfaces/categories';

@Component({
  selector: 'app-new-event',
  standalone: true,
  imports: [ReactiveFormsModule],
  templateUrl: './new-event.component.html',
  styleUrl: './new-event.component.scss'
})
export class NewEventComponent {
  eventForm: FormGroup;
  isSubmitting = false;
  successMessage = '';
  errorMessage = '';

  //Category
  categories = [...CATEGORIES]
  selectedCategory: string = '';

  //Language
  languages = [...LANGUAGES];
  selectedLanguages: any[] = [];
  @ViewChild('inputEl') inputEl!: ElementRef<HTMLInputElement>;
  searching = false

  //Photo
  selectedFile?: File;
  previewUrl: string | ArrayBuffer | null = null;

  constructor(private fb: FormBuilder, private eventService: EventService, private router: Router) {
    this.eventForm = this.fb.group({
      title: ['', [Validators.required, Validators.minLength(3)]],
      image: [null],
      description: [''],
      category: ['', Validators.required],
      languages: ['', Validators.required],
      location: ['', Validators.required],
      city: ['', Validators.required],
      startDate: ['', Validators.required],
      endDate: ['', Validators.required],
    });
  }

  //Category
  checkCategory() {
    this.selectedCategory = this.eventForm.get('category')?.value || '';
    if (!this.categories.find(s => s.name == this.selectedCategory)) {
      return false
    }

    return true
  }

  //Language
  onSearch() {
    this.searching = true

    const query = this.eventForm.get('languages')?.value || '';

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

    this.eventForm.get('languages')?.setValue(this.selectedLanguages);
    this.inputEl.nativeElement.value = ''
    this.eventForm.markAsDirty();

  }

  removeLanguage(lang: any): void {
    this.selectedLanguages = this.selectedLanguages.filter(l => l !== lang);
    this.eventForm.get('languages')?.setValue(this.selectedLanguages);
    this.eventForm.markAsDirty();
  }

  onInputBlur() {
    setTimeout(() => (this.searching = false), 250);
  }

  //Image
  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    this.selectedFile = input.files[0];

    // Show Image preview
    const reader = new FileReader();
    reader.onload = () => {
      const base64 = reader.result as string
      this.previewUrl = base64;
    };
    reader.readAsDataURL(this.selectedFile);
  }

  private saveNewEvent() {
    const dto: CreateEventDto = {
      Title: this.eventForm.get('title')?.value,
      Image: this.previewUrl?.toString(),
      Description: this.eventForm.get('description')?.value,
      Category: this.eventForm.get('category')?.value,
      Language: this.selectedLanguages.join(','),
      City: this.eventForm.get('city')?.value,
      Location: this.eventForm.get('location')?.value,
      StartDate: this.eventForm.get('startDate')?.value,
      EndDate: this.eventForm.get('endDate')?.value,

    }

    return dto
  }

  onSubmit() {
    if (this.eventForm.invalid) {
      this.eventForm.markAllAsTouched();
      console.log("Invalid Form")
      Object.keys(this.eventForm.controls).forEach(key => {
        const control = this.eventForm.get(key);
        if (control?.invalid) {
          console.log(`${key} is invalid`, control.errors);
        }
      });

      console.log(this.saveNewEvent());
      return;
    }

    this.isSubmitting = true;
    this.successMessage = '';
    this.errorMessage = '';

    this.eventService.createEvent(this.saveNewEvent()).subscribe({
      next: (res) => {
        this.successMessage = 'Event created successfully!';
        this.isSubmitting = false;

        // Redirect after 1,5 seconds
        setTimeout(() => this.router.navigate(['/private/teacher/myevents']), 1500);
      },
      error: (err) => {
        this.errorMessage = err.error?.message || 'Failed to create event';
        this.isSubmitting = false;
      }
    });
  }

}

