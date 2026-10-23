# Task Tracker API

A small REST API for managing **projects** and the **tasks** inside them, built with **ASP.NET Core (.NET 8)** and documented with **Swagger**.

![Swagger UI showing all endpoints](docs/swagger.png)

## Tech stack

- .NET 8 / ASP.NET Core Web API (controllers)
- Swagger / OpenAPI (Swashbuckle)
- SQLite database with Entity Framework Core 8 (default)
- In-memory data store (a hard-coded `List`), available with one setting

## How to run

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- Optional: Visual Studio 2022 (with the *ASP.NET and web development* workload)

No database server is needed. SQLite is a single file that the app creates itself.

### Option 1: Visual Studio

1. Open `TaskTracker.sln`.
2. Press **F5**. Swagger opens in the browser automatically.

### Option 2: Command line

```bash
git clone https://github.com/zulmajdizainuddin/task-tracker-api.git
cd task-tracker-api
dotnet run --project TaskTracker.Api
```

Then open **http://localhost:5030**. It redirects to Swagger.

> To use HTTPS instead, run `dotnet run --project TaskTracker.Api --launch-profile https` and open **https://localhost:7246**.
> If the browser warns about the certificate, run `dotnet dev-certs https --trust` once.

### Data storage

- **SQLite (default):** on first run, the app creates `TaskTracker.Api/tasktracker.db`, builds the tables from the EF Core migrations, and adds the sample data. Data **survives restarts**.
- **Reset the data:** stop the app and delete `TaskTracker.Api/tasktracker.db`. It is recreated with the sample data on the next run.
- **In-memory mode:** set `"DataStore": "InMemory"` in `TaskTracker.Api/appsettings.json`, or run `dotnet run --project TaskTracker.Api -- --DataStore=InMemory`. Data then lives in a hard-coded `List` and resets on every restart.

### Try the requests

- **Swagger:** expand any endpoint, then click **Try it out** and **Execute**.
- **Visual Studio:** open `TaskTracker.Api/TaskTracker.Api.http` and click **Send request** above any request. It covers every endpoint, including error cases.

## Requirements checklist

| Requirement | Status | Where |
|---|---|---|
| .NET 8 Web API with Swagger | ✅ Done | `TaskTracker.Api.csproj`, `Program.cs` |
| In-memory data: hard-coded `List` with at least 2 projects and 4 tasks | ✅ Done (2 projects, 5 tasks) | `Data/SeedData.cs`, `Data/InMemoryTaskTrackerRepository.cs` |
| Models: `Project` and `TaskItem` (with `Status`: Todo / InProgress / Done) | ✅ Done | `Models/` |
| CRUD endpoints for projects (5) | ✅ Done | `Controllers/ProjectsController.cs` |
| CRUD endpoints for tasks (5) | ✅ Done | `Controllers/TasksController.cs` |
| README: how to run and what was completed | ✅ Done | README.md |
| *Good to have:* database instead of the hard-coded list | ✅ Done (SQLite + EF Core; the hard-coded list is still available) | `Data/TaskTrackerDbContext.cs`, `Data/EfTaskTrackerRepository.cs`, `Migrations/` |

## Endpoints

| Method | URL | Description | Success | Errors |
|---|---|---|---|---|
| GET | `/api/projects` | List all projects with their tasks | 200 | — |
| GET | `/api/projects/{id}` | Get one project | 200 | 404 |
| POST | `/api/projects` | Create a project | 201 + `Location` header | 400 |
| PUT | `/api/projects/{id}` | Replace a project's name and description | 200 | 400, 404 |
| DELETE | `/api/projects/{id}` | Delete a project **and all of its tasks** | 204 | 404 |
| GET | `/api/projects/{id}/tasks` | List a project's tasks (`[]` if it has none) | 200 | 404 |
| POST | `/api/projects/{id}/tasks` | Create a task in a project | 201 + `Location` header | 400, 404 |
| GET | `/api/tasks/{id}` | Get one task | 200 | 404 |
| PUT | `/api/tasks/{id}` | Replace a task's title, description, status and due date | 200 | 400, 404 |
| DELETE | `/api/tasks/{id}` | Delete a task | 204 | 404 |

### Example: create a task

```http
POST /api/projects/1/tasks
Content-Type: application/json

{
  "title": "Write homepage copy",
  "description": "Short, friendly text for the hero section.",
  "status": "InProgress",
  "dueDate": "2026-11-10"
}
```

Response (on a fresh start): `201 Created`, with `Location: /api/tasks/6` and the new task in the body.

## Rules and validation

- **The server sets** `id` and `createdAt` (UTC). Clients cannot set them.
- **Project name:** required, 1–100 characters. **Task title:** required, 1–200 characters. **Description:** optional, up to 500 characters. Empty or whitespace-only names and titles are rejected.
- **Status:** must be `Todo`, `InProgress` or `Done`, sent as text (not case-sensitive). Numbers such as `2` are rejected. Defaults to `Todo` when creating a task.
- **PUT replaces the whole item:** a field left out of the body is cleared.
- **A task stays in its project:** PUT cannot move it to another project.
- **IDs are never reused** after a delete.
- **All errors** (400, 404, 500) use the standard [ProblemDetails](https://www.rfc-editor.org/rfc/rfc9457) JSON format.

## Sample data

| Project | Tasks |
|---|---|
| 1 - Website Redesign | Create wireframes (Done), Design homepage mockup (InProgress), Build contact form (Todo) |
| 2 - Mobile App Launch | Write app store description (InProgress), Set up push notifications (Todo) |

## Project structure

```text
TaskTracker.Api/
├── Controllers/           ProjectsController, TasksController (HTTP endpoints)
├── DataTransferObjects/   Request and response shapes for the API
├── Data/                  Repository interface, SQLite and in-memory implementations, DbContext, seed data
├── Migrations/            EF Core migrations that create the SQLite tables and sample data
├── Models/                Project, TaskItem, TaskItemStatus
├── Program.cs             Service registration, data store switch and request pipeline
└── TaskTracker.Api.http   Ready-to-run sample requests
```

## Design decisions

- **Repository behind an interface (`ITaskTrackerRepository`).** Controllers depend on the interface, not on how data is stored. The SQLite version was added as a second implementation without changing any controller. The methods are `async` because database calls really do wait.
- **Two data stores, one setting.** `DataStore` in `appsettings.json` chooses SQLite (default) or the in-memory list. Both use the same `SeedData`, so they start with identical sample data.
- **Different lifetimes on purpose.** The in-memory store is a **singleton**, because the object *is* the data. The SQLite repository is **scoped** (one per request), because the data lives in the file and a `DbContext` should not be shared between requests.
- **Migrations applied on startup.** `Database.Migrate()` creates or updates the database automatically, so anyone who clones the repo can run it with no setup.
- **Database details.** Deleting a project cascades to its tasks in the database itself, status is stored as text (`"Done"`) so the table is readable, and dates are read back as UTC.
- **Singleton lifetime for the in-memory store.** One shared list lives for the whole app, so data created in one request is visible in the next. A `lock` protects every change to the list.
- **Separate request and response classes (DTOs).** Clients can only send the fields they are allowed to change (no `id` or `createdAt`), and internal model changes don't leak into the API.
- **Nested and flat routes.** Tasks are created and listed under their project (`/api/projects/{id}/tasks`), then accessed directly by their own ID (`/api/tasks/{id}`), as the assignment specifies.
- **Deleting a project deletes its tasks.** A project is treated as a folder of tasks. The alternative would be refusing the delete (409 Conflict) while it still has tasks.
- **The model is called `TaskItem`, not `Task`,** to avoid clashing with .NET's `System.Threading.Tasks.Task`.

## Known limitations

- SQLite is a single file, which suits one server. Several servers sharing data would need a database server such as SQL Server.
- In in-memory mode, data **resets when the app restarts**, and each running copy of the API has its own separate data.
- In in-memory mode, responses are built after the lock is released, so under very heavy simultaneous writes a request could fail in rare cases. SQLite mode does not have this problem.
- No authentication or authorization (out of scope for this assignment).
- Swagger is only enabled in the Development environment.

## Future improvements

- Switch to SQL Server: install the SQL Server EF Core provider, change `UseSqlite` to `UseSqlServer`, and create migrations for it. The repository and controllers stay the same.
- A Blazor page that calls the API.
- Automated tests (xUnit unit tests and integration tests).
- Paging and filtering, e.g. `GET /api/projects/{id}/tasks?status=Done`.
- Authentication with JWT.
- Upgrade to .NET 10 (LTS), since .NET 8 support ends in November 2026.