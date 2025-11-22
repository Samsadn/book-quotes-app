import { Routes } from '@angular/router';
import { LoginComponent } from './auth/login.component';
import { RegisterComponent } from './auth/register.component';
import { BookFormComponent } from './books/book-form.component';
import { BookListComponent } from './books/book-list.component';
import { authChildGuard, authGuard } from './guards/auth.guard';
import { QuoteListComponent } from './quotes/quote-list.component';
import { ShellComponent } from './layout/shell.component';

export const routes: Routes = [
  { path: 'login', component: LoginComponent },
  { path: 'register', component: RegisterComponent },
  {
    path: '',
    component: ShellComponent,
    canActivate: [authGuard],
    canActivateChild: [authChildGuard],
    children: [
      { path: 'books', component: BookListComponent },
      { path: 'books/new', component: BookFormComponent },
      { path: 'books/:id/edit', component: BookFormComponent },
      { path: 'quotes', component: QuoteListComponent },
      { path: '', pathMatch: 'full', redirectTo: 'books' }
    ]
  },
  { path: '**', redirectTo: 'books' }
];
