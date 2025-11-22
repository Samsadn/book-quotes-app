import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { Router, RouterLink } from '@angular/router';
import { Book } from '../models/book';
import { BookService } from '../services/book.service';

@Component({
  selector: 'app-book-list',
  standalone: true,
  imports: [CommonModule, RouterLink],
  templateUrl: './book-list.component.html'
})
export class BookListComponent implements OnInit {
  books = signal<Book[]>([]);
  loading = signal<boolean>(true);
  error = signal<string>('');

  constructor(private bookService: BookService, private router: Router) {}

  ngOnInit(): void {
    this.loadBooks();
  }

  loadBooks(): void {
    this.loading.set(true);
    this.bookService.getBooks().subscribe({
      next: (books) => {
        this.books.set(books);
        this.loading.set(false);
      },
      error: () => {
        this.error.set('Could not load books. Make sure you are logged in.');
        this.loading.set(false);
      }
    });
  }

  deleteBook(id?: number): void {
    if (!id) return;
    if (!confirm('Delete this book?')) return;

    this.bookService.deleteBook(id).subscribe({
      next: () => this.loadBooks(),
      error: () => this.error.set('Failed to delete the book.')
    });
  }

  goToAdd(): void {
    this.router.navigate(['/books/new']);
  }
}
