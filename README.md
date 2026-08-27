# Tour Management System - .NET 8

A modern Tour Management web application built with ASP.NET Core 8 Razor Pages, following Clean Architecture principles.

## Architecture

This solution follows Clean Architecture with four layers:

```
TourManagement/
├── src/
│   ├── TourManagement.Domain/          # Domain entities, interfaces, exceptions
│   ├── TourManagement.Application/     # Business logic, services, DTOs, AutoMapper
│   ├── TourManagement.Infrastructure/  # EF Core, repositories, data access
│   └── TourManagement.Web/             # Razor Pages UI, ViewModels, Program.cs
├── tests/
│   ├── TourManagement.UnitTests/       # xUnit unit tests with Moq
│   └── TourManagement.IntegrationTests/ # Integration tests with InMemory DB
└── docs/                               # Documentation
```

## Features

- **Tour Management**: Full CRUD for tour packages with image upload
- **User Registration & Login**: Cookie-based authentication with BCrypt password hashing
- **Booking System**: Users can book tours and view their bookings
- **Admin Dashboard**: Admin panel for managing tours, users, and bookings
- **Search**: Search tours by name or destination
- **Responsive UI**: Bootstrap 5 with Bootstrap Icons

## Technology Stack

- **Framework**: ASP.NET Core 8 Razor Pages
- **ORM**: Entity Framework Core 8 with SQL Server
- **Authentication**: Cookie Authentication
- **Mapping**: AutoMapper 12
- **Logging**: Serilog
- **Password Hashing**: BCrypt.Net
- **Testing**: xUnit, Moq, FluentAssertions

## Setup Instructions

### Prerequisites
- .NET 8 SDK
- SQL Server (LocalDB or full instance)

### Database Setup
1. Update the connection string in `src/TourManagement.Web/appsettings.json`
2. Run EF Core migrations:
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

## Migration Notes

This application was migrated from ASP.NET Web Forms 4.7.2 to .NET 8:

| Old (Web Forms) | New (.NET 8) |
|-----------------|--------------|
| `.aspx` pages | Razor Pages (`.cshtml`) |
| Code-behind files | Page Models (`.cshtml.cs`) |
| `Web.config` | `appsettings.json` |
| `Global.asax` | `Program.cs` |
| ADO.NET SqlConnection | EF Core 8 |
| Forms Authentication | Cookie Authentication |
| `System.Web` | `Microsoft.AspNetCore` |
| `packages.config` | SDK-style `.csproj` |

## Default Admin Login

To access admin features, configure an admin user in the database with the `Admin` role.

## Build Verification

```bash
dotnet build TourManagement.sln
dotnet test TourManagement.sln
```
