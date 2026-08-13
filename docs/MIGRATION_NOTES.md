# Migration Notes

## What Was Migrated

### From ASP.NET Web Forms 4.7.2 to .NET 8 ASP.NET Core Razor Pages

### Pages Migrated

| Web Forms Page | Razor Page | Notes |
|---|---|---|
| userlogin.aspx | Pages/User/Login.cshtml | Session-based auth |
| SignUpForm.aspx | Pages/User/Register.cshtml | Full validation |
| MainProfilePage.aspx | Pages/User/Profile.cshtml | Session-protected |
| AdminLogin2.aspx | Pages/Admin/Login.cshtml | Config-based credentials |
| AdminProfile.aspx | Pages/Admin/Dashboard.cshtml | Stats dashboard |
| DisplayTours.aspx | Pages/Tour/Index.cshtml | Search + CRUD |
| AddTour.aspx | Pages/Tour/Create.cshtml | File upload |
| TourCrud.aspx | Pages/Tour/Index.cshtml + Edit/Delete | Full CRUD |
| Order.aspx | Pages/Booking/Create.cshtml | Booking form |
| mybooking.aspx | Pages/Booking/MyBookings.cshtml | User bookings |
| allbooking.aspx | Pages/Booking/Index.cshtml | Admin view |
| usercrud.aspx | Pages/User/Index.cshtml | Admin user mgmt |

### Key Differences from Web Forms

1. **No ViewState** - State managed via session and TempData
2. **No Code-Behind** - Page models with clean separation
3. **No Server Controls** - HTML Tag Helpers
4. **No Web.config** - appsettings.json
5. **No Global.asax** - Program.cs with middleware pipeline
6. **No ADO.NET** - Entity Framework Core 8
7. **No System.Web** - ASP.NET Core equivalents

### Breaking Changes

- Connection string format updated for EF Core
- Password storage: plain text preserved for compatibility (recommend hashing in production)
- Session handling: uses ASP.NET Core distributed memory cache

### Configuration Changes

| Web.config | appsettings.json |
|---|---|
| `<connectionStrings>` | `ConnectionStrings` section |
| `<appSettings>` | Top-level JSON keys |
| `<compilation debug="true">` | Environment-based (Development) |

### Known Issues

1. Password hashing not implemented (plain text for migration compatibility)
2. File upload path changed from `~/Tour_pics/` to `~/images/`
3. Admin credentials stored in appsettings.json (use secrets in production)

### Future Improvements

1. Implement proper password hashing (BCrypt)
2. Add ASP.NET Core Identity for full auth
3. Add pagination for tour listings
4. Implement caching for tour data
5. Add email notifications for bookings
