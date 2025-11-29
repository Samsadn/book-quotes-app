Book & My Quotes is a full-stack sample that pairs an ASP.NET Core 9 Web API with an Angular 20 single-page application. It lets authenticated users manage personal reading lists and favorite quotes with a responsive, Bootstrap-based UI and JWT-protected CRUD endpoints.

## Table of contents
- [Requirements](#requirements)
- [Features](#features)
- [Architecture](#architecture)
- [Prerequisites](#prerequisites)
- [Backend setup (ASP.NET Core)](#backend-setup-aspnet-core)
- [Frontend setup (Angular)](#frontend-setup-angular)
- [Running tests](#running-tests)
- [API quick reference](#api-quick-reference)
- [Authentication flow](#authentication-flow)

## Requirements
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

## Features
- **Book management:** create, edit, and delete books; view a personalized list for the logged-in user.
- **Quote collection:** maintain a separate list of favorite quotes with full CRUD support.
- **User accounts with JWT:** register and log in; tokens are issued by the API and attached to every authenticated request.
- **Responsive design:** Bootstrap layout, Font Awesome icons, and a dark/light theme toggle in the navbar.
- **Navigation:** switch between book and quote views without leaving the SPA.

## Architecture
- **Backend (`Backend/`):** ASP.NET Core 9 Web API with Entity Framework Core and SQLite. Controllers expose Auth, Books, and Quotes endpoints. JWT bearer authentication is required for all book and quote operations.
- **Frontend (`Frontend/`):** Angular 20 SPA. Services call the API, a JWT interceptor attaches the token, and components provide forms, lists, and theme toggling.
- **Database:** SQLite file `bookQuotesApp.db` in the backend working directory. You can override the connection string in `Backend/appsettings.json` (`ConnectionStrings:DefaultConnection`).
- **Ports:** The backend listens on `http://localhost:5099` (see `Backend/Properties/launchSettings.json`). The Angular dev server uses `http://localhost:4200` and is allowed by backend CORS configuration.

## Prerequisites
- [.NET 9 SDK](https://dotnet.microsoft.com/)
- [Node.js 20+](https://nodejs.org/) and npm (Angular CLI is installed via `npm install`)
- Optional: SQLite tools if you want to inspect the database file.

## Backend setup (ASP.NET Core)
1. Navigate to the backend project:
   ```bash
   cd Backend
   ```
2. Install dependencies:
   ```bash
   dotnet restore
   ```
3. Configure settings as needed in `appsettings.json`:
   - `Jwt:Key` must be a secure secret string (required).
   - `Jwt:Issuer` and `Jwt:Audience` should match your client configuration.
   - `ConnectionStrings:DefaultConnection` can point to a custom SQLite path; when empty, the API creates `bookQuotesApp.db` in the project directory.
4. Create or update the database schema (uses existing migrations):
   ```bash
   dotnet ef database update
   ```
5. Run the API:
   ```bash
   dotnet run
   ```
   The service will start on `http://localhost:5099` (and `https://localhost:7068` when HTTPS is enabled) and log the SQLite path in use.

6. Explore the interactive API docs (development profile):
   - Swagger UI: `http://localhost:5099/swagger`
   - OpenAPI JSON: `http://localhost:5099/swagger/v1/swagger.json`
   These endpoints are enabled by default in development to let you try requests and verify schemas before wiring up the Angular client.

## Frontend setup (Angular)
1. Navigate to the Angular project:
   ```bash
   cd Frontend
   ```
2. Install dependencies:
   ```bash
   npm install
   ```
3. Ensure the API base URL matches your backend port in `src/app/config/app.constants.ts` (default: `http://localhost:5099/api`).
4. Start the dev server:
   ```bash
   npm start
   ```
   Visit `http://localhost:4200` to use the app. The Angular CLI will proxy calls directly to the configured API URL.

## Running tests
- **Backend unit tests (xUnit):** Located in `Backend.Tests/`, covering controllers, services, and JWT utilities. Run from the repo root or the Backend.Tests folder:
  ```bash
  dotnet test Backend.Tests
  ```
- **Frontend unit tests (Karma/Jasmine):** Located alongside Angular components and services under `Frontend/src/app`. From the repo root, run:
  ```bash
  cd Frontend
  npm test
  ```

## Continuous integration (GitHub Actions)
- Workflow: `.github/workflows/ci.yml` runs on pushes and pull requests targeting `main`.
- Backend: Restores .NET 9 SDK dependencies, builds the solution in Release mode, and runs `dotnet test`.
- Frontend: Installs Node.js 20, installs dependencies via `npm ci`, runs `npm test -- --watch=false --browsers=ChromeHeadless`,
  and builds the Angular app for production.
- Artifacts: The workflow currently runs verification only; no build artifacts are published.

## API quick reference
Base URL: `http://localhost:5099/api`

**Auth**
- `POST /auth/register` — Register a new user (`{ userName, password }`).
- `POST /auth/login` — Authenticate and receive `{ token }`.

**Books** (requires `Authorization: Bearer <token>`)
- `GET /books` — List books for the current user.
- `POST /books` — Create a book (`{ title, author, description }`).
- `PUT /books/{id}` — Update a book.
- `DELETE /books/{id}` — Delete a book.

**Quotes** (requires `Authorization: Bearer <token>`)
- `GET /quotes` — List quotes for the current user.
- `POST /quotes` — Create a quote (`{ text, author }`).
- `PUT /quotes/{id}` — Update a quote.
- `DELETE /quotes/{id}` — Delete a quote.

## Authentication flow
1. **Register** a user via `/auth/register` (or use the Angular Register form).
2. **Log in** via `/auth/login`. The API issues a JWT signed with the configured `Jwt:Key`.
3. The Angular client stores the token in local storage and attaches it to subsequent requests via its JWT interceptor.
4. Authenticated endpoints validate the token and authorize access to book and quote data scoped to the current user.