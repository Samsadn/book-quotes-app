import { CommonModule } from '@angular/common';
import { Component, OnInit, signal } from '@angular/core';
import { AbstractControl, FormBuilder, FormGroup, ReactiveFormsModule, Validators } from '@angular/forms';
import { Quote } from '../models/quote';
import { QuoteService } from '../services/quote.service';

@Component({
  selector: 'app-quote-list',
  standalone: true,
  imports: [CommonModule, ReactiveFormsModule],
  templateUrl: './quote-list.component.html'
})
export class QuoteListComponent implements OnInit {
  quotes = signal<Quote[]>([]);
  loading = signal<boolean>(true);
  error = signal<string>('');
  editingId: number | null = null;

  form: FormGroup;

  private readonly defaults: Quote[] = [
  
  ];

  constructor(private fb: FormBuilder, private quoteService: QuoteService) {
    this.form = this.buildForm();
  }

  get textControl(): AbstractControl {
    return this.form.get('text')!;
  }

  get authorControl(): AbstractControl {
    return this.form.get('author')!;
  }

  ngOnInit(): void {
    this.loadQuotes();
  }

  private buildForm(): FormGroup {
    return this.fb.group({
      text: ['', [Validators.required, Validators.minLength(5)]],
      author: ['', [Validators.required, Validators.minLength(2)]]
    });
  }

  loadQuotes(): void {
    this.loading.set(true);
    this.quoteService.getQuotes().subscribe({
      next: (quotes) => {
        this.quotes.set(quotes);
        this.loading.set(false);
        if (quotes.length === 0) {
          this.seedDefaults();
        }
      },
      error: () => {
        this.error.set('Could not load quotes.');
        this.loading.set(false);
      }
    });
  }

  seedDefaults(): void {
    this.defaults.forEach((quote) => {
      this.quoteService.createQuote(quote).subscribe({
        next: () => this.quoteService.getQuotes().subscribe((list) => this.quotes.set(list))
      });
    });
  }

  submit(): void {
    if (this.form.invalid) {
      this.form.markAllAsTouched();
      return;
    }

    const payload = this.form.value as Quote;
    this.loading.set(true);

    if (this.editingId) {
      this.quoteService.updateQuote(this.editingId, payload).subscribe({
        next: () => {
          this.resetForm();
          this.loadQuotes();
        },
        error: () => {
          this.loading.set(false);
          this.error.set('Failed to update quote.');
        }
      });
    } else {
      this.quoteService.createQuote(payload).subscribe({
        next: () => {
          this.resetForm();
          this.loadQuotes();
        },
        error: () => {
          this.loading.set(false);
          this.error.set('Failed to add quote.');
        }
      });
    }
  }

  editQuote(quote: Quote): void {
    this.form.patchValue({ text: quote.text, author: quote.author });
    this.editingId = quote.id ?? null;
  }

  deleteQuote(id?: number): void {
    if (!id) return;
    if (!confirm('Delete this quote?')) return;

    this.quoteService.deleteQuote(id).subscribe({
      next: () => this.loadQuotes(),
      error: () => this.error.set('Failed to delete quote.')
    });
  }

  resetForm(): void {
    this.form.reset();
    this.loading.set(false);
    this.editingId = null;
  }
}