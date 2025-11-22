import { HttpClient } from '@angular/common/http';
import { Injectable, computed, inject, signal } from '@angular/core';
import { tap } from 'rxjs/operators';
import { API_BASE_URL } from '../config/app.constants';
import { AuthResponse, LoginRequest, RegisterRequest } from '../models/auth';
import { StorageService } from './storage.service';

@Injectable({ providedIn: 'root' })
export class AuthService {
  private readonly apiUrl = `${API_BASE_URL}/auth`;
  private readonly http = inject(HttpClient);
  private readonly storage = inject(StorageService);
  private readonly isAuthenticatedSignal = signal<boolean>(false);
  readonly isAuthenticated = computed(() => this.isAuthenticatedSignal());

  constructor() {
    this.syncAuthStateFromStorage();
  }

  private syncAuthStateFromStorage(): void {
    this.isAuthenticatedSignal.set(!!this.storage.getToken());
  }

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