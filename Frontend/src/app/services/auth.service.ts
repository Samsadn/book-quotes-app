import { HttpClient } from '@angular/common/http';
import { Injectable, computed, signal } from '@angular/core';
import { tap } from 'rxjs/operators';
import { API_BASE_URL } from '../config/app.constants';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth';
import { StorageService } from './storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${API_BASE_URL}/auth`;
  private readonly isAuthenticatedSignal = signal<boolean>(!!this.storage.getToken());
  readonly isAuthenticated = computed(() => this.isAuthenticatedSignal());

  constructor(private http: HttpClient, private storage: StorageService) {}

  login(credentials: LoginRequest) {
    return this.http.post<AuthResponse>(`${this.apiUrl}/login`, credentials).pipe(
      tap((response) => {
        this.storage.setToken(response.token);
        this.isAuthenticatedSignal.set(true);
      })
    );
  }

  register(payload: RegisterRequest) {
    return this.http.post<void>(`${this.apiUrl}/register`, payload);
  }

  logout(): void {
    this.storage.clearToken();
    this.isAuthenticatedSignal.set(false);
  }

  getToken(): string | null {
    return this.storage.getToken();
  }
}
