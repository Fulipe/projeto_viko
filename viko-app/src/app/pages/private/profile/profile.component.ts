import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { UserInfo } from '../../../interfaces/interfaces';
import { COUNTRY_CODES } from '../../../interfaces/country-codes';
import { splitDialAndNumber } from '../../../services/dialcodes-helper.service';
import { AbstractControl, Form, ValidationErrors, ValidatorFn } from '@angular/forms';

import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, FormsModule, NgForm, ReactiveFormsModule, Validators } from '@angular/forms';

const PHONE_PATTERN = /^[0-9+()\-\s]{6,20}$/;
const PASSWORD_PATTERN = /^(?=.*[a-z])(?=.*[A-Z])(?=.*\d).{8,}$/;

@Component({
  selector: 'app-profile',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './profile.component.html',
  styleUrl: './profile.component.scss'
})
export class ProfileComponent implements OnInit {
  private userService = inject(UserService)

  profileForm!: FormGroup; //Form of profile
  passwordForm!: FormGroup; //Form of password update

  loading = true

  user: UserInfo = {} as UserInfo;
  selectedFile?: File;
  previewUrl: string | ArrayBuffer | null = null;

  countryCodes = COUNTRY_CODES;
  selectedCode = '+370';
  phoneNumber = '';
  fullPhone = '';

  constructor(private fb: FormBuilder) {
    //Forms startup

    this.profileForm = this.fb.group({
      name: [{ value: '', disabled: true }, [Validators.required]],
      username: [{ value: '', disabled: true }, [Validators.required]],
      email: [{ value: '', disabled: true }, [Validators.required, Validators.email]],
      photo: [null],
      birthdate: [{ value: '' }, [Validators.required]],
      selectedCode: ['+370', Validators.required],
      phoneNumber: ['', [Validators.required, Validators.pattern(PHONE_PATTERN), Validators.maxLength(20)]],
      language: ['', [Validators.required]]
    });

    this.passwordForm = this.fb.group({
      password: ['', [Validators.required, Validators.pattern(PASSWORD_PATTERN)]],
      confirmPassword: ['', [Validators.required]],
    });
  };

  ngOnInit() {
    this.loadUser()
  }

  private loadUser() {
    this.userService.userInfo().subscribe({
      next: (res: UserInfo | false) => {
        console.log(res)
        if (res == false) {
          this.fillFormEmpty()
          return;
        }
        
        const u: any = res;
        const map: UserInfo = {
          name: u.Name ?? u.name ?? '',
          email: u.Email ?? u.email ?? '',
          username: u.Username ?? u.username ?? '',
          language: u.Language ?? u.language ?? '',
          birthdate: u.Birthdate ?? u.birthdate ?? '',
          phone: u.Phone ?? u.phone ?? '',
          photo: u.Photo ?? u.photo ?? null
        }

        const { countryCode, number } = splitDialAndNumber(map.phone)

        this.profileForm.patchValue({
          name: map.name,
          username: map.username,
          email: map.email,
          birthdate: map.birthdate,
          selectedCode: countryCode,
          phoneNumber: number,
          language: map.language
        })

        if (map.photo) this.previewUrl = map.photo
        if (map.phone) this.fullPhone = map.phone


        this.profileForm.markAsPristine();

        this.loading = false

        // this.user = user
        // this.previewUrl = user.photo ? user.photo : null

        // this.selectedCode = countryCode;
        // this.phoneNumber = number;

        // console.log(this.user);
        // this.loading = false;
      },
      error: (_) => {
        console.error('Erro ao carregar perfil:', _)
        this.loading = false;
        this.fillFormEmpty()
      }
    })
  }

  private fillFormEmpty() {
    this.profileForm.patchValue({
      username: '',
      name: '',
      email: '',
      birthdate: '',
      selectedCode: '+370',
      phoneNumber: '',

    });
    this.profileForm.markAsPristine();
  };

  onFileSelected(event: Event) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;

    this.selectedFile = input.files[0];

    // Show Image preview
    const reader = new FileReader();
    reader.onload = () => {
      const base64 = reader.result as string
      this.previewUrl = base64;
      this.user.photo = base64;
    };
    reader.readAsDataURL(this.selectedFile);
  }

  combinePhone() {
    // Remove apenas o indicativo exato se o número começar por ele
    const { selectedCode, phoneNumber } = this.profileForm.getRawValue();
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
    this.profileForm.get('phoneNumber')?.setValue(input.value, { emitEvent: false });

  }

  private updateUser(){
    this.user.name = this.profileForm.get('name')?.value,
    this.user.username = this.profileForm.get('username')?.value,
    this.user.email = this.profileForm.get('email')?.value,
    this.user.phone = this.combinePhone(),
    this.user.birthdate = this.profileForm.value.birthdate,
    this.user.language = this.profileForm.value.language
  }

  onSave() {
    if (this.profileForm.invalid || !this.profileForm.dirty) {
      this.profileForm.markAllAsTouched();

      return;
    };

    this.updateUser()

    console.log('Perfil atualizado:', this.user);
    this.userService.userUpdate(this.user).subscribe({
      next: () => {
        console.log('Perfil guardado com sucesso!');
        // form.form.markAsPristine(); // limpa o estado dirty
        this.loadUser()
      },
      error: (err) => {
        console.error('Erro ao guardar perfil:', err);
      }
    });
  }
}
