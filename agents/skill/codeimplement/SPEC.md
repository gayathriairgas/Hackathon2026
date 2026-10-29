# Specification Contract — Air Liquide Loop Modernization

## 1. Overview & Purpose
This specification formalizes the product intent defined in `docs/prd.md` for the **Air Liquide Loop Modernization** full-stack application.
- **Frontend**: Angular 19 (Standalone components, Signals, Reactive state).
- **Backend**: .NET Core / .NET C# Web API.
- **Persistence**: Static JSON database engine (`users.json`, `facilities.json`, `analytics.json`).

---

## 2. Domain Model & Schemas

### 2.1 User & Persona Model (`users.json`)
```json
{
  "id": "string",
  "username": "string",
  "displayName": "string",
  "role": "StandardEmployee | LegacyADUser | SiteAdmin",
  "country": "US | France | Brazil",
  "isLegacyAd": "boolean"
}
```

### 2.2 Facility & Schedule Model (`facilities.json`)
```json
{
  "country": "string",
  "region": "North | South | East | West",
  "facilityId": "string",
  "facilityName": "string",
  "resources": [
    {
      "id": "string",
      "name": "string",
      "type": "MeetingRoom | Desk",
      "isFavorite": "boolean",
      "slots": [
        {
          "time": "09:00 AM",
          "date": "YYYY-MM-DD",
          "isBooked": "boolean",
          "bookedBy": "string | null"
        }
      ]
    }
  ]
}
```

### 2.3 Analytics & Hierarchy Model (`analytics.json`)
```json
{
  "bubbleHierarchy": {
    "country": "string",
    "regions": ["North", "South", "East", "West"]
  },
  "scalingMetrics": {},
  "floorPlans": []
}
```

---

## 3. Specification Rules

| Rule ID | PRD Req | Description & Acceptance Criteria |
| :--- | :--- | :--- |
| **R1** | FR1 | **Login Preset Selector**: The UI must render a 1-click preset selector offering quick switching between personas (Standard Employee US/FR/BR, Legacy AD User, Site Admin). |
| **R2** | FR2 | **Dual-Mode Routing Engine**: Users with `isLegacyAd == true` must be routed to `/legacy-loop`. Standard employees and admins must be routed to `/workspace`. Direct navigation must be enforced via guards. |
| **R3** | FR3 | **Localized Facility Resolution**: The system must filter and display facilities scoped to the authenticated user's country mapping. |
| **R4** | FR4 | **Quick-Favorites Ribbon**: The UI must display a ribbon constrained to exactly 2 quick-favorites, equipped with a heart toggle and a "+ More Favorites" dropdown. |
| **R5** | FR5 | **Hourly Time-Slot Grid**: The booking grid must display hourly intervals from 9:00 AM to 6:00 PM for the current date, dynamically indicating booked vs available states. |
| **R6** | FR6 | **Calendar Synchronization**: Selecting a slot must allow generating a direct Google Calendar synchronization link and downloading a standard `.ics` file. |
| **R7** | FR7 | **Frosted Onboarding Walkthrough**: A frosted backdrop blur tutorial must automatically launch on first login or when triggering the Help button. |
| **R8** | FR8 | **Admin Route Guard**: The `/admin` path must strictly permit access only to users with `role == 'SiteAdmin'`. Non-admins must be redirected to `/workspace`. |
| **R9** | FR9 | **Admin Bubble UI Hierarchy**: Admin console must render an interactive Bubble UI hierarchy covering Country, at least 4 Regions (North, South, East, West), Facilities, and Resources. |
| **R10** | FR10 | **Admin Wizards**: The admin console must provide a functional "+" Scaling Wizard and a 2D Floor Plan upload component. |
| **R11** | FR11 | **Atomic Slot Release & JSON Persistence**: Booking or canceling a slot must immediately update state in-memory and write back atomically to `facilities.json`. |

---

## 4. REST API Contract (.NET Core)

- `GET /api/auth/presets`: Retrieve all available user personas from `users.json`.
- `POST /api/auth/login`: Authenticate with selected preset profile.
- `GET /api/facilities?country={country}`: Retrieve localized facilities and rooms.
- `POST /api/bookings/reserve`: Reserve a time slot (`facilityId`, `resourceId`, `time`, `date`, `userId`). Updates `facilities.json`.
- `POST /api/bookings/cancel`: Cancel and release a time slot. Updates `facilities.json`.
- `GET /api/admin/hierarchy`: Retrieve bubble hierarchy metadata from `analytics.json`.
- `POST /api/admin/scale`: Trigger scaling configuration updates.
- `POST /api/admin/floorplan`: Register a 2D floor plan layout.

---

## 5. Global Constraints & Non-Functional Invariants
1. **No External RDBMS**: Data persistence strictly operates against static JSON files with thread-safe file stream locking.
2. **Frontend Framework**: Angular 19 using Standalone Components and Signals.
3. **Backend Framework**: .NET Core Web API with CORS enabled for the Angular client.
