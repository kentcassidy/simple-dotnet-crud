# IntroSite: Test Point Tracker

A small full-stack CRUD app for tracking test points through their lifecycle (Planned → Executed → Passed / Failed), built as a hands-on introduction to C# and modern .NET.

I built this to get some working experience in the .NET web stack. I've built full-stack apps before on WAMP and Laravel, so the architecture was familiar: ORM, DI container, routing, and a component UI / Views. The goal was to learn how C# and ASP.NET Core express those same ideas, and where the runtime model differs from C++, Rust, and PHP.

## Stack

- **C# / .NET 10**
- **ASP.NET Core**: minimal API endpoints
- **Blazor** (Interactive Server): component-based UI in C#
- **Entity Framework Core** with **SQLite**

## Design notes

- **Dependency injection throughout.** The database context is registered once at startup and supplied by the container to API endpoints and the UI.
- **Context factory for Blazor.** Server-side Blazor connections are long-lived and `DbContext` isn't thread-safe, so the UI creates a short-lived context per operation (`await using`). API endpoints use the framework's per-request scope.
- **Async end to end.** Every database call is awaited. Nothing blocks on `.Result` or `.Wait()`.
- **Schema via `EnsureCreated`.** This is a deliberate demo shortcut. A production version would use EF Core migrations.
- **No authentication.** Left out on purpose. An internal tool would more likely use Windows auth, Entra ID, or CAC/PKI than self-registered accounts. Had to balance MVP and available time.

## Run it

Requires the [.NET 10 SDK](https://dotnet.microsoft.com/download).

```
git clone <repo-url>
cd IntroSite
dotnet run
```

Open the URL printed in the console and go to `/testpoints`. The SQLite database (`tracker.db`) is created on first run.

## API

| Method | Route | Action |
|---|---|---|
| GET | `/api/testpoints` | List all |
| GET | `/api/testpoints/{id}` | Get one |
| POST | `/api/testpoints` | Create |
| PUT | `/api/testpoints/{id}` | Update name/status |
| DELETE | `/api/testpoints/{id}` | Delete |

Example (PowerShell):

```powershell
Invoke-RestMethod -Method Post -Uri http://localhost:PORT/api/testpoints `
  -ContentType "application/json" -Body '{"name":"RWR self-test","status":0}'
```

Status values: `0` Planned, `1` Executed, `2` Passed, `3` Failed.
