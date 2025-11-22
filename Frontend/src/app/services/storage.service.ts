import { Injectable } from '@angular/core';

type Theme = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class StorageService {
  private readonly tokenKey = 'book-quotes-token';
  private readonly themeKey = 'book-quotes-theme';

  private get isBrowser(): boolean {
    return typeof localStorage !== 'undefined';
  }

  getToken(): string | null {
    if (!this.isBrowser) return null;
    return localStorage.getItem(this.tokenKey);
  }

  setToken(token: string): void {
    if (!this.isBrowser) return;
    localStorage.setItem(this.tokenKey, token);
  }

  clearToken(): void {
    if (!this.isBrowser) return;
    localStorage.removeItem(this.tokenKey);
  }

  getTheme(): Theme {
    if (!this.isBrowser) return 'light';
    const stored = localStorage.getItem(this.themeKey) as Theme | null;
    return stored ?? 'light';
  }

  setTheme(theme: Theme): void {
    if (!this.isBrowser) return;
    localStorage.setItem(this.themeKey, theme);
  }
}
