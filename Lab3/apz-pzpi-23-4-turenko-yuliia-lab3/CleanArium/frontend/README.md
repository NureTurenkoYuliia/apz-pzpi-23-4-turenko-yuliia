# CleanArium — Admin Frontend

React + TypeScript + Tailwind admin panel for the CleanArium .NET Web API.

## Stack

- **React 18** + **TypeScript**
- **Vite** (dev server + bundler)
- **Tailwind CSS** (styling)
- **React Router v6** (routing + role guards)
- **Axios** (HTTP client with JWT interceptors + auto refresh)
- **React Hook Form** + **Zod** (forms + validation)
- **i18next** + **react-i18next** (EN / UK localization)
- **Recharts** (daily activity area chart)
- **React Datepicker** (date range for correlation query)
- **React Hot Toast** (notifications)
- **jwt-decode** (client-side token parsing)
- **Lucide React** (icons)

## Project Structure

```
src/
├── api/
│   ├── client.ts          # Axios instance + JWT interceptor + refresh logic
│   ├── auth.ts            # Auth endpoints (login, register, logout, refresh)
│   ├── admin.ts           # Admin endpoints
│   └── users.ts           # User endpoints
├── components/
│   ├── charts/
│   │   ├── DailyActivityChart.tsx
│   │   └── CorrelationTable.tsx   # Shared Admin + Moderator
│   ├── common/
│   │   ├── LoadingSpinner.tsx
│   │   └── ProtectedRoute.tsx
│   ├── layout/
│   │   ├── Sidebar.tsx
│   │   └── DashboardLayout.tsx
│   └── modals/
│       ├── ConfirmModal.tsx
│       └── UserDetailModal.tsx
├── i18n/
│   ├── index.ts           # i18next setup (auto-detects browser language)
│   └── locales/
│       ├── en.json
│       └── uk.json
├── pages/
│   ├── admin/
│   │   ├── AdminDashboard.tsx
│   │   ├── UsersPage.tsx
│   │   ├── InactiveUsersPage.tsx
│   │   ├── ModeratorsPage.tsx
│   │   ├── SystemSettingsPage.tsx
│   │   └── AnalyticsPage.tsx
│   ├── moderator/
│   │   └── ModeratorDashboard.tsx
│   └── shared/
│       ├── LoginPage.tsx
│       ├── RegisterPage.tsx
│       └── NoAccessPage.tsx
├── store/
│   └── AuthContext.tsx    # Auth context + provider
├── types/
│   └── index.ts           # All TypeScript types / DTOs / enums
└── utils/
    ├── tokenUtils.ts      # JWT decode, token storage helpers
    └── roleRedirect.ts    # Role → home path mapping
```

## Role Routing

| Role      | Home                    | Can access                                         |
|-----------|-------------------------|----------------------------------------------------|
| Admin     | `/admin/dashboard`      | All pages                                          |
| Moderator | `/moderator/dashboard`  | Users, Inactive Users, Analytics                   |
| User      | `/no-access`            | "No permission" page only                          |

## Getting Started

```bash
npm install
npm run dev
```

The dev server runs on **http://localhost:3000** and proxies `/api` to `http://localhost:5250`.

## Environment / API URL

The API base URL is set in `src/api/client.ts`:

```ts
const BASE_URL = 'http://localhost:5250';
```

Change it there or extract to a `.env` file (`VITE_API_URL`).

## Localization

Language is auto-detected from the browser. The user can toggle EN ↔ UK via the sidebar.
Translation files: `src/i18n/locales/en.json` and `src/i18n/locales/uk.json`.
