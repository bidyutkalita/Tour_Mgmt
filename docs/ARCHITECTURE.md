# Architecture Documentation

## Clean Architecture Overview

```
┌─────────────────────────────────────────────────────────┐
│                    Web Layer                             │
│              (TourManagement.Web)                        │
│         Razor Pages, ViewModels, Program.cs              │
├─────────────────────────────────────────────────────────┤
│                 Application Layer                        │
│           (TourManagement.Application)                   │
│          Services, DTOs, AutoMapper, Validators          │
├─────────────────────────────────────────────────────────┤
│               Infrastructure Layer                       │
│          (TourManagement.Infrastructure)                 │
│        EF Core DbContext, Repositories, Migrations       │
├─────────────────────────────────────────────────────────┤
│                  Domain Layer                            │
│             (TourManagement.Domain)                      │
│         Entities, Interfaces, Exceptions, Enums          │
└─────────────────────────────────────────────────────────┘
```

## Layer Responsibilities

### Domain Layer
- Pure C# classes with no external dependencies
- Entities: `UserInfo`, `Tour`, `Booking`
- Repository interfaces: `ITourRepository`, `IUserRepository`, `IBookingRepository`
- Service interfaces: `ITourService`, `IUserService`, `IBookingService`
- Custom exceptions: `NotFoundException`

### Application Layer
- Implements business logic via services
- Uses AutoMapper for entity-to-DTO mapping
- DTOs: `TourDto`, `UserDto`, `BookingDto` (and Create/Update variants)
- Services: `TourService`, `UserService`, `BookingService`

### Infrastructure Layer
- EF Core `TourManagementDbContext`
- Repository implementations using EF Core
- Entity configurations (Fluent API)
- DI extension methods

### Web Layer
- ASP.NET Core 8 Razor Pages
- ViewModels (manually mapped from DTOs)
- Program.cs with middleware pipeline
- Static files (CSS, JS)
- Session-based authentication

## Dependency Flow

```
Web → Application → Domain ← Infrastructure
```

- Web depends on Application and Infrastructure (for DI setup)
- Application depends on Domain
- Infrastructure depends on Domain and Application
- Domain has no dependencies
