import { Component } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormBuilder, ReactiveFormsModule, Validators } from '@angular/forms';
import { Router, RouterLink } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { RegisterRequest } from '../models/auth';

@Component({
  selector: 'app-register',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterLink],
  templateUrl: './register.component.html'
})
export class RegisterComponent {
  errorMessage = '';
  successMessage = '';
  loading = false;
  form = this.fb.group({
    userName: ['', [Validators.required, Validators.minLength(3)]],
    password: ['', [Validators.required, Validators.minLength(6)]],
    confirmPassword: ['', [Validators.required]]
  });

  constructor(private fb: FormBuilder, private authService: AuthService, private router: Router) {}

  submit(): void {
    if (this.form.invalid || this.form.value.password !== this.form.value.confirmPassword) {
      this.form.markAllAsTouched();
      this.errorMessage = 'Passwords must match and meet the requirements.';
      return;
    }

    this.loading = true;
    this.errorMessage = '';
    const payload: RegisterRequest = {
      userName: this.form.value.userName!,
      password: this.form.value.password!
    };

    this.authService.register(payload).subscribe({
      next: () => {
        this.loading = false;
        this.successMessage = 'Registration successful! You can now log in.';
        setTimeout(() => this.router.navigate(['/login']), 800);
      },
      error: (err) => {
        this.loading = false;
        this.errorMessage = err?.error ?? 'Registration failed.';
      }
    });
  }
}
