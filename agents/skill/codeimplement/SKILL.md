---
name: codeimplement
description: Full-stack implementation skill that transforms docs/prd.md into executable code using Angular 19 for the frontend, static JSON files for data persistence, and .NET Core (C#) for the backend API. Generates and synchronizes SPEC.md as needed, ensuring strict alignment between product requirements, specifications, and architecture. Use when asked to generate or implement code from docs/prd.md, build Angular 19 and .NET Core full-stack applications, or update SPEC.md and execute full-stack code implementation.
---

# /codeimplement

A comprehensive full-stack execution skill that converts product requirements (`docs/prd.md`) into a fully functional, production-ready full-stack application built with:
- **Frontend**: **Angular 19** (Standalone components, Signals, Reactive forms, Modern routing & guards, Tailwind/SCSS).
- **Backend**: **.NET Core / .NET 9/10 C# Web API** (Minimal APIs or Controller pattern, clean dependency injection, typed services, JSON persistence engine).
- **Data Persistence**: **Static JSON files** (e.g. `users.json`, `facilities.json`, `analytics.json`) serving as the datastore with thread-safe file read/write operations.
- **Specification Engine**: Automatically synthesizes or updates **`SPEC.md`** with traceable requirements, rule IDs (`R1`, `R2`, ...), domain models, and acceptance test cases.

---

## When to Use

Use this skill whenever:
- The user requests: "implement code from docs/prd.md", "generate the application", "build the Angular and .NET core app", or runs `/codeimplement`.
- Developing, scaffolding, or extending the full-stack system with Angular 19 frontend and .NET Core backend.
- Creating or updating `SPEC.md` based on `docs/prd.md` before or during code implementation.
- Implementing features spanning frontend UI, .NET Core API endpoints, and JSON file storage.

---

## Technical Stack & Architecture

### 1. Backend: .NET Core Web API
- **Framework**: .NET 8 / .NET 9 / .NET 10 (C#)
- **Architecture**:
  - `Controllers/` or Minimal API endpoints mapped to functional requirements (e.g., `AuthController`, `FacilitiesController`, `BookingsController`, `AdminController`).
  - `Services/` implementing business logic, rule evaluations, and JSON read/write operations.
  - `Models/` strongly typed C# DTOs and entity models with `System.Text.Json` serialization attributes.
  - `Data/` repository service that handles atomic read/write locks (`SemaphoreSlim` or `lock`) for JSON storage files.
- **CORS & Environment**: CORS enabled for Angular frontend (e.g. `http://localhost:4200`).

### 2. Frontend: Angular 19
- **Framework**: Angular 19 (Standalone Components, Signals, `inject()`-based dependency injection, functional route guards, modern control flow `@if`, `@for`, `@switch`).
- **Structure**:
  - `src/app/core/`: Authentication service, route guards, API client interceptors, models.
  - `src/app/features/`: Modular feature views (e.g. `auth/login`, `workspace/`, `legacy-loop/`, `booking/`, `admin/`).
  - `src/app/shared/`: Reusable components (e.g., quick-favorites ribbon, modal dialogs, bubble hierarchy charts, calendar export helpers).
- **Routing**: Dual-mode routing engine (`/workspace` vs `/legacy-loop`) guided by user role/profile.

### 3. Data Store: Static JSON Engine
- Stored under a centralized data directory (e.g., `data/` or `backend/Data/`):
  - `users.json`: Presets, personas (Standard Employee, Legacy AD User, Site Admin), roles, country mappings.
  - `facilities.json`: Countries, regions (North, South, East, West), facilities, rooms, hourly time-slot schedules (9:00 AM - 6:00 PM).
  - `analytics.json`: Usage metrics, floor plan metadata, scaling configuration.
- Thread-safe updates ensuring bookings, cancellations, and status modifications persist directly to JSON.

---

## Step-by-Step Execution Workflow

### Phase 1: Requirement Analysis & Specification (`SPEC.md`)
1. **Read `docs/prd.md`**:
   - Extract functional requirements (`FR1` to `FRn`), user personas, constraints, and non-functional requirements.
2. **Generate or Update `SPEC.md`**:
   - Check if `SPEC.md` exists at the root. If not, generate a formal specification containing:
     - **Purpose & Scope**: Summary of the system.
     - **Domain Model**: Types, Entities, Enums (User, Role, Facility, Room, TimeSlot, Booking, AdminHierarchy).
     - **Specification Rules (`R1`, `R2`, ...)**: Concrete, testable rules derived directly from `FR1`–`FR11`.
     - **API Contract**: HTTP methods, endpoints, request/response JSON payloads.
     - **Precedence Order & Global Invariants**: Thread-safety, file persistence guarantees, fallback behaviors.

### Phase 2: Static JSON Datastore Provisioning
1. Scaffold JSON datastore files matching the PRD data schemas:
   - `users.json`: Include personas (`standard_employee` with country mappings, `ad_user`, `admin` e.g., `gayathri`).
   - `facilities.json`: Seed with regional hierarchy (North, South, West, East) and 9:00 AM to 6:00 PM bookable slots.
   - `analytics.json`: Initialize baseline stats and configurations.
2. Verify valid JSON syntax and schema completeness.

### Phase 3: .NET Core Backend Implementation
1. **Scaffold / Create .NET Project**:
   - Setup Web API project (e.g., `dotnet new webapi -n Backend --use-controllers`).
   - Configure `Program.cs` with JSON options, CORS policy for Angular, and Dependency Injection.
2. **Implement Data Service Layer**:
   - Create `JsonFileRepository<T>` or dedicated service (e.g., `FacilityService`, `BookingService`, `UserService`).
   - Ensure thread-safe write locks (`SemaphoreSlim` or lock object) when writing back to disk.
3. **Implement API Controllers / Endpoints**:
   - `AuthController`: Preset selector logins, returns user identity, roles, and country profile.
   - `FacilitiesController`: Localized facility queries, hourly slot status.
   - `BookingsController`: Slot reservation, cancellation, atomic release updating `facilities.json`.
   - `AdminController`: Bubble hierarchy data, scaling wizard operations, floor plan management.
4. **Backend Tests**:
   - Add unit/integration tests verifying API contracts, JSON persistence, and edge-case handling (e.g. double booking prevention).

### Phase 4: Angular 19 Frontend Implementation
1. **Scaffold / Create Angular 19 Project**:
   - Modern Angular structure with standalone components and TypeScript strict mode.
2. **State Management & Services**:
   - Create typed HttpClient services using Angular Signals for reactivity.
   - Implement `AuthService` managing active preset profile and role state.
   - Implement `AuthGuard` protecting `/admin` and directing users dynamically.
3. **Feature Components**:
   - **Login / Preset Selector**: 1-click user switcher mapped to `users.json`.
   - **Dual-Mode Routing**: Seamless routing to `/workspace` (modernized layout) or `/legacy-loop` (classic view).
   - **Modern Workspace**:
     - Top navigation bar, localized facility filter, left resource navigation.
     - Strict 2-item Quick-Favorites ribbon with heart toggle and "+ More Favorites" menu.
     - Hourly time-slot grid (9:00 AM – 6:00 PM) showing real-time slot states.
     - Calendar modal with Google Calendar deep-link generator and downloadable `.ics` generator.
     - Frosted backdrop blur tutorial with interactive onboarding.
      - Actionable Event Tags: New Theme Release theme switcher, Project Launch milestone orchestrator with War-Room reservation, Meet manager, Create New theme, and Meeting.
   - **Admin Console (`/admin`)**:
     - Interactive Bubble UI hierarchy (Country > North/South/West/East > Facility > Resource).
     - Scaling Wizard and 2D Floor Plan management.

### Phase 5: End-to-End Verification
1. Verify that updating or booking a slot via the UI calls .NET Core API and persists to `facilities.json`.
2. Confirm route guards prevent unauthorized access to `/admin`.
3. Validate dual-mode routing redirecting legacy AD user vs standard employee.
4. Ensure all requirements from `docs/prd.md` and `SPEC.md` pass verification.

---

## Acceptance Criteria

1. **Specification Completeness**:
   - `SPEC.md` exists at the root, derived from `docs/prd.md`, with explicit domain models, contracts, and testable rule mappings (`R1`–`R11`).
2. **Frontend Architecture & Capabilities**:
   - Implemented with **Angular 19** standalone components, signal-based reactivity, and typed services.
   - Fast 1-click preset selector authenticated against `users.json`.
   - Dual-mode routing engine directing `isLegacyAd == true` to `/legacy-loop` and standard/admin users to `/workspace`.
   - Dynamic localization scoping facilities and rooms by user country.
   - Strict 2-item quick-favorites ribbon with heart toggle and "+ More Favorites" dropdown.
   - Hourly time-slot booking grid (9:00 AM to 6:00 PM) defaulting to today's date with real-time booked/available indicators.
   - Calendar integration producing Google Calendar URLs and downloadable `.ics` files.
   - Frosted backdrop blur tutorial launchable automatically on first visit or via Help.
   - Dedicated `/admin` route guarded by `RoleGuard` restricted to Site Admins.
   - Admin Bubble UI hierarchy representing Country, Regions (North, South, East, West), Facilities, and Resources.
   - Functional Admin Scaling Wizard and 2D Floor Plan layout component.
3. **Backend Architecture & Capabilities**:
   - Built on **.NET Core / C#** Web API with clean controller/service separation and dependency injection.
   - Exposes REST endpoints matching the contract in `SPEC.md`.
   - Full CORS policy configured allowing requests from the Angular development server.
4. **Data Persistence & Thread Safety**:
   - Standalone static JSON engine reading and writing to `users.json`, `facilities.json`, and `analytics.json`.
   - Slot booking and cancellation atomically modify and persist data back to `facilities.json` without file corruption or race conditions.

---

## Done When

The task is considered **DONE WHEN**:
- [ ] `SPEC.md` is generated, up to date with `docs/prd.md`, and contains all domain models and rule mappings (`R1`–`R11`).
- [ ] Static JSON datastore files (`users.json`, `facilities.json`, `analytics.json`) are initialized with valid JSON and seeded with required personas, 4 regions (North, South, East, West), and slots.
- [ ] .NET Core backend compiles without errors, serves endpoints on its configured port, and passes all backend tests.
- [ ] Angular 19 frontend compiles with zero errors, standalone components and routes render correctly, and guards enforce role permissions.
- [ ] Slot booking or release from the UI persists directly into `facilities.json`.
- [ ] Calendar sync (.ics download and Google Calendar link) executes successfully.
- [ ] The dual-mode routing and admin console with bubble hierarchy operate according to specifications.
- [ ] Verification checklist is completely satisfied and tested end-to-end.

---

## Verification Checklist

- [ ] `SPEC.md` exists, reflects all requirements in `docs/prd.md`, and assigns unique Rule IDs (`R1`, `R2`, ...).
- [ ] Static JSON files are properly initialized, valid, and editable without file corruption.
- [ ] .NET Core backend compiles, handles CORS, and exposes all required endpoints.
- [ ] Angular 19 frontend compiles with zero errors, uses standalone components, signals, and functional guards.
- [ ] Slot booking and cancellation persist reliably to JSON.
- [ ] Calendar sync (.ics and Google Calendar link) produces valid events.
- [ ] Admin console renders the 4-region hierarchy and enforces site admin security.
