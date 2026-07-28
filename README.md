# 🚀 Freelance Marketplace Platform

A robust, full-featured Freelance Marketplace Web Application built with **.NET Core MVC**, **Entity Framework Core**, and **ASP.NET Core Identity**. The platform seamlessly connects Clients and Freelancers, facilitating job postings, proposal submissions, real-time/automated notifications, media uploads, and mutual review systems.

---

## 🛠️ Tech Stack & Dependencies

* **Framework:** ASP.NET Core MVC (.NET 8 / .NET Core)
* **ORM & Database:** Entity Framework Core (SQL Server)
* **Authentication & Authorization:** ASP.NET Core Identity (Role-based: Admin, Client, Freelancer)
* **Background Jobs & Scheduling:** Hangfire (Automated notifications)
* **Real-Time Communication:** ASP.NET Core SignalR
* **Email Services:** SMTP / SendGrid Integration
* **Media & Cloud Storage:** Cloudinary SDK (Profile pictures, attachments)
* **Frontend:** HTML5, CSS3, Bootstrap 5, JavaScript

---

## ✨ Key Features

### 1. User Authentication & Security
* Multi-role registration and login system (**Client**, **Freelancer**, **Admin**).
* Role-based access control and action authorization.
* Password recovery and email confirmation workflows (via SMTP).

### 2. User Profiles
* **Clients:** Company details, logo upload, and active/past job postings.
* **Freelancers:** Professional bio, skill sets, downloadable portfolio (PDF/Images), and past ratings.
* **General:** Profile image management via Cloudinary, secure password updating, and activity history.

### 3. Job Posting & Management
* **Clients:** Create, update, or delete job listings with parameters including Title, Detailed Description, Budget, Fixed Deadline, Required Skills, Categories, and File Attachments.
* **Category Management:** Structured job categories and tagging system for improved discoverability.

### 4. Job Browsing & Search
* Interactive paginated grid view for Freelancers.
* Real-time search, multi-metric filtering (Category, Skills, Budget range), and multi-criteria sorting (Date Posted, Budget High/Low).

### 5. Proposal & Application System
* **Freelancers:** Submit detailed proposals (Cover Letter, Bid Amount, Estimated Delivery Timeline, and Portfolio/Attachment uploads).
* **Clients:** Application management dashboard to review, accept, or reject candidate proposals.
* **Status Tracking:** Comprehensive status flow (`Submitted`, `Under Review`, `Accepted`, `Rejected`).

### 6. Automated & Real-Time Notifications
* **Skill Matching (Hangfire):** Automated background job that matches newly posted job requirements with Freelancer skills and dispatches notification alerts.
* **In-App Notifications (SignalR):** Real-time pop-up alerts for application status updates and job matches.
* **Email Notifications (SMTP):** Direct email dispatches for critical workflow events.
* **Notification Center:** Dashboard widget with unread badge counter and full notification log history.

### 7. Reviews & Ratings System
* Two-way review system: Clients and Freelancers rate each other (1 to 5 Stars) upon job completion.
* Aggregated average star ratings dynamically displayed on user public profiles.

### 8. Admin Dashboard
* Centralized analytics and system statistics (Total Users, Active Jobs, Total Applications, Completed Projects).
* User management capabilities (Suspend/Activate accounts, modify roles).
* Content moderation (Approve/Reject job listings, flag inappropriate applications).

---

## 📸 Application

### User Authentication & Dashboards

| User Registration / Login | Client Dashboard | Freelancer Dashboard |
| :---: | :---: | :---: |
| <img src="Freelify\docs\images\auth-screen.png" width="250" alt="Auth Screen"/> | <img src="Freelify\docs\images\client-jobs.png" width="250" alt="Client Dashboard"/> | <img src="Freelify\docs\images\freelancer-profile.png" width="250" alt="Freelancer Dashboard"/> |

### Job Management & Search

| Job Feed & Filters | Job Details & Proposal Submission | Admin Analytics Dashboard |
| :---: | :---: | :---: |
| <img src="Freelify\docs\images\job-search.png" width="250" alt="Job Search"/> | <img src="Freelify\docs\images\job-apply.png" width="250" alt="Job Details"/> | <img src="Freelify\docs\images\admin.png" width="250" alt="Admin Dashboard"/> |

---

## 🏗️ Architecture & Project Structure

```text
FreelanceMarketplace/
├── Controllers/         # ASP.NET Core MVC Controllers (Job, Application, Account, Admin)
├── Models/
    ├── Entities/
    ├── ViewModels/
    ├── Enums/
├── Views/               # Razor Views (.cshtml) organized by feature modules
├── Services/            # Business Logic, Cloudinary Uploads, Email (SMTP), Notification Service
├── Hubs/                # SignalR WebSockets Hubs for real-time alerts
├── Data/                # EF Core DbContext and Configurations
└── wwwroot/             # Static Assets (CSS, JS, Libraries)
```

---

## ⚙️ Getting Started & Setup

### Prerequisites
* [.NET 10 SDK](https://dotnet.microsoft.com/download)
* [SQL Server](https://www.microsoft.com/en-us/sql-server/sql-server-downloads) or LocalDB
* Cloudinary Account (for media storage)
* SMTP Server / SendGrid API Credentials

### Installation Steps

1. **Clone the Repository:**
   ```bash
   git clone https://github.com/your-username/FreelanceMarketplace.git
   cd FreelanceMarketplace
   ```

2. **Configure App Settings:**
   Update `appsettings.json` with your database connection strings and API credentials:
   ```json
   {
     "ConnectionStrings": {
       "DefaultConnection": "Server=YOUR_SERVER;Database=FreelanceDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True;"
     }
   }
   ```

3. For security reasons, third-party API credentials (**Cloudinary** and **SMTP Email Settings**) are stored using .NET's **Secret Manager** (`dotnet user-secrets`) and are **not** pushed to the repository.


4. **Apply Database Migrations:**
   ```bash
   Update-Database
   ```

5. **Run the Application:**
   
   Open your browser and navigate to `https://localhost:7111`

---
