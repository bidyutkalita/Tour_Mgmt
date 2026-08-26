# Migration Notes

## What Was Migrated

### From ASP.NET Web Forms 4.7.2 to .NET 8 ASP.NET Core

| Web Forms Component | .NET 8 Equivalent |
|---------------------|-------------------|
| `userlogin.aspx` | `Pages/Users/Login.cshtml` |
| `SignUpForm.aspx` | `Pages/Users/Register.cshtml` |
| `MainProfilePage.aspx` | `Pages/Users/Profile.cshtml` |
| `usercrud.aspx` | `Pages/Admin/Users.cshtml` |
| `AdminLogin2.aspx` | `Pages/Admin/Login.cshtml` |
| `AdminProfile.aspx` | `Pages/Admin/Dashboard.cshtml` |
| `AddTour.aspx` | `Pages/Tours/Create.cshtml` |
| `TourCrud.aspx` | `Pages/Tours/Index.cshtml` |
| `DisplayTours.aspx` | `Pages/Tours/Index.cshtml` |
| `Order.aspx` | `Pages/Bookings/Create.cshtml` |
| `mybooking.aspx` | `Pages/Bookings/MyBookings.cshtml` |
| `allbooking.aspx` | `Pages/Bookings/AllBookings.cshtml` |
| `Web.config` | `appsettings.json` |
| `Global.asax` | `Program.cs` |

## Key Differences from Web Forms

1. **No ViewState** - State managed via session and TempData
2. **No Code-Behind** - Page models with proper separation of concerns
3. **Dependency Injection** - All services injected via constructor
4. **Async/Await** - All I/O operations are async
5. **EF Core** - Replaced ADO.NET with Entity Framework Core 8
6. **BCrypt** - Passwords are now hashed (previously stored in plain text)
7. **FluentValidation** - Proper validation instead of inline checks
8. **AutoMapper** - Clean DTO mapping

## Breaking Changes

- Password hashing: Existing plain-text passwords will not work. Users must re-register.
- Database schema: `BookingDate` column added to `booking` table
- Password column in `UserInfo` extended to 100 chars for BCrypt hash

## Configuration Changes

- Connection string moved from `Web.config` to `appsettings.json`
- Admin credentials configurable in `appsettings.json` under `AdminCredentials`

## Security Improvements

- Passwords now hashed with BCrypt
- CSRF protection enabled by default in Razor Pages
- Session-based authentication
- Input validation with FluentValidation and DataAnnotations
