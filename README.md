# ServiceHub

ASP.NET Core 8 MVC web app jahan customers local services (electrician, plumber, cleaning, tutoring, etc.) browse aur book kar sakte hain. Service providers apni services list karte hain aur admin sab manage karta hai.

## Features
- Role-based login: **Admin**, **Service Provider**, **Customer** (ASP.NET Core Identity)
- Service browse + details, category-wise
- Booking create, My Bookings, Reviews
- Provider dashboard + calendar, service create
- Admin dashboard: users, categories, bookings, reviews
- Email notifications (MailKit)
- Indian cities list

## Tech Stack
- ASP.NET Core 8 MVC, C#
- Entity Framework Core 8 (SQL Server) + ADO.NET repository
- ASP.NET Core Identity
- MailKit / MimeKit
- Bootstrap, Font Awesome

## Setup
1. Requirements: .NET 8 SDK, SQL Server Express, Visual Studio 2022+ (ya `dotnet` CLI)
2. Clone karo:
   ```bash
   git clone https://github.com/<your-username>/ServiceHub.git
   cd ServiceHub
   ```
3. `appsettings.json` me connection string apne system ke hisaab se check karo.
4. Email testing ke liye local SMTP (e.g. smtp4dev / MailHog) port `1025` par chalao.
5. Run:
   ```bash
   dotnet restore
   dotnet run
   ```
   App start hone par database migrate aur seed data apne aap ban jata hai.

## Demo Accounts (sirf development/testing ke liye)
| Role | Email | Password |
|------|-------|----------|
| Admin | admin@servicehub.com | Admin@123! |

> Production me ye passwords zaroor change karo.

## Project Structure
```
Controllers/   Account, Admin, Bookings, Home, Provider, Services
Data/          DbContext, DbInitializer, ADO.NET repository
Models/        Booking, Category, Review, Service, ApplicationUser
Services/      EmailService
Views/         Razor views
Migrations/    EF Core migrations
```

## License
MIT (ya jo aap chahein)
