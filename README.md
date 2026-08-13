# Tour Management System - .NET 8

A modern Tour Management web application built with ASP.NET Core 8 Razor Pages, following Clean Architecture principles.

## Architecture

This solution follows Clean Architecture with four layers:

```
TourManagement/
├── src/
│   ├── TourManagement.Domain/          # Domain entities, interfaces
│   ├── TourManagement.Application/     # Business logic, services, DTOs
│   ├── TourManagement.Infrastructure/  # EF Core, repositories
│   └── TourManagement.Web/             # Razor Pages UI
├── tests/
│   ├── TourManagement.UnitTests/
│   └── TourManagement.IntegrationTests/
└── docs/
```

## Features

- **Tour Management**: Browse, search, add, edit, delete tours
- **User Registration & Login**: Secure user accounts
- **Booking System**: Book tours and view booking history
- **Admin Panel**: Manage tours, users, and bookings
- **Responsive UI**: Bootstrap 5 based responsive design

## Setup

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)

### Configuration
Update the connection string in `src/TourManagement.Web/appsettings.json`:
```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TourManagementDb;Trusted_Connection=True;"
  }
}
```

### Database Setup
```bash
cd src/TourManagement.Web
dotnet ef migrations add InitialCreate --project ../TourManagement.Infrastructure
dotnet ef database update --project ../TourManagement.Infrastructure
```

### Running the Application
```bash
cd src/TourManagement.Web
dotnet run
```

### Running Tests
```bash
dotnet test
```

## Admin Access
- Email: `admin@gmail.com`
- Password: `admin`

## Migration Notes
This application was migrated from ASP.NET Web Forms 4.7.2 to .NET 8.
See `docs/MIGRATION_NOTES.md` for details.
