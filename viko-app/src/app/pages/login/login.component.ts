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

  msg = '';
  loginAlert = false;

  form = this.fb.group({
    username: this.fb.nonNullable.control('', Validators.required),
    password: this.fb.nonNullable.control('', Validators.required)
  });

  onSubmit() {
    if (this.form.invalid) return;

    const credentials: LoginRequest = this.form.getRawValue();

    this.auth.login(credentials).subscribe({
      next: (res) => {
        if (res.token) {
          this.router.navigate(['/private/home'])
          alert(res.msg)
        } else {
          this.loginAlert = true;
          this.msg = res.msg ?? "Authentication failed.";
        }
      },
      error: (err) => {
        this.loginAlert = true;
        this.msg = err.error?.msg ?? "Authentication failed.";
        console.error('Login failed', err)
      }
    });
  }
}
