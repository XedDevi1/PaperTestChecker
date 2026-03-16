---
applyTo: frontend/src/**/*.{jsx,js}
---

# Frontend Coding Instructions (React 18 / Vite)

## Environment & API Base URL

- **Always** read the backend API URL from `import.meta.env.VITE_API_URL`. Never hardcode `localhost` or any URL.
- Use the existing abstraction layer in `src/api/` for all HTTP calls. Do not inline `fetch()` or `axios()` calls directly in components or pages.

```js
// ✅ Correct
import { getStudents } from '../api/teacherApi';

// ❌ Wrong
const res = await fetch('http://localhost:5000/api/students');
```

## Component Conventions

- File names: **PascalCase** (e.g., `StudentCard.jsx`, `TestResultsPage.jsx`).
- One component per file.
- Functional components only — no class components.
- Export components as **named exports** for shared components, **default export** for pages.

## State Management

- Use React Context from `src/context/` for global/shared state (authentication, user info).
- Do **not** introduce a new state management library (Redux, Zustand, etc.).
- Local UI state stays in `useState`; side effects and data fetching use `useEffect`.

## Routing

- Routes are defined in `src/App.jsx` using React Router v6.
- Use `<Outlet />` inside layout components (`src/layouts/`) for nested routing.
- Navigate programmatically with the `useNavigate` hook.

## Styling

- Use the global utility classes and CSS custom properties defined in `src/index.css`.
- Prefer CSS Modules for component-scoped styles (create a `ComponentName.module.css` file).
- **Do not use inline styles** (`style={{ ... }}`), except for dynamic values that cannot be expressed in CSS.
- Do not introduce Tailwind CSS or other CSS frameworks.

## Authentication

- The authentication token (JWT) is managed by the Auth context in `src/context/`.
- Attach the token to API calls inside the `src/api/` layer — do not manually set `Authorization` headers in components.
- Protected routes should check auth state from context and redirect to `/login` if unauthenticated.

## Error Handling

- Wrap API calls in `try/catch` blocks and display user-friendly error messages.
- Log errors to the console with `console.error()` during development.

## Code Style

- Use `const` for all variable declarations; use `let` only when reassignment is required.
- Use arrow functions for callbacks and event handlers.
- Destructure props and state values at the top of the function body.
- Avoid deeply nested ternaries — use early returns or helper variables instead.
