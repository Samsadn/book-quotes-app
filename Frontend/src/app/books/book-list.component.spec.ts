import { provideZonelessChangeDetection } from '@angular/core';
import { TestBed } from '@angular/core/testing';
import { ActivatedRoute, Router } from '@angular/router';
import { of, throwError } from 'rxjs';
import { BookListComponent } from './book-list.component';
import { BookService } from '../services/book.service';
import { Book } from '../models/book';

class BookServiceMock {
  getBooks = jasmine.createSpy('getBooks');
  deleteBook = jasmine.createSpy('deleteBook');
}

describe('BookListComponent', () => {
  let bookService: BookServiceMock;
  let routerNavigateSpy: jasmine.Spy;
  let routerCreateUrlTreeSpy: jasmine.Spy;

  const routerStub = () => ({
    navigate: routerNavigateSpy,
    createUrlTree: routerCreateUrlTreeSpy,
    events: of(),
    serializeUrl: (url: unknown) => String(url)
  });

  const setup = () => {
    TestBed.configureTestingModule({
      imports: [BookListComponent],
      providers: [
        provideZonelessChangeDetection(),
        { provide: ActivatedRoute, useValue: {} },
        { provide: BookService, useValue: bookService },
        { provide: Router, useValue: routerStub() }
      ]
    });

    return TestBed.createComponent(BookListComponent);
  };

  beforeEach(() => {
    bookService = new BookServiceMock();
    routerNavigateSpy = jasmine.createSpy('navigate');
    routerCreateUrlTreeSpy = jasmine.createSpy('createUrlTree').and.returnValue({});
  });

  it('loads books and clears loading state on init', () => {
    const books: Book[] = [
      { id: 1, title: '1984', author: 'George Orwell', publicationDate: '1949-06-08' },
      { id: 2, title: 'Dune', author: 'Frank Herbert', publicationDate: '1965-08-01' }
    ];
    bookService.getBooks.and.returnValue(of(books));

    const fixture = setup();
    fixture.detectChanges();
    const component = fixture.componentInstance;

    expect(bookService.getBooks).toHaveBeenCalled();
    expect(component.books()).toEqual(books);
    expect(component.loading()).toBeFalse();
    expect(component.error()).toBe('');
  });

  it('sets an error message when loading books fails', () => {
    bookService.getBooks.and.returnValue(throwError(() => new Error('Failed to fetch')));

    const fixture = setup();
    fixture.detectChanges();
    const component = fixture.componentInstance;

    expect(component.error()).toBe('Could not load books. Make sure you are logged in.');
    expect(component.loading()).toBeFalse();
  });

  it('navigates to the add book page', () => {
    bookService.getBooks.and.returnValue(of([]));

    const fixture = setup();
    fixture.detectChanges();
    const component = fixture.componentInstance;

    component.goToAdd();

    expect(routerNavigateSpy).toHaveBeenCalledWith(['/books/new']);
  });

  it('confirms and deletes a book, then reloads the list', () => {
    bookService.getBooks.and.returnValue(of([]));
    bookService.deleteBook.and.returnValue(of(void 0));
    spyOn(window, 'confirm').and.returnValue(true);

    const fixture = setup();
    fixture.detectChanges();
    const component = fixture.componentInstance;

    component.deleteBook(5);

    expect(window.confirm).toHaveBeenCalledWith('Delete this book?');
    expect(bookService.deleteBook).toHaveBeenCalledWith(5);
    expect(bookService.getBooks).toHaveBeenCalledTimes(2);
  });
});