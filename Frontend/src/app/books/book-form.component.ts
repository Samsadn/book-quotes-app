import { CommonModule } from '@angular/common';
import { Component, OnInit } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { ActivatedRoute, Router, RouterModule } from '@angular/router';
import { BookService } from '../services/book.service';
import { Book } from '../models/book';

@Component({
  selector: 'app-book-form',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule, RouterModule],
  templateUrl: './book-form.component.html'
})
export class BookFormComponent implements OnInit {
  isEdit = false;
  loading = false;
  error = '';
  form: FormGroup;

  constructor(
    private fb: FormBuilder,
    private route: ActivatedRoute,
    private router: Router,
    private bookService: BookService
  ) {
    this.form = this.buildForm();
  }

  get titleControl(): AbstractControl {
    return this.form.get('title')!;
  }

  get authorControl(): AbstractControl {
    return this.form.get('author')!;
  }

  get publicationDateControl(): AbstractControl {
    return this.form.get('publicationDate')!;
  }

  ngOnInit(): void {
    const idParam = this.route.snapshot.paramMap.get('id');
    if (idParam) {
      this.isEdit = true;
      this.loadBook(parseInt(idParam, 10));
    }
  }

  private buildForm(): FormGroup {
    return this.fb.group({
      title: ['', Validators.required],
      author: ['', Validators.required],
      publicationDate: ['', Validators.required]
    });
  }

  loadBook(id: number): void {
    this.loading = true;
    this.bookService.getBook(id).subscribe({
      next: (book) => {
        this.loading = false;
        if (!book) {
          this.error = 'Book not found.';
          return;
        }
        this.form.patchValue({
          title: book.title,
          author: book.author,
          publicationDate: book.publicationDate
        });
      },
      error: () => {
        this.loading = false;
        this.error = 'Could not load book.';
      }
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload: Book = this.form.value as Book;
    this.loading = true;

    if (this.isEdit) {
      const id = parseInt(this.route.snapshot.paramMap.get('id')!, 10);
      this.bookService.updateBook(id, payload).subscribe({
        next: () => this.router.navigate(['/books']),
        error: () => {
          this.error = 'Failed to update book.';
          this.loading = false;
        }
      });
    } else {
      this.bookService.createBook(payload).subscribe({
        next: () => this.router.navigate(['/books']),
        error: () => {
          this.error = 'Failed to create book.';
          this.loading = false;
        }
      });
    }
  }
}