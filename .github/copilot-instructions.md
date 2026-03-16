# Copilot Instructions for PaperTestChecker

## Project Overview
**PaperTestChecker** is an AI-powered web application that lets teachers upload photos of paper tests, automatically grades them using a Vision LLM (Groq / Gemini), and generates per-student feedback and reading recommendations.

User roles: **Admin**, **Teacher**, **Student**.

---

## Tech Stack

| Layer | Technology |
|---|---|
| Frontend | React 18, Vite, React Router v6, CSS Modules / vanilla CSS |
| Backend | ASP.NET Core 8 Web API, C# 12 |
| ORM | Entity Framework Core 8 (Npgsql / PostgreSQL) |
| Auth | JWT Bearer tokens (`Microsoft.AspNetCore.Authentication.JwtBearer`) |
| AI Integration | Groq Vision API / Google Gemini API |
| Deployment | Vercel (frontend), Render (backend API), Neon.tech (PostgreSQL) |

---

## Repository Structure

```
/
├── backend/
│   └── PaperTestChecker/          # ASP.NET Core Web API project
│       ├── Controllers/           # Thin HTTP controllers only
│       ├── Services/              # Business logic
│       ├── DTOs/                  # Request/Response data classes
│       ├── Models/                # EF Core entities
│       ├── Data/                  # AppDbContext
│       ├── Configuration/         # Options classes bound to appsettings
│       └── Extensions/            # IServiceCollection helpers
├── frontend/
│   └── src/
│       ├── api/                   # Axios/fetch wrappers for backend calls
│       ├── components/            # Shared UI components
│       ├── pages/                 # Route-level page components
│       ├── layouts/               # Layout wrappers (MainLayout, AdminLayout)
│       └── context/               # React Context (auth, etc.)
├── .github/
│   ├── copilot-instructions.md   # This file
│   └── instructions/             # Scoped instruction files
└── screenshots/
```

---

## Critical Rules (Apply Everywhere)

1. **Never hardcode secrets.** Connection strings, JWT secrets, and AI API keys must always come from configuration / environment variables. In development, secrets go in `appsettings.Development.json` (gitignored). In production, they go in environment variables.
2. **Output only the file requested.** Do not add unrequested files or extra markdown commentary around code blocks.
3. **Async all the way.** Use `async`/`await` end-to-end. Never use `.Result`, `.Wait()`, or blocking calls.
4. **Follow the existing project patterns.** Before generating a new file, look at an existing similar file (controller, service, page component) and match its conventions.
5. **Role-based access.** Every protected API endpoint must declare `[Authorize(Roles = "...")]` explicitly. Roles are: `Admin`, `Teacher`, `Student`.
