# PRD — Air Liquide Loop Modernization

|  |  |
| --- | --- |
| **Product** | Air Liquide Loop Modernization — a modernized workplace platform replacing the legacy interface

 |
| **Author** | Product Owner |
| **Status** | Draft |
| **Repo** | `[your-org]/air-liquide-loop` |
| **Last updated** | 2026-10-28 |

> **Altitude note.** This PRD is *product intent*. It describes the behaviour we want and how we'll know it's right — not the implementation. Engineering translates each accepted story into a technical specification, which becomes the source of truth the implementation follows.

---

## 1. Summary

The Air Liquide Loop Modernization is a fully implemented application replacing the legacy stark white space and isolated center icons with a functional, high-utility enterprise design. It operates with an Angular 19 frontend and a .NET 10 backend, utilizing static JSON file persistence for its database engine.

## 2. Background & problem

The legacy Air Liquide Loop interface featured a stark white space, isolated center icons, and secluded tiles. A modernized workspace is required to improve usability, offering a dual-mode routing engine to support transitioning users, a core booking UI, automated onboarding, and an advanced admin console.

## 3. Goals & non-goals

**Goals**

* Deliver a modern Angular 19 and .NET 10 application architecture.


* Provide a dual-mode routing engine that directs standard employees to a modernized Omah-style layout and legacy AD users to a classic Loop replica.


* Implement a core booking UI with quick-favorites, a workday-style time-slot grid, and calendar synchronization.


* Automate onboarding with a frosted backdrop blur tutorial and simulated video walkthrough.



**Non-goals (v1)**

* Implementation of a full relational database, as the system currently relies on static JSON schemas (`users.json`, `facilities.json`, `analytics.json`).



## 4. Personas

* **Standard Employee** — (US, France, Brazil mappings) routed to the modernized workspace.


* **Legacy AD User (`ad_user`)** — routed to the classic Loop interface replica.


* **Site Admin (`gayathri`)** — has secure access to the admin console for scaling, analytics, and floor plan management.



## 5. Experience

A user logs in using a fast 1-click preset selector. Based on their identity, they are automatically routed to either a replica of the classic Loop interface or the modernized Omah-style layout. In the modern layout, users see a top navigation bar, a left sidebar for resources, and a dynamically scoped facility view based on their country. They can quickly book rooms from a time-slot grid, add favorites, and launch interactive onboarding tutorials if they need assistance.

## 6. Functional requirements

| ID | Requirement |
| --- | --- |
| **FR1** | The login component must offer a fast 1-click preset selector mapped to static JSON profiles.

 |
| **FR2** | The routing engine must route `ad_user` profiles to `/legacy-loop` and standard/admin employees to `/workspace`.

 |
| **FR3** | The application must dynamically resolve and filter facilities to match the user's localized geography.

 |
| **FR4** | The UI must display a strict 2-item quick-favorites ribbon with a heart button toggle and a "+ More Favorites" dropdown.

 |
| **FR5** | Booking must utilize an hourly time-slot grid (9:00 AM to 6:00 PM) showing real-time booked versus available slots. The grid must dynamically default to the current date.

 |
| **FR6** | The booking modal must generate a direct Google Calendar synchronization link and a downloadable `.ics` file.

 |
| **FR7** | A frosted backdrop blur tutorial must launch on first login or on-demand via the Help button.

 |
| **FR8** | The `/admin` route must be protected by an Angular Route Guard restricted to site admins.

 |
| **FR9** | The admin console must feature an interactive Bubble UI hierarchy for countries, regions, facilities, and resources.

 |
| **FR10** | The admin console must provide a "+" Scaling Wizard and a 2D Floor Plan upload wizard.

 |
| **FR11** | Releasing or canceling a booked slot must immediately update the availability status and persist the change back to the `facilities.json` file. |

## 7. Non-functional requirements

| ID | Requirement |
| --- | --- |
| **NFR1 — Frontend Architecture** | The frontend must be built on Angular 19 with Standalone Architecture, TypeScript, and Clean CSS, running on port 4200.

 |
| **NFR2 — Backend Architecture** | The backend must utilize a .NET 10 / C# Web API running on port 5000.

 |
| **NFR3 — Persistence** | The system must persist data via static JSON files (`users.json`, `facilities.json`, `analytics.json`).

 |
| **NFR4 — Observability** | The system must track room usage efficiency analytics, including login counts, session duration, and facility utilization.

 |
| **NFR5 — Theming** | A Light/Dark theme toggle must be available and save preferences directly to the static database.

 |

## 8. User stories (the backlog)

### US1 — Dual-Mode Routing

**As a** system user, **I want** to be directed to the correct UI based on my profile **so that** I experience the appropriate layout.

**Acceptance criteria**

* Given an `ad_user` login, the user is routed to `/legacy-loop` featuring the Montreal headquarters banner and 6 secluded tiles.


* Given a standard employee or admin login, the user is routed to `/workspace` featuring the modernized Omah-style layout.



### US2 — Core Booking & Quick-Favorites

**As an** employee, **I want** to quickly favorite rooms and book them from a time grid **so that** my scheduling is efficient.

**Acceptance criteria**

* Up to 2 favorited rooms/workstations are displayed in a pinned top ribbon.


* Additional favorites are placed in a "+ More Favorites" dropdown.


* Clicking the time-slot grid between 9:00 AM and 6:00 PM opens an interactive booking popup modal. The grid view defaults to the current date.


* Confirming a reservation generates a Google Calendar sync link and `.ics` file.


* Releasing an active reservation immediately updates the UI to show the slot as available and overwrites the state change directly into `facilities.json`.

### US3 — Automated Onboarding

**As a** new employee, **I want** interactive guidance **so that** I can easily learn how to use the modern workspace.

**Acceptance criteria**

* On first login, a frosted backdrop blur tutorial automatically launches.


* The tutorial includes a simulated video player modal with chapter captioning, a playback scrubber, and replay capabilities.


* A smart dynamic route listener hooks into uncataloged pages to surface toast notifications.



### US4 — Admin Console Management

**As a** site admin, **I want** a secure console to manage facilities and track usage **so that** I can maintain and scale the application.

**Acceptance criteria**

* The `/admin` route is secured and accessible only by users mapped as site admins (`gayathri`).


* The UI displays an interactive Bubble UI hierarchy for navigating from country down to resource catalog.


* Administrators can use a "+" Scaling Wizard form dialog to add new regions and facilities to `facilities.json`.


* The console displays a tabular report tracking employee logins, session duration, and facility utilization.



## 9. Out of scope (v1)

* Migration to a fully managed relational database system (currently using static JSON schemas).



## 10. Success metrics

* **Adoption rate** — Successful routing and login counts logged within the `analytics.json` telemetry.


* **Usage tracking** — Monitoring of day of week, session duration, and facility utilization via the Room Usage Efficiency Analytics Tracker.



## 11. Prioritisation

| Story | Priority |
| --- | --- |
| US1 — Dual-Mode Routing | Must have |
| US2 — Core Booking & Quick-Favorites | Must have |
| US4 — Admin Console Management | Must have |
| US3 — Automated Onboarding | Should have |

## 12. Open questions & risks

* Strategy for migrating the static JSON file persistence (`users.json`, `facilities.json`, `analytics.json`) to a production-grade database system in future phases.


* Finalizing the full list of amenity badges (screens, whiteboards, dual monitors, seating capacity) required for all new and scaled facilities.