import { CommonModule } from '@angular/common';
import { Component } from '@angular/core';
import { RouterLink, RouterLinkActive, RouterOutlet } from '@angular/router';
import { AuthService } from '../services/auth.service';
import { ThemeService } from '../services/theme.service';

@Component({
  selector: 'app-shell',
  standalone: true,
  imports: [CommonModule, RouterLink, RouterLinkActive, RouterOutlet],
  templateUrl: './shell.component.html'
})
export class ShellComponent {
  isNavbarCollapsed = true;

  constructor(public authService: AuthService, public themeService: ThemeService) {}

  logout(): void {
    this.authService.logout();
  }

  toggleNav(): void {
    this.isNavbarCollapsed = !this.isNavbarCollapsed;
  }
}
