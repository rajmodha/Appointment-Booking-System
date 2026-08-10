# MindCare — Project Brain 🧠

A single reference document covering everything about the MindCare Therapist Appointment
Booking System: the database, every user flow from login to logout, and the complete
email notification system. Written for project documentation / viva purposes.

---

## 1. Project Overview

MindCare is a three-role therapist appointment booking platform where:
- **Patients** search for therapists, book sessions, pay via UPI QR, and leave feedback.
- **Therapists** manage their profile, availability, and incoming appointment requests.
- **Admin** approves therapists, verifies payments, manages refunds, and oversees the system.

The defining design decision in this project is that **money and trust flow through a
manual verification chain**, not an automated payment gateway: a patient's UPI payment is
self-reported, then **Admin manually verifies the transaction before the therapist is even
shown the request**. This was a deliberate choice to keep the payment integration simple
and beginner-friendly while still being realistic about how a small business without a
payment gateway contract would actually operate.

---

## 2. Technology Stack

| Layer | Technology |
|---|---|
| Server | ASP.NET Web Forms, C#, .NET Framework 4.8 |
| Database | MySQL (via the official `MySql.Data` connector) |
| IDE | Visual Studio 2026 Community Edition, IIS Express (local) |
| Frontend | Bootstrap 5 (base) + a custom hand-written theme (`Content/mindcare.css`) |
| QR Codes | Generated locally with the `QRCoder` NuGet package (no external API) |
| Email | Gmail SMTP via `System.Net.Mail` |
| Auth | Session-based, SHA256 password hashing (no third-party auth provider) |
| Localization | ASP.NET `.resx` resources (English, Hindi, Gujarati, German) |

---

## 3. Database Schema (11 tables)

### Roles
| Column | Type | Notes |
|---|---|---|
| RoleId | INT PK | |
| RoleName | VARCHAR(50) | 'Admin', 'Therapist', 'Patient' |

### Users
The shared login table for all three roles.

| Column | Type | Notes |
|---|---|---|
| UserId | INT PK | |
| FullName | VARCHAR(150) | |
| Email | VARCHAR(150) UNIQUE | login identifier |
| PasswordHash | VARCHAR(255) | SHA256 hash, never plain text |
| Phone | VARCHAR(20) | |
| RoleId | INT FK → Roles | |
| IsActive | TINYINT(1) | Admin can disable an account without deleting it |
| CreatedOn | DATETIME | |

### TherapyCategories
| Column | Type |
|---|---|
| CategoryId | INT PK |
| CategoryName | VARCHAR(100) |
| Description | VARCHAR(255) |

### Therapists
Extra profile data, 1-to-1 with a Users row.

| Column | Type | Notes |
|---|---|---|
| TherapistId | INT PK | |
| UserId | INT FK → Users | |
| CategoryId | INT FK → TherapyCategories | |
| Qualification | VARCHAR(255) | |
| Specialization | VARCHAR(255) | |
| Language | VARCHAR(150) | |
| Location | VARCHAR(150) | |
| Fees | DECIMAL(10,2) | per-session fee |
| ConsultationType | ENUM | 'Online', 'Offline', 'Both' |
| Bio | TEXT | |
| ProfileImage | VARCHAR(255) | path under `~/Uploads/TherapistPhotos/`, NULL until uploaded |
| ApprovalStatus | ENUM | 'Pending', 'Approved', 'Rejected' — set by Admin |

### Patients
Extra profile data, 1-to-1 with a Users row.

| Column | Type |
|---|---|
| PatientId | INT PK |
| UserId | INT FK → Users |
| DateOfBirth | DATE |
| Gender | ENUM('Male','Female','Other') |
| Address | VARCHAR(255) |

### TherapistAvailability
The weekly recurring time slots a therapist offers — this is what
`Patient/BookAppointment.aspx` reads to generate bookable time slots.

| Column | Type |
|---|---|
| AvailabilityId | INT PK |
| TherapistId | INT FK → Therapists |
| DayOfWeek | ENUM (Monday…Sunday) |
| StartTime | TIME |
| EndTime | TIME |
| IsActive | TINYINT(1) |

### Appointments
The core transactional table.

| Column | Type | Notes |
|---|---|---|
| AppointmentId | INT PK | |
| PatientId | INT FK → Patients | |
| TherapistId | INT FK → Therapists | |
| AppointmentDate | DATE | |
| AppointmentTime | TIME | |
| ConsultationType | ENUM('Online','Offline') | |
| MeetingLink | VARCHAR(255) | filled by therapist for online sessions |
| Status | ENUM | 'Pending', 'Confirmed', 'Completed', 'Cancelled', 'Rescheduled', 'Rejected' |
| Amount | DECIMAL(10,2) | snapshot of the fee at booking time |
| Notes | VARCHAR(500) | |
| CreatedOn | DATETIME | |

### Payments
UPI payment + refund tracking.

| Column | Type | Notes |
|---|---|---|
| PaymentId | INT PK | |
| AppointmentId | INT FK → Appointments | |
| Amount | DECIMAL(10,2) | |
| UpiTransactionRef | VARCHAR(100) | the patient's self-reported payment reference |
| RefundTransactionRef | VARCHAR(100) | the *outgoing* refund's own reference (if refunded) |
| RefundedOn | DATETIME | when Admin marked it refunded |
| PaymentStatus | ENUM | 'Pending', 'Success', 'Failed', 'Refunded' |
| PaymentDate | DATETIME | |

### Feedback
| Column | Type |
|---|---|
| FeedbackId | INT PK |
| AppointmentId | INT FK → Appointments |
| PatientId | INT FK → Patients |
| TherapistId | INT FK → Therapists |
| Rating | INT (1–5) |
| Comments | VARCHAR(500) |
| CreatedOn | DATETIME |

### ContactMessages
| Column | Type |
|---|---|
| MessageId | INT PK |
| FullName, Email, Subject, Message | contact form fields |
| SubmittedOn | DATETIME |
| IsRead | TINYINT(1) |

### PasswordResetTokens
| Column | Type |
|---|---|
| TokenId | INT PK |
| UserId | INT FK → Users |
| Token | VARCHAR(255) |
| ExpiryDate | DATETIME (1 hour from creation) |
| IsUsed | TINYINT(1) |

---

## 4. Site Architecture

```
MindCare/
├── Database/MindCare_Database.sql       ← full schema + seed
├── Helpers/
│   ├── DBHelper.cs        ← all parameterized SQL runs through this
│   ├── SecurityHelper.cs  ← password hashing, reset tokens
│   ├── EmailHelper.cs     ← every email template + sender
│   └── BasePages.cs       ← PatientBasePage / TherapistBasePage / AdminBasePage
├── Content/mindcare.css
├── App_GlobalResources/   ← .resx localization files (en/hi/gu/de)
├── Global.asax(.cs)       ← applies the saved language on every request
├── GenerateQR.ashx(.cs)   ← generates the UPI QR code image on demand
├── Uploads/TherapistPhotos/
├── Site.Master(.cs)       ← shared layout: navbar, account dropdown, footer,
│                             language switcher, global loading overlay
├── Default.aspx, About.aspx, FindTherapist.aspx, TherapistDetails.aspx,
│   Contact.aspx, Login.aspx, Register.aspx, ForgotPassword.aspx, ResetPassword.aspx
├── Patient/   (PatientDashboard, MyAppointments, BookAppointment, CalendarView, MyProfile, Feedback)
├── Therapist/ (TherapistDashboard, Requests, Availability, Profile, CalendarView)
└── Admin/     (AdminDashboard, ManageTherapists, ManagePatients, VerifyPayments,
                Refunds, AllAppointments, ManageCategories, ContactMessages,
                ViewFeedback, Reports, MyProfile)
```

**Authentication:** session-based (`Session["UserId"]`, `Session["FullName"]`,
`Session["RoleId"]`). Every protected page inherits from `PatientBasePage`,
`TherapistBasePage`, or `AdminBasePage`, which redirect to `Login.aspx` (with a
`ReturnUrl` so the person lands back where they were headed) if the session doesn't
match the required role.

---

## 5. Complete User Flows

### 5.1 Patient — start to end

1. **Register** on `Register.aspx`, choosing "Patient" → a `Users` row (RoleId=3) and a
   matching `Patients` row are created immediately, no approval needed → redirected to
   `Login.aspx` with a success banner.
2. **Login** → redirected to `Patient/PatientDashboard.aspx`.
3. **Browse therapists** on `FindTherapist.aspx` — filter by specialization, location,
   language, consultation type, max fee. Only `ApprovalStatus = 'Approved'` therapists
   ever appear.
4. **View a therapist's profile** on `TherapistDetails.aspx` — bio, qualifications,
   average rating, weekly availability, recent reviews.
5. **Book an appointment** (`Patient/BookAppointment.aspx`):
   - Pick a date → available time slots are generated live from the therapist's
     `TherapistAvailability` rows for that day of week, minus any slot already booked.
   - Pick consultation type (only types the therapist actually offers are shown).
   - Click **Proceed to Payment** — *nothing is saved to the database yet* at this
     point, only the choice is held in `ViewState`.
   - A UPI QR code is generated on the fly (`GenerateQR.ashx`) with the exact fee
     amount embedded in a standard `upi://pay?...` link.
   - Patient pays via their own UPI app, then enters the transaction reference back
     on the site and clicks **I've Completed Payment**.
   - *Only now* are the `Appointments` row (Status = `Pending`) and `Payments` row
     (PaymentStatus = `Pending`) actually created together. No email is sent yet.
6. **Admin verifies the payment** (see Admin flow below) — this is the gate that
   makes the appointment visible to the therapist at all.
7. **Therapist accepts or rejects** the request (see Therapist flow below).
8. **Therapist marks the session Completed** after it happens.
9. **Leave feedback** — on `Patient/MyAppointments.aspx`, any appointment at
   `Status = Completed` with no feedback yet shows a **Leave Feedback** button,
   opening `Patient/Feedback.aspx?appointmentId=X` (star rating + comment).
10. **Cancel anytime** while `Pending` or `Confirmed`, from `MyAppointments.aspx` —
    notifies the therapist, and notifies Admin about a refund *only if* the payment
    had already been verified.
11. **Track everything visually** on `Patient/CalendarView.aspx` — a monthly calendar
    with a green dot under any date that has a session; click a date to see details.
12. **Edit personal details** on `Patient/MyProfile.aspx` (name, phone, DOB, gender,
    address — email is fixed as the login identifier).
13. **Logout** clears the session and returns to the homepage.

### 5.2 Therapist — start to end

1. **Register**, choosing "Therapist" → a `Users` row (RoleId=2) and a `Therapists`
   row are created, but `ApprovalStatus = 'Pending'` and `Fees = 0`.
2. **Cannot log in yet** — `Login.aspx` explicitly blocks Pending/Rejected therapists
   with a message explaining approval is required.
3. **Admin approves** the application (see Admin flow) → therapist can now log in.
4. **Login** → redirected to `Therapist/TherapistDashboard.aspx`.
5. **Complete the profile** (`Therapist/Profile.aspx`) — personal details (name,
   phone), professional details (category, specialization, qualification, language,
   location, fees, consultation mode, bio), and **upload a profile photo** (saved to
   `~/Uploads/TherapistPhotos/` with a unique filename; falls back to a generated
   placeholder avatar everywhere on the site until one is uploaded).
6. **Set weekly availability** (`Therapist/Availability.aspx`) — add day + start/end
   time blocks; overlapping slots on the same day are rejected.
7. **Review appointment requests** (`Therapist/Requests.aspx`) — **critically, this
   page only ever shows appointments whose payment has been verified by Admin**
   (`Payments.PaymentStatus = 'Success'`). An unverified booking is completely
   invisible here, not just hidden behind a status label.
   - **Accept** (optionally attaching a meeting link for online sessions) → status
     becomes `Confirmed`, patient gets the final confirmation email.
   - **Reject** (only while `Pending`) → status becomes `Rejected`, patient and every
     Admin account are emailed about the needed refund.
   - **Cancel Session** (only once already `Confirmed` — backing out after having
     accepted) → status becomes `Cancelled`, same dual-email pattern, worded
     differently since the patient had already been told it was confirmed.
   - **Mark Completed** (only once `Confirmed`) → unlocks the patient's ability to
     leave feedback.
8. **View their own calendar** (`Therapist/CalendarView.aspx`) — same pattern as the
   patient's, scoped to their own payment-verified sessions.
9. **Check dashboard stats** — Pending Requests, Upcoming (Confirmed), Completed
   Sessions, Average Rating (computed live from `Feedback`).
10. **Logout**.

### 5.3 Admin — start to end

1. There is no public Admin registration — the single Admin account is seeded
   directly via the database script.
2. **Login** → redirected to `Admin/AdminDashboard.aspx`, which shows system-wide
   stats: total patients, approved therapists, pending therapist approvals,
   payments awaiting verification, refunds pending, unread contact messages, total
   appointments, and **Revenue Collected** — `SUM(Amount) FROM Payments WHERE
   PaymentStatus = 'Success'` only, so `Pending`/`Failed`/`Refunded` payments never
   count toward it.
3. **Manage Therapists** — Approve/Reject pending applications; Enable/Disable any
   therapist account without deleting their data.
4. **Manage Patients** — Enable/Disable any patient account.
5. **Verify Payments** — the core trust gate. Reviews each pending payment's
   self-reported UPI transaction reference against the amount/appointment:
   - **Approve** → `PaymentStatus = 'Success'` (this is what makes the appointment
     visible to the therapist), emails both the patient ("Payment Verified") and
     the therapist ("New Appointment Request").
   - **Reject** → `PaymentStatus = 'Failed'`, the appointment is cancelled, patient
     is emailed an explanation.
6. **Refunds** — tracks every payment that was `Success` but whose appointment was
   later `Rejected`/`Cancelled` by either party. Admin sends the actual refund
   manually via their own UPI app *outside* the system, then enters that refund's
   own transaction ID here and clicks **Mark as Refunded** — this both emails the
   patient the refund reference and flips `PaymentStatus` to `Refunded`, which is
   what removes it from the Dashboard's revenue total.
7. **All Appointments** — read-only, filterable view of every appointment
   system-wide.
8. **Therapy Categories** — add/edit/delete specialization categories; delete is
   blocked (with a clear message) if any therapist is still assigned to that
   category, rather than letting a foreign-key error crash the page.
9. **Contact Messages** — view/mark-read/delete public Contact Us submissions.
10. **Feedback** — view every rating/comment across all therapists.
11. **Reports** — appointments by status, bookings by category, top-rated
    therapists.
12. **My Profile** — edit own name/phone.
13. **Logout**.

---

## 6. Appointment Lifecycle (state machine)

```
                 ┌─────────┐
   book+pay ───► │ Pending │ (Payment: Pending)
                 └────┬────┘
                       │  Admin approves payment
                       ▼
                 ┌─────────┐
                 │ Pending │ (Payment: Success — now visible to therapist)
                 └────┬────┘
          ┌────────────┼────────────┐
   Therapist Accept    │      Therapist Reject / Admin rejects payment
          ▼            │            ▼
    ┌───────────┐      │      ┌───────────┐
    │ Confirmed │      │      │ Rejected/ │
    └─────┬─────┘      │      │ Cancelled │
          │             │      └───────────┘
   ┌──────┴──────┐      │       (refund flow triggered if payment was Success)
Therapist marks  Therapist/Patient
  Completed         cancels
      ▼                ▼
┌───────────┐    ┌───────────┐
│ Completed │    │ Cancelled │
└─────┬─────┘    └───────────┘
      │
Patient can now
 leave Feedback
```

Payment status moves independently alongside this: `Pending → Success → (Refunded
if the appointment above fell through)`, or `Pending → Failed` if Admin rejects the
transaction outright.

---

## 7. Email Notification System — complete reference

All emails are sent via `EmailHelper.SendEmail()` (Gmail SMTP, configured in
`Web.config`'s `appSettings`). Every template lives in `Helpers/EmailHelper.cs`.

| # | Trigger | Sent To | Subject | Purpose |
|---|---|---|---|---|
| 1 | Admin **approves** a payment | Patient | "MindCare - Payment Verified" | Confirms payment is verified; explains the therapist still needs to accept |
| 2 | Admin **approves** a payment | Therapist | "MindCare - New Appointment Request" | Nudges them to check Requests and Accept/Reject |
| 3 | Admin **rejects** a payment | Patient | "MindCare - Payment Could Not Be Verified" | Explains the appointment was cancelled, invites retry |
| 4 | Therapist **Accepts** a request | Patient | "MindCare - Appointment Confirmed" | The real, final confirmation |
| 5 | Therapist **Rejects** a Pending request | Patient | "MindCare - Your Session Request Was Declined" | Explains the decline + that a refund is coming |
| 5b | Therapist **Rejects** a Pending request | Every Admin | "MindCare - Refund Needed (Action Required)" | Amount + UPI transaction ref needed to process the refund |
| 6 | Therapist **Cancels** an already-Confirmed session | Patient | "MindCare - Your Session Was Cancelled" | Same as #5 but worded for "this was already confirmed" |
| 6b | Therapist **Cancels** an already-Confirmed session | Every Admin | "MindCare - Refund Needed (Action Required)" | Same refund notice as 5b |
| 7 | **Patient cancels** their own appointment | Therapist | "MindCare - A Session Was Cancelled" | Informational only, no action needed |
| 7b | **Patient cancels** their own appointment | Every Admin | "MindCare - Refund Needed (Patient Cancelled)" **or** "MindCare - Appointment Cancelled by Patient" | Worded differently depending on whether the payment had actually been verified yet — if not, there's genuinely nothing to refund |
| 8 | Admin marks a payment **Refunded** | Patient | "MindCare - Your Refund Has Been Processed" | Includes the actual outgoing refund's transaction reference |
| 9 | Patient requests **Forgot Password** | The account holder (if it exists) | "Reset your MindCare password" | 1-hour, single-use reset link. The on-screen message is identical whether or not the email is actually registered, so the form can't be used to discover which emails exist in the system |

**Design principle behind the refund emails:** by the time a therapist can even see
an appointment on their Requests page, its payment has already been verified as
`Success` — so a therapist Rejecting or Cancelling always means real money needs to
come back. A patient cancelling is different: they can do it *before* Admin has even
verified the payment, so that email path checks the actual payment status and only
frames it as "refund needed" when money was genuinely already counted as collected.

---

## 8. Security Notes

- Passwords are never stored in plain text — SHA256 hashed via
  `SecurityHelper.HashPassword()`.
- All SQL goes through `DBHelper`'s parameterized queries — no string-concatenated
  SQL anywhere, protecting against SQL injection.
- Every server-side action that mutates data re-validates ownership in its `WHERE`
  clause (e.g. a patient can only cancel `WHERE PatientId = @PatientId`, a therapist
  can only act on appointments `WHERE TherapistId = @TherapistId`) — the UI hiding a
  button is never the only line of defense.
- File uploads (therapist photos) are restricted by extension whitelist and a 2MB
  size cap, saved under a generated unique filename to prevent collisions or path
  traversal from a crafted filename.
- Password reset tokens expire after 1 hour and are marked single-use immediately
  after being consumed.

---

## 9. Localization

Static site text (navigation, buttons, headings) supports **English (default),
Hindi, Gujarati, and German** via ASP.NET's `.resx` resource system:
- `App_GlobalResources/SiteText.resx` (English/fallback), `SiteText.hi.resx`,
  `SiteText.gu.resx`, `SiteText.de.resx`.
- `Global.asax`'s `Application_BeginRequest` reads a `MindCareCulture` cookie on
  every request and sets the thread's culture accordingly — this is what every
  `<%$ Resources:SiteText, Key %>` expression in markup reads from.
- A language switcher in the navbar sets that cookie client-side and reloads the
  page.
- **User-generated content is not auto-translated** — a therapist's own bio,
  specialization text, or a patient's notes stay exactly as entered, since
  translating that live would require a separate translation API integration.

This is applied to `Site.Master` and the Home page so far, with the same pattern
extendable page by page.

---

## 10. Known Limitations (worth mentioning in a viva)

- Payment verification is entirely manual (no payment gateway webhook) — by
  design, for simplicity, but it means Admin is a bottleneck and trusts the
  patient's self-reported transaction ID until they manually cross-check it.
- No automated refund transfer — Admin sends the actual money manually via their
  own UPI app; the system only *tracks* that it happened.
- Localization currently covers the shared navbar/footer and the Home page, not
  every page yet.
- Single Admin account model — no granular admin permission levels.
