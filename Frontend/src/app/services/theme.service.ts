import { Injectable, signal } from '@angular/core';
import { StorageService } from './storage.service';

type Theme = 'light' | 'dark';

@Injectable({ providedIn: 'root' })
export class ThemeService {
  private readonly themeSignal = signal<Theme>('light');
  private readonly isBrowser = typeof document !== 'undefined';

  constructor(private storage: StorageService) {
    const stored = this.storage.getTheme();
    this.themeSignal.set(stored);
    this.applyTheme(stored);
  }

  get theme() {
    return this.themeSignal.asReadonly();
  }

  toggleTheme(): void {
    const nextTheme = this.themeSignal() === 'light' ? 'dark' : 'light';
    this.setTheme(nextTheme);
  }

  setTheme(theme: Theme): void {
    this.themeSignal.set(theme);
    this.storage.setTheme(theme);
    this.applyTheme(theme);
  }

  private applyTheme(theme: Theme): void {
    if (!this.isBrowser) return;
    document.body.classList.remove('theme-light', 'theme-dark');
    document.body.classList.add(`theme-${theme}`);
  }
}
