# Book & My Quotes App

Full-stack assignment using *.NET 9 C# Web API, **Angular 20, **JWT auth, **Bootstrap, and **Font Awesome*.

## Assignment Requirements → Implementation

| Requirement | Implementation |
|------------|----------------|
| Responsive CRUD application with token management, Angular 20 + .NET 9 C# API | Angular 20 SPA in /client, .NET 9 Web API in /Api, JWT auth and responsive Bootstrap layout |
| Page that displays a list of all books | Home page (/ and /books) shows a list of all books for the logged-in user |
| Home page with "Add New Book" button | BooksComponent header has *Add New Book* button which opens the create form |
| Create new book via form, then see it in list | Form in BooksComponent (submitNew()) calls POST /api/books and reloads list |
| Edit book via form, see updates | Edit mode in BooksComponent (startEdit, submitEdit) calls PUT /api/books/{id} |
| Delete book, removed from list | Delete button calls DELETE /api/books/{id} and reloads list |
| Register new user and login | POST /api/auth/register and POST /api/auth/login endpoints, login page in Angular |
| Backend generates token on successful login | AuthController.Login uses TokenService.CreateToken to generate JWT |
| Front-end stores token and uses it for API calls | AuthService.saveToken saves JWT in localStorage, JwtInterceptor adds Authorization: Bearer header |
| Token validation so only authenticated users access CRUD | [Authorize] on BooksController and QuotesController, JWT bearer auth configured in Program.cs |
| "My Quotes" separate view | /quotes route with QuotesComponent showing "My Quotes" |
| List of quotes with 5 pre-added quotes | QuotesComponent seeds 5 favorite quotes when user has none yet |
| Add, remove and edit quotes | add(), saveEdit(), remove() in QuotesComponent call QuotesService CRUD methods |
| Menu to switch between Book view and Quote view | Navbar links: *Book View* (/) and *My Quotes* (/quotes) |
| Responsive design | Bootstrap grid, responsive navbar (navbar-expand-lg), cards; tested on desktop and mobile viewports |
| Use Bootstrap and Font Awesome | Imported bootstrap and @fortawesome/fontawesome-free in angular.json, used icons in navbar, buttons, and titles |
| Button to switch between light and dark UX design | Theme toggle button in navbar using ThemeService, CSS for body.dark-mode |

## How to run

### Backend

```bash
cd Api
dotnet restore
dotnet ef migrations add InitialCreate
dotnet ef database update
dotnet run
