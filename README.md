# Tour Management System - .NET 8

A modern Tour Management web application migrated from ASP.NET Web Forms 4.7.2 to .NET 8 using clean architecture principles.

## Architecture

This application follows Clean Architecture with four layers:

- **Domain** (`src/TourManagement.Domain`) - Entities, repository interfaces, domain exceptions
- **Application** (`src/TourManagement.Application`) - Services, DTOs, AutoMapper profiles, service interfaces
- **Infrastructure** (`src/TourManagement.Infrastructure`) - EF Core DbContext, repository implementations, configurations
- **Web** (`src/TourManagement.Web`) - Razor Pages, ViewModels, static files

## Features

- User registration and login
- Admin login and dashboard
- Tour management (CRUD)
- Tour booking system
- User profile management
- File upload for tour images

## Setup Instructions

1. **Prerequisites**: .NET 8 SDK, SQL Server (or LocalDB)

2. **Configure connection string** in `src/TourManagement.Web/appsettings.json`:
   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=TourManagementDb;..."
   }
   ```

3. **Run database migrations**:
   ```bash
   cd src/TourManagement.Web
   dotnet ef database update --project ../TourManagement.Infrastructure
   ```

4. **Run the application**:
   ```bash
   dotnet run --project src/TourManagement.Web
   ```

## Default Admin Credentials

- Email: `admin@gmail.com`
- Password: `admin`

## Build

```bash
dotnet build TourManagement.sln
```

## Tests

```bash
dotnet test TourManagement.sln
```
