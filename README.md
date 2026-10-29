# Tour Management System - .NET 8 Migration

## Overview
This is a Tour Management System migrated from ASP.NET Web Forms (.NET 4.7.2) to .NET 8 using clean architecture principles.

## Architecture
The solution follows Clean Architecture with four layers:

```
TourManagement/
├── src/
│   ├── TourManagement.Domain/          # Entities, interfaces, exceptions
│   ├── TourManagement.Application/     # Services, DTOs, mappings
│   ├── TourManagement.Infrastructure/  # EF Core, repositories
│   └── TourManagement.Web/             # Razor Pages, ViewModels
├── tests/
│   ├── TourManagement.UnitTests/       # Service unit tests
│   └── TourManagement.IntegrationTests/ # Repository integration tests
└── docs/                               # Documentation
```

## Features
- **Tour Management**: Full CRUD for tours with image upload
- **User Registration & Login**: Cookie-based authentication with BCrypt password hashing
- **Booking System**: Users can book tours and view their bookings
- **Admin Panel**: Admin dashboard with tour/user/booking management

## Setup

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)

### Database Setup
1. Update the connection string in `src/TourManagement.Web/appsettings.json`
2. Run migrations: `dotnet ef database update --project src/TourManagement.Infrastructure --startup-project src/TourManagement.Web`

### Running the Application
```bash
cd src/TourManagement.Web
dotnet run
```

### Running Tests
```bash
dotnet test
```

## Migration Notes
- Migrated from ASP.NET Web Forms to Razor Pages
- Replaced ADO.NET with Entity Framework Core 8.0
- Replaced Forms Authentication with Cookie Authentication
- Replaced Web.config with appsettings.json
- Replaced Global.asax with Program.cs
- Added BCrypt password hashing (legacy plain-text passwords supported via fallback)
- Added proper async/await patterns throughout
- Added dependency injection for all services

## Default Admin Credentials
- Email: admin@gmail.com
- Password: admin

> **Note**: Change admin credentials in `appsettings.json` before production deployment.
