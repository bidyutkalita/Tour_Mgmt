# Tour Management System - .NET 8

A modern ASP.NET Core 8 web application for managing tours and bookings, migrated from ASP.NET Web Forms 4.7.2.

## Architecture

This application follows **Clean Architecture** principles with four layers:

- **Domain** (`src/TourManagement.Domain`) - Entities, interfaces, domain exceptions
- **Application** (`src/TourManagement.Application`) - Business logic, DTOs, AutoMapper, FluentValidation
- **Infrastructure** (`src/TourManagement.Infrastructure`) - EF Core, repositories, data access
- **Web** (`src/TourManagement.Web`) - Razor Pages, ViewModels, static files

## Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB or full instance)

## Setup

1. Clone the repository
2. Update the connection string in `src/TourManagement.Web/appsettings.json`
3. Run the application - the database will be created automatically on first run

```bash
cd src/TourManagement.Web
dotnet run
```

## Building

```bash
dotnet build TourManagement.sln
```

## Testing

```bash
dotnet test TourManagement.sln
```

## Features

- **Tours**: Browse, search, create, edit, delete tours (admin)
- **Users**: Register, login, view/edit profile
- **Bookings**: Book tours, view my bookings, admin view all bookings
- **Admin**: Dashboard with statistics, manage users and tours

## Default Admin Credentials

- Email: `admin@gmail.com`
- Password: `admin`

## Migration Notes

See `docs/MIGRATION_NOTES.md` for details on what was migrated from Web Forms.
