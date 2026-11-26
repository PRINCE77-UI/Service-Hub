<div align="center">

# 🛠️ ServiceHub

**A local services marketplace where customers discover, book and review trusted professionals.**

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-5C2D91)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap&logoColor=white)

</div>

---

## 📖 Overview

ServiceHub connects **customers** with **local service providers** such as electricians, plumbers, cleaners, tutors, painters and more. Customers can browse services by category and city, book a time slot and leave reviews. Providers manage their own services and booking requests, while admins oversee the entire platform.

## ✨ Features

### 👤 Customer
- Register / login and browse services by category and city
- View service details, pricing and reviews
- Book a service with date, time slot and notes
- Track bookings (Pending, Accepted, Rejected, Completed, Cancelled)
- Rate and review completed services

### 🧰 Service Provider
- Dashboard with booking overview
- Create and manage service listings
- Accept or reject booking requests
- Calendar view of scheduled bookings

### 🛡️ Admin
- Platform dashboard with key statistics
- Manage users, categories, bookings and reviews

### ⚙️ General
- Role-based authorization (Admin, ServiceProvider, Customer) using ASP.NET Core Identity
- Email notifications via MailKit (SMTP)
- Auto database migration and seed data on first run
- Supports 30+ Indian cities
- Responsive UI built with Bootstrap 5

## 🧱 Tech Stack

| Layer | Technology |
|-------|------------|
| Framework | ASP.NET Core 8 MVC |
| Language | C# |
| Database | SQL Server (Express / LocalDB) |
| ORM / Data Access | Entity Framework Core 8 + ADO.NET repository |
| Authentication | ASP.NET Core Identity |
| Email | MailKit / MimeKit |
| Frontend | Razor Views, Bootstrap 5.3, Font Awesome 6 |

## 📁 Project Structure

```
ServiceHub/
├── Controllers/     # Account, Admin, Bookings, Home, Provider, Services
├── Data/            # DbContext, DbInitializer (seed), ADO.NET repository
├── Migrations/      # EF Core migrations
├── Models/          # Booking, Category, Service, Review, ApplicationUser, ViewModels
├── Services/        # EmailService
├── Views/           # Razor views (per controller + Shared layout)
├── wwwroot/         # CSS, JS, images
├── Program.cs       # App startup and DI configuration
└── appsettings.json # Configuration
```

## 🚀 Getting Started

### Prerequisites
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express (or LocalDB)
- Visual Studio 2022+ or VS Code
- *(Optional)* Local SMTP tool such as [smtp4dev](https://github.com/rnwood/smtp4dev) or MailHog for testing emails

### Installation

```bash
# 1. Clone the repository
git clone https://github.com/PRINCE77-UI/ServiceHub.git
cd ServiceHub/ServiceHub

# 2. Restore dependencies
dotnet restore

# 3. Run the application
dotnet run
```

The database is created, migrated and seeded automatically on first startup.

### Configuration

Update `appsettings.json` if needed:


> 🔒 Never commit real SMTP passwords or production connection strings. Use [User Secrets](https://learn.microsoft.com/aspnet/core/security/app-secrets) or environment variables.

The app runs at `https://localhost:7184` or `http://localhost:5052`.

## 🗺️ Roadmap

- [ ] Online payment integration
- [ ] Provider ratings and verification badges
- [ ] Real-time booking notifications
- [ ] Image upload for services
- [ ] REST API and mobile app

## 🤝 Contributing

1. Fork the repository
2. Create a feature branch: `git checkout -b feature/your-feature`
3. Commit your changes: `git commit -m "Add your feature"`
4. Push the branch: `git push origin feature/your-feature`
5. Open a Pull Request

## 👥 Authors

- **PRINCE KUMAR** – [GitHub](https://github.com/PRINCE77-UI)
- **RISHAV KUMAR** – [GitHub](https://github.com/Rishav-kumar06)

