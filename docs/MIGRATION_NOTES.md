# Migration Notes

## What Was Migrated

### Pages Migrated (Web Forms → Razor Pages)

| Web Forms Page | Razor Page | Description |
|----------------|------------|-------------|
| `Default.aspx` | `Pages/Index.cshtml` | Home page with featured tours |
| `AddTour.aspx` | `Pages/Tours/Create.cshtml` | Add new tour with file upload |
| `DisplayTours.aspx` | `Pages/Tours/Index.cshtml` | Browse all tours |
| `TourCrud.aspx` | `Pages/Tours/Index.cshtml` + Edit/Delete | Tour management |
| `Order.aspx` | `Pages/Bookings/Create.cshtml` | Book a tour |
| `mybooking.aspx` | `Pages/Bookings/MyBookings.cshtml` | View my bookings |
| `allbooking.aspx` | `Pages/Admin/Bookings.cshtml` | Admin: all bookings |
| `SignUpForm.aspx` | `Pages/Users/Register.cshtml` | User registration |
| `userlogin.aspx` | `Pages/Users/Login.cshtml` | User login |
| `AdminLogin2.aspx` | Role-based auth | Admin authentication |
| `AdminProfile.aspx` | `Pages/Admin/Dashboard.cshtml` | Admin dashboard |
| `MainProfilePage.aspx` | `Pages/Users/Profile.cshtml` | User profile |
| `usercrud.aspx` | `Pages/Admin/Users.cshtml` | Admin: user management |

## Key Differences from Web Forms

1. **No ViewState**: State is managed via TempData, session, or form fields
2. **No Code-Behind**: Logic is in PageModel classes with proper separation
3. **Dependency Injection**: All services injected via constructor
4. **Async/Await**: All I/O operations are async
5. **Tag Helpers**: Replace server controls (`asp-for`, `asp-page`, etc.)
6. **Model Binding**: `[BindProperty]` replaces form control access
7. **Validation**: DataAnnotations + client-side validation

## Breaking Changes

1. **Password Storage**: Passwords are now BCrypt hashed (old plain-text passwords won't work)
2. **Connection String**: Updated format for EF Core
3. **File Upload**: Uses `IFormFile` instead of `FileUpload` server control
4. **Authentication**: Cookie-based instead of Forms Authentication

## Configuration Changes

| Web.config | appsettings.json |
|------------|------------------|
| `<connectionStrings>` | `"ConnectionStrings"` section |
| `<appSettings>` | `"AppSettings"` section |
| `<system.web>` | Middleware in `Program.cs` |
| `<compilation debug="true">` | `ASPNETCORE_ENVIRONMENT=Development` |

## Security Improvements

1. BCrypt password hashing (was plain-text)
2. CSRF protection built-in with Razor Pages
3. Parameterized queries via EF Core (was string concatenation - SQL injection risk)
4. Cookie security settings (HttpOnly, SameSite)
5. Input validation with DataAnnotations

## Known Issues

1. Admin role assignment requires manual database setup
2. Image files from old `Tour_pics/` folder need to be copied to `wwwroot/Tour_pics/`
3. Database migration required before first run
