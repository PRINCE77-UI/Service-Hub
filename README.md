<div align="center">

# 🛠️ ServiceHub

**A Local Services Marketplace Where Customers Discover, Book and Review Trusted Professionals.**

![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?logo=dotnet&logoColor=white)
![C#](https://img.shields.io/badge/C%23-239120?logo=csharp&logoColor=white)
![ASP.NET Core MVC](https://img.shields.io/badge/ASP.NET%20Core-MVC-5C2D91)
![SQL Server](https://img.shields.io/badge/SQL%20Server-CC2927?logo=microsoftsqlserver&logoColor=white)
![Bootstrap](https://img.shields.io/badge/Bootstrap-5.3-7952B3?logo=bootstrap&logoColor=white)
![Entity Framework](https://img.shields.io/badge/Entity%20Framework-Core-68217A)

</div>

---

## 📖 Overview

ServiceHub is a local services marketplace developed using **ASP.NET Core 8 MVC**. It connects customers with local service providers such as electricians, plumbers, cleaners, tutors, painters and many other professionals.

Customers can browse services by category and city, view service details, check pricing, book appointments and leave reviews. Service providers can manage their service listings and booking requests, while administrators oversee the entire platform.

The main purpose of ServiceHub is to make finding and booking reliable local services simple, convenient and efficient.

## 🎯 Project Objectives

- Provide a centralized platform for discovering local services.
- Simplify the service booking process.
- Help service providers manage their services online.
- Improve transparency through customer ratings and reviews.
- Provide secure authentication and role-based access.
- Offer administrators complete platform management.
- Create a responsive and user-friendly marketplace.

## ✨ Features

### 👤 Customer

- Register and log in securely.
- Browse services by category and city.
- View service details, pricing and customer reviews.
- Book services with preferred dates and time slots.
- Add additional booking notes.
- Track booking status.
- Cancel bookings.
- Rate and review completed services.

### 🧰 Service Provider

- Dedicated provider dashboard.
- View booking statistics and requests.
- Create and manage service listings.
- Update service information and pricing.
- Accept or reject booking requests.
- View scheduled bookings.
- Manage service availability.

### 🛡️ Admin

- Dedicated administration dashboard.
- View platform statistics.
- Manage registered users.
- Manage service categories.
- Monitor bookings.
- Manage customer reviews.
- Oversee platform activities.

### ⚙️ General Features

- Role-based authorization (Admin, ServiceProvider, Customer).
- ASP.NET Core Identity authentication.
- Email notifications using MailKit and SMTP.
- Automatic database migration and seed data.
- Support for 30+ Indian cities.
- Responsive user interface.
- Bootstrap 5 styling.
- Entity Framework Core database integration.

## 🔄 How ServiceHub Works

1. **Registration:** Customers and service providers create accounts.
2. **Service Discovery:** Customers browse services based on category and city.
3. **Service Selection:** Customers check service descriptions, pricing and reviews.
4. **Booking:** Customers select a suitable date and time slot.
5. **Provider Response:** Providers accept or reject booking requests.
6. **Service Completion:** The service is delivered to the customer.
7. **Feedback:** Customers submit ratings and reviews after completion.

## 🌟 Key Highlights

- Three different user roles with separate functionalities.
- City-based service discovery.
- Complete booking management system.
- Booking status tracking.
- Customer review and rating functionality.
- Email notification support.
- Automatic database initialization.
- Responsive and modern user interface.
- Centralized administration system.

## 🔐 Security Features

- ASP.NET Core Identity for authentication.
- Role-based access control.
- Secure password hashing.
- Protected application routes.
- Server-side input validation.
- User-specific access to booking information.
- Secure configuration using User Secrets and environment variables.

## 🧱 Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 MVC |
| Language | C# |
| Database | SQL Server (Express / LocalDB) |
| ORM / Data Access | Entity Framework Core 8 + ADO.NET Repository |
| Authentication | ASP.NET Core Identity |
| Email | MailKit / MimeKit |
| Frontend | Razor Views |
| CSS Framework | Bootstrap 5.3 |
| Icons | Font Awesome 6 |
| Development Tools | Visual Studio 2022 / VS Code |
| Version Control | Git and GitHub |

## 🗄️ Database Modules

The application uses SQL Server for storing and managing application data.

| Module | Description |
|---|---|
| ApplicationUsers | Stores user information and roles |
| Categories | Stores service categories |
| Services | Stores service details and pricing |
| Bookings | Manages customer booking requests |
| Reviews | Stores customer ratings and feedback |

**Database Operations:**
- Create, read, update and delete records.
- Entity Framework Core migrations.
- Database initialization and seed data.
- Repository-based data access where implemented.
- Relationships between application entities.

## 📁 Project Structure

```text
ServiceHub/
│
├── Controllers/
│   ├── AccountController.cs
│   ├── AdminController.cs
│   ├── BookingsController.cs
│   ├── HomeController.cs
│   ├── ProviderController.cs
│   └── ServicesController.cs
│
├── Data/
│   ├── ApplicationDbContext.cs
│   ├── DbInitializer.cs
│   └── Repository/
│
├── Migrations/
│
├── Models/
│   ├── Booking.cs
│   ├── Category.cs
│   ├── Service.cs
│   ├── Review.cs
│   ├── ApplicationUser.cs
│   └── ViewModels/
│
├── Services/
│   └── EmailService.cs
│
├── Views/
│   ├── Account/
│   ├── Admin/
│   ├── Bookings/
│   ├── Home/
│   ├── Provider/
│   ├── Services/
│   └── Shared/
│
├── wwwroot/
│   ├── css/
│   ├── js/
│   └── images/
│
├── Program.cs
├── appsettings.json
└── ServiceHub.csproj
```

## 📸 Screenshots

Add screenshots of your actual application to the `screenshots` folder.

### 🏠 Home Page

![Home Page](screenshots/home.png)

### 👤 Customer Dashboard

![Customer Dashboard](screenshots/customer-dashboard.png)

### 🧰 Provider Dashboard

![Provider Dashboard](screenshots/provider-dashboard.png)

### 🛡️ Admin Dashboard

![Admin Dashboard](screenshots/admin-dashboard.png)

### 🛒 Service Listing

![Service Listing](screenshots/services.png)

## 🚀 Getting Started

### Prerequisites

Make sure you have installed:

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express or LocalDB
- Visual Studio 2022+ or VS Code
- Git
- Optional: SMTP testing tools such as [smtp4dev](https://github.com/rnwood/smtp4dev) or MailHog.

### Installation

**1. Clone the repository**

```bash
git clone https://github.com/PRINCE77-UI/ServiceHub.git
```

**2. Navigate to the project directory**

```bash
cd ServiceHub/ServiceHub
```

**3. Restore dependencies**

```bash
dotnet restore
```

**4. Build the application**

```bash
dotnet build
```

**5. Run the application**

```bash
dotnet run
```

The database is created, migrated and seeded automatically on first startup, according to the application's initialization configuration.

## ⚙️ Configuration

Update `appsettings.json` according to your local environment.

Configure the following:

- SQL Server connection string.
- SMTP server configuration.
- Email sender information.
- Application-specific settings.

> 🔒 Never commit real SMTP passwords or production connection strings. Use .NET User Secrets or environment variables.

### Application URLs

The application runs at:

```text
https://localhost:7184
http://localhost:5052
```

Actual ports may vary depending on your launch settings.

## 🧪 Testing

The following application modules can be tested:

| Test Case | Expected Result |
|---|---|
| Customer Registration | Customer account is created |
| User Login | User is authenticated |
| Role Authorization | Users access permitted sections only |
| Service Listing | Available services are displayed |
| Service Management | Providers manage their services |
| Booking Creation | Customer booking is saved |
| Booking Management | Providers update booking status |
| Review Submission | Customers submit service reviews |
| Admin Dashboard | Platform statistics are displayed |
| Email Notifications | Configured emails are sent |

## 🗺️ Roadmap

- [ ] Online payment gateway integration.
- [ ] Provider ratings and verification badges.
- [ ] Real-time booking notifications.
- [ ] Image upload for services.
- [ ] REST API development.
- [ ] Mobile application integration.
- [ ] Google Maps integration.
- [ ] Advanced search and filtering.
- [ ] Booking history and invoice generation.

## 🤝 Contributing

Contributions are welcome!

To contribute:

1. Fork the repository.
2. Create a feature branch.

```bash
git checkout -b feature/your-feature
```

3. Commit your changes.

```bash
git commit -m "Add your feature"
```

4. Push the branch.

```bash
git push origin feature/your-feature
```

5. Open a Pull Request.

## 📚 Learning Outcomes

Through developing ServiceHub, we explored:

- ASP.NET Core MVC architecture.
- C# programming and object-oriented concepts.
- Entity Framework Core and database operations.
- SQL Server integration.
- ASP.NET Core Identity authentication.
- Role-based authorization.
- CRUD operations.
- Repository pattern and dependency injection.
- Razor Views and Bootstrap.
- Email integration using SMTP.
- Git and GitHub collaboration.

## 🙏 Acknowledgements

We would like to thank the following technologies and platforms that helped us develop this project:

- Microsoft ASP.NET Core Documentation.
- Entity Framework Core Documentation.
- Bootstrap Documentation.
- SQL Server Documentation.
- Font Awesome.
- GitHub.

## 👥 Authors

**PRINCE KUMAR**
- GitHub: [PRINCE77-UI](https://github.com/PRINCE77-UI)

**RISHAV KUMAR**
- GitHub: [Rishav-kumar06](https://github.com/Rishav-kumar06)
- LinkedIn: [Rishav Kumar](https://www.linkedin.com/in/rishav-kumar-02196a340/)

## 📜 License

This project is developed for educational and learning purposes.

---

<div align="center">

**⭐ If you like ServiceHub, consider giving this repository a star!**

Made with ❤️ using ASP.NET Core MVC and C#.

</div>
