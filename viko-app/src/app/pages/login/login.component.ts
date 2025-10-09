import { Component, inject } from '@angular/core';
import { FormBuilder, FormsModule, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { AuthService } from '../../services/auth.service';
import { CommonModule } from '@angular/common';
import { LoginRequest } from '../../interfaces/interfaces';

@Component({
  selector: 'app-login',
  standalone: true,
  imports: [
    CommonModule,
    FormsModule,
    ReactiveFormsModule
  ],
  templateUrl: './login.component.html',
  styleUrl: './login.component.scss'
})
export class LoginComponent {
  private fb = inject(FormBuilder);
  private auth = inject(AuthService);
  private router = inject(Router);

  form = this.fb.group({
    username: this.fb.nonNullable.control('', Validators.required),
    password: this.fb.nonNullable.control('', Validators.required)
  });

  onSubmit() {
    if (this.form.invalid) return;

    const credentials: LoginRequest = this.form.getRawValue();
    
    this.auth.login(credentials).subscribe({
        next: () => this.router.navigate(['/private/dashboard']),
        error: (err) => console.error('Login failed', err)
      });
  }
}
