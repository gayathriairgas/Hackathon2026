# Specification: Air Liquide Loop Modernization

> **Source of truth.** Derived directly from `docs/PRD.md` and visually referenced against `docs/LegacyUI.png` (Legacy Loop) and `docs/UI Design Template.png` (Modern Omah Design).

## Purpose & Scope
The Air Liquide Loop Modernization application replaces the legacy stark white space and secluded icon interface with a high-utility enterprise workplace platform. It operates with a dual-mode routing engine to support transitioning users, a core booking UI with calendar sync, automated onboarding, and an advanced admin console.

---

## Domain Model
- **UserSession / Persona**:
  - `id: string` (`ad_user` | `emp_us` | `emp_fr` | `emp_br` | `gayathri`)
  - `name: string`
  - `email: string`
  - `role: string` (`ad_user` | `employee` | `admin`)
  - `geography: string` (`Canada` | `US` | `France` | `Brazil`)
  - `theme: string` (`light` | `dark`)
- **Facility**:
  - `id: string`
  - `name: string`
  - `country: string`
  - `region: string` (`North` | `South` | `West` | `East`)
  - `city: string`
  - `address: string`
  - `resources: list[Resource]`
- **Resource**:
  - `id: string`
  - `name: string`
  - `type: string` (`Conference Room` | `Workstation` | `Board Room` | `Innovation Lab`)
  - `capacity: int`
  - `amenities: list[str]` (e.g. `Dual 4K Screens`, `Video Conferencing`, `Whiteboard`, `Standing Desk`)
  - `slots: list[Slot]`
- **Slot**:
  - `time: string` (`09:00`, `10:00`, ..., `17:00`)
  - `status: string` (`available` | `booked`)
  - `bookedBy: string | null`
- **AnalyticsTelemetry**:
  - `totalLogins: int`
  - `loginsByRole: map[string, int]`
  - `facilityUtilization: list[FacilityUtilization]`
  - `recentSessions: list[SessionRecord]`

---

## Global Constraints & Architectural Invariants
- **C1 (Data Persistence)**: The datastore MUST use static JSON files (`data/users.json`, `data/facilities.json`, `data/analytics.json`). File writes must be thread-safe.
- **C2 (Legacy Visual Invariant)**: When routed to `/legacy-loop`, the UI MUST faithfully replicate `docs/LegacyUI.png` (Montreal headquarters banner, left collapsible navigation, 6 circular colored center tiles).
- **C3 (Modern Visual Invariant)**: When routed to `/workspace`, the UI MUST faithfully adhere to `docs/UI Design Template.png` (Omah layout, rounded capsule navigation, left pastel category tags, appointment grid with circular status dots).
- **C4 (Admin Authorization)**: Route `/admin` is strictly guarded and accessible only when `user.role == "admin"` (`gayathri`).
- **C5 (Regional Hierarchy)**: The admin hierarchy under any country MUST include at least four distinct regions: `North`, `South`, `West`, and `East`.

---

## Traceable Specification Rules (FR1 – FR11)

### R1: 1-Click Preset Selector (FR1)
- **Behavior:** The application provides a 1-click preset selector in the top bar and landing page populated from `users.json`. Selecting a preset immediately sets the active session without password prompts.
- **Test:** Given `selectUser('emp_us')`, session is set to Alex Miller with `geography="US"` and `role="employee"`.

### R2: Dual-Mode Routing Engine (FR2)
- **Behavior:**
  - If `user.role == "ad_user"`, the router directs to `/legacy-loop`.
  - If `user.role in ["employee", "admin"]`, the router directs to `/workspace`.
- **Test:** `evaluateRoute('ad_user')` $\rightarrow$ `/legacy-loop`; `evaluateRoute('emp_fr')` $\rightarrow$ `/workspace`.

### R3: Dynamic Geolocation Facility Scope (FR3)
- **Behavior:** In `/workspace`, facilities and rooms are dynamically filtered to match the authenticated user's `geography`. If no exact country match exists, default to the primary facility.
- **Test:** For `emp_us`, facility header resolves to `Houston Innovation Hub (US)`. For `emp_fr`, facility resolves to `Paris Head Office Campus (France)`.

### R4: Strict 2-Item Quick-Favorites Ribbon (FR4)
- **Behavior:** The workspace displays up to 2 favorited resources in a pinned top ribbon with heart toggle icons. All additional favorites overflow into a "+ More Favorites" dropdown.
- **Test:** Pinned ribbon count $\le 2$. When 3rd item is favorited, it appears inside the dropdown.

### R5: Workday Time-Slot Grid (FR5)
- **Behavior:** Displays an hourly time-slot grid between `09:00` and `18:00` (9:00 AM to 6:00 PM) showing real-time `booked` vs `available` slots. Grid defaults to today's date.
- **Test:** Clicking an available slot opens the reservation confirmation modal.

### R6: Calendar Synchronization (.ics & Google Calendar) (FR6)
- **Behavior:** The booking modal generates a deep-link for Google Calendar synchronization and produces a downloadable RFC-5545 compliant `.ics` calendar file.
- **Test:** Triggering `downloadIcsFile()` emits an `.ics` blob with `DTSTART`, `DTEND`, and `SUMMARY`.

### R7: Frosted Backdrop Blur Tutorial (FR7)
- **Behavior:** A frosted backdrop blur tutorial launches automatically on first login or on-demand via the Help `?` button. Includes a simulated video player modal with chapter captioning, scrubber, and replay capabilities.
- **Test:** Clicking Help opens `#tutorialModal` with backdrop blur, chapter navigation, and play/pause controls.

### R8: Protected Admin Route Guard (FR8)
- **Behavior:** The `/admin` navigation route is protected by a route guard. Only authenticated site admins (`gayathri`) can access `/admin`. Unauthorized attempts are redirected to `/workspace`.
- **Test:** `canActivate('/admin', 'emp_us')` $\rightarrow$ `false`; `canActivate('/admin', 'gayathri')` $\rightarrow$ `true`.

### R9: Interactive Geographic Bubble UI Hierarchy (FR9)
- **Behavior:** The admin console displays an interactive Bubble UI hierarchy for navigating from Global $\rightarrow$ Country $\rightarrow$ at least 4 Regions (`North`, `South`, `West`, `East`) $\rightarrow$ Facilities $\rightarrow$ Resources.
- **Test:** Expanding country bubble reveals North, South, West, and East regional nodes.

### R10: Scaling Wizard & Floor Plan Wizard (FR10)
- **Behavior:** The admin console provides a "+" Scaling Wizard form dialog to add and scale new regions/facilities directly into `facilities.json`.
- **Test:** Submitting the scaling form provisions a new facility with valid ID and registers it in the datastore.

### R11: Real-Time Release & Atomic JSON Persistence (FR11)
- **Behavior:** Releasing or canceling a booked slot immediately updates the UI status to `available` and persists the state change directly to `facilities.json`.
- **Test:** Canceling a slot invokes `POST /api/facilities/slot` with `action="release"`, saving `status="available"` to `facilities.json`.

### R12: Interactive Workspace Event Tag Actions (FR12, US5)
- **Behavior:**
  - Clicking **"New Theme Release"** opens an interactive modal allowing users to toggle and apply themes (Slate Minimal, Air Liquide Cyan, Midnight Emerald, Royal Indigo) with instant UI theme application.
  - Clicking **"Project Launch"** opens a project rollout modal showing launch milestones, checklist items, and an integrated 1-click War-Room room reservation button.
  - Clicking **"Meet manager"**, **"My Event"**, **"Create New theme"**, or **"Meeting"** launches tailored calendar workflows or direct appointment scheduling.
- **Test:** Triggering `handleCategoryTag('New Theme Release')` opens `#themeReleaseModal`; triggering `handleCategoryTag('Project Launch')` opens `#projectLaunchModal` with active milestone timeline.

---

## API Contract
- `GET /api/users`: Returns JSON list of all user profiles.
- `GET /api/facilities?country={country}`: Returns facilities filtered by country or all facilities.
- `POST /api/facilities/slot`: Body `{ facilityId, resourceId, time, action: "book"|"release", userId }`. Atomically updates and persists slot state.
- `POST /api/facilities`: Body `{ name, country, region, resources }`. Appends new facility via Scaling Wizard.
- `GET /api/analytics`: Returns telemetry tracking logins, session durations, and room usage efficiency.
- `GET /health`: Health status check.

---

## Precedence Order
1. `R1` (Preset Authentication)
2. `R8` (Admin Route Guard)
3. `R2` (Dual-Mode Routing)
4. `R3` (Geolocation Scope)
5. `R4` (Quick-Favorites Ribbon)
6. `R5` (Time-Slot Grid)
7. `R6` (Calendar Sync & .ics)
8. `R11` (Slot Cancellation & Persistence)
9. `R9` (Bubble Hierarchy)
10. `R10` (Scaling Wizard)
11. `R7` (Onboarding Tutorial)
