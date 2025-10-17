import { Component, inject, OnInit } from '@angular/core';
import { UserService } from '../../../services/user.service';
import { UserInfo } from '../../../interfaces/interfaces';
import { COUNTRY_CODES } from '../../../interfaces/country-codes';
import { splitDialAndNumber } from '../../../services/dialcodes-helper.service';

import { CommonModule } from '@angular/common';
import { FormBuilder, FormControl, FormGroup, FormsModule, NgForm, ReactiveFormsModule } from '@angular/forms';

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

  loading = true

  user: UserInfo = {} as UserInfo;
  selectedFile?: File;
  previewUrl: string | ArrayBuffer | null = null;

  countryCodes = COUNTRY_CODES;
  selectedCode = '+370';
  phoneNumber = '';
  fullPhone = '';

  ngOnInit() {
    this.userService.userInfo().subscribe({
      next: (user) => {
        this.user = user
        this.previewUrl = user.photo ? user.phone : null
        
        const {countryCode, number} = splitDialAndNumber(this.user.phone)
        this.selectedCode = countryCode;
        this.phoneNumber = number;
        
        console.log(this.user);
        this.loading = false;
      },
      error: (err) => {
        console.error('Erro ao carregar perfil:', err)
        this.loading = false;
      }
    })
  }
  
  onFileSelected(event: Event, form: NgForm) {
    const input = event.target as HTMLInputElement;
    if (!input.files?.length) return;
  
    this.selectedFile = input.files[0];
  
    // Show Image preview
    const reader = new FileReader();
    reader.onload = () => {
      const base64 =  reader.result as string
      this.previewUrl = base64;
      this.user.photo = base64;
      form.form.markAsDirty();   
    };
    reader.readAsDataURL(this.selectedFile);
  }

  onCodeChange() {
    this.combinePhone();
    console.log(this.selectedCode)
    console.log(this.phoneNumber)

  }

  combinePhone() {
    // Remove apenas o indicativo exato se o número começar por ele
    let cleanNumber = this.phoneNumber.trim();

    if (cleanNumber.startsWith(this.selectedCode)) {
      cleanNumber = cleanNumber.slice(this.selectedCode.length);
    }

    this.fullPhone = `${this.selectedCode}${cleanNumber}`;
  }

  onSave(form: NgForm){
    if (!form.dirty) return; // Evita chamadas desnecessárias

    this.combinePhone()
    this.user.phone = this.fullPhone

    console.log('Perfil atualizado:', this.user);
    this.userService.userUpdate(this.user).subscribe({
      next: () => {
        console.log('Perfil guardado com sucesso!');
        form.form.markAsPristine(); // limpa o estado dirty
      },
      error: (err) => {
        console.error('Erro ao guardar perfil:', err);
      }
    });
  }  
}
