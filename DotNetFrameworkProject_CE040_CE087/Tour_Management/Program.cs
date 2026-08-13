using Microsoft.EntityFrameworkCore;
using Tour_Management.Data;

// ASP.NET Core entry point replacing ASP.NET Web Forms Global.asax and Web.config.
// Configuration is loaded from appsettings.json and environment variables at runtime,
// replacing Web.config build-time transformations (cr-dotnet-0010).
var builder = WebApplication.CreateBuilder(args);

// -----------------------------------------------------------------------
// Configuration: environment-based, runtime-configurable (cr-dotnet-0010)
// Connection string is read from environment variable CONNECTIONSTRINGS__DBCONNECTION
// or from appsettings.{Environment}.json, enabling runtime configuration
// without rebuilding the application.
// -----------------------------------------------------------------------
var connectionString = builder.Configuration.GetConnectionString("dbconnection")
    ?? throw new InvalidOperationException(
        "Connection string 'dbconnection' not found. " +
        "Set the CONNECTIONSTRINGS__DBCONNECTION environment variable.");

// -----------------------------------------------------------------------
// Entity Framework Core with Azure SQL connection resiliency (cr-dotnet-0013)
// Replaces direct SqlConnection management with EF Core, enabling:
//   - Built-in connection pooling
//   - Retry logic for transient Azure SQL faults
//   - Azure SQL Database integration
// -----------------------------------------------------------------------
builder.Services.AddDbContext<TourManagementDbContext>(options =>
    options.UseSqlServer(connectionString, sqlOptions =>
    {
        // Enable retry on failure for Azure SQL transient fault handling
        sqlOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null);
    }));

// -----------------------------------------------------------------------
// ASP.NET Core Razor Pages replacing ASP.NET Web Forms (cr-dotnet-0026)
// Razor Pages provide a similar page-based model while eliminating
// ViewState, postbacks, and server affinity requirements, enabling
// stateless horizontal scaling.
// -----------------------------------------------------------------------
builder.Services.AddRazorPages();

// Structured logging for cloud monitoring
builder.Logging.ClearProviders();
builder.Logging.AddConsole();
builder.Logging.AddJsonConsole(options =>
{
    options.IncludeScopes = true;
    options.TimestampFormat = "yyyy-MM-ddTHH:mm:ssZ";
});

// Session support (stateless-compatible with distributed cache)
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(30);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

// -----------------------------------------------------------------------
// HTTP request pipeline
// -----------------------------------------------------------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseSession();
app.UseAuthorization();

app.MapRazorPages();

// Default route: redirect root to user login page
app.MapGet("/", context =>
{
    context.Response.Redirect("/User/Login");
    return Task.CompletedTask;
});

app.Run();
