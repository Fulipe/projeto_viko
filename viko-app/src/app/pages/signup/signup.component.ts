import { Component, ElementRef, inject, OnInit, ViewChild } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { COUNTRY_CODES } from '../../interfaces/country-codes';
import { LANGUAGES } from '../../interfaces/languages';
import { LanguagesObjectFormat } from '../../interfaces/interfaces';
import { CommonModule } from '@angular/common';
import { UserService } from '../../services/user.service';
import { Router } from '@angular/router';

const PHONE_PATTERN = /^[0-9+()\-\s]{6,20}$/;
const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

@Component({
  selector: 'app-signup',
  standalone: true,
  imports: [
    CommonModule,
    ReactiveFormsModule
  ],
  templateUrl: './signup.component.html',
  styleUrl: './signup.component.scss'
})
export class SignupComponent implements OnInit {
  private userService = inject(UserService)
  private router = inject(Router);

  signupForm: FormGroup;

  //Photo
  selectedFile?: File;
  previewUrl: string | ArrayBuffer | null = null;

  //Phone
  countryCodes = COUNTRY_CODES;
  selectedCode = '+370';
  fullPhone = '';

  //Language
  languages = [...LANGUAGES];
  selectedLanguages: any[] = [];
  @ViewChild('inputEl') inputEl!: ElementRef<HTMLInputElement>;
  searching = false

  
  constructor(private fb: FormBuilder) {
    
    this.signupForm = this.fb.group({
      FirstName: ['', Validators.required],
      LastName: ['', Validators.required],
      Username: ['', Validators.required],
      Email: ['', [Validators.required, Validators.email]],
      Password: ['', [Validators.required, Validators.minLength(8), Validators.pattern(PASSWORD_PATTERN)]],
      ConfirmPassword: ['', Validators.required],
      Photo: [null],
      BirthDate: [{ value: '' }, [Validators.required]],
      selectedCode: ['+370', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern(PHONE_PATTERN), Validators.maxLength(20)]],
      language: ['']
    }, { validators: [SignupComponent.passwordsMatch] });
  }
  
  ngOnInit(): void {
    this.signupForm.markAsPristine()
  }

  static passwordsMatch(control: AbstractControl) {
    const password = control.get('Password')?.value;
    const confirm = control.get('ConfirmPassword')?.value;
    return password === confirm ? null : { passwordMismatch: true };
  }

  // static passwordInvalid()

  onSearch() {
    this.searching = true

    const query = this.signupForm.get('language')?.value || '';

    if (query.trim() === '') {
      this.languages = LANGUAGES.filter(
        l => !this.selectedLanguages.includes(l.name)
      );
    } else {
      this.languages = LANGUAGES.filter(
        l => l.name.toLowerCase().startsWith(query) &&
          !this.selectedLanguages.includes(l.name));
      console.log(this.languages)
    }
  }

  selectLanguage(lang: LanguagesObjectFormat) {
    this.searching = false

    this.selectedLanguages.push(lang.name)

    this.inputEl.nativeElement.value = ''
    this.signupForm.markAsDirty();

  }

  removeLanguage(lang: any): void {
    this.selectedLanguages = this.selectedLanguages.filter(l => l !== lang);
    this.signupForm.markAsDirty();
  }

  onInputBlur() {
    setTimeout(() => (this.searching = false), 250);
  }

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

  combinePhone() {
    // Remove apenas o indicativo exato se o número começar por ele
    const { selectedCode, phoneNumber } = this.signupForm.getRawValue();
    return `${selectedCode}${phoneNumber}`;
  }

  onCodeChange() {
    this.combinePhone();
  }

  phoneValidator(event: Event): void {
    const input = event.target as HTMLInputElement;
    // Remove tudo que NÃO seja número
    input.value = input.value.replace(/[^0-9]/g, '');
    // Atualiza o valor no FormControl
    this.signupForm.get('phoneNumber')?.setValue(input.value, { emitEvent: false });

  }

  private saveUser() {
    const dto = {
      FirstName: this.signupForm.get("FirstName")?.value,
      LastName: this.signupForm.get("LastName")?.value,
      Languages: this.selectedLanguages.join(','),
      Username: this.signupForm.get("Username")?.value,
      Email: this.signupForm.get("Email")?.value,
      BirthDate:this.signupForm.value.BirthDate,
      Phone: this.combinePhone(),
      Photo: this.previewUrl,
      Password: this.signupForm.get("Password")?.value,
      ConfirmPassword: this.signupForm.get("ConfirmPassword")?.value
    }

    console.log(dto)

    return dto
  }

  onSubmit() {
    if (this.signupForm.invalid) {
      console.log("Form invalido")
      return;
    }

    this.userService.createUser(this.saveUser()).subscribe({
      next: () => {
        console.log('User Created!');
        this.router.navigate(['/login'])
      
      },
      error: (err) => {
        console.error('Error creating user:', err);
      }
    });
  }
}
