import { Component } from '@angular/core';
import { RouterOutlet } from '@angular/router';
import { ThemeService } from './services/theme.service';

@Component({
  selector: 'app-root',
  standalone: true,
  imports: [RouterOutlet],
  templateUrl: './app.html',
  styleUrl: './app.scss'
})
export class App {
  constructor(private themeService: ThemeService) {
    // Ensure theme class is applied on bootstrap
    this.themeService.setTheme(this.themeService.theme());
  }
}
