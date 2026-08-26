# ASP.NET Web Forms to .NET 8 Migration Analysis Report
## Module: Tour_Management
**Analysis Date:** 2025-01-30  
**Current Framework:** ASP.NET Web Forms 4.7.2  
**Target Framework:** .NET 8  
**Migration Complexity:** Complex  

---

## Executive Summary

The **Tour_Management** application is a classic ASP.NET Web Forms application targeting .NET Framework 4.7.2. It consists of **11 Web Forms pages** with code-behind files, uses **raw ADO.NET** for data access with direct SQL string concatenation (SQL injection vulnerabilities), relies entirely on **System.Web** infrastructure, and stores configuration in **Web.config**. The application has **no master pages**, **no user controls**, and **no Global.asax**, but has extensive Web Forms-specific patterns that are incompatible with .NET 8.

| Severity | Count |
|----------|-------|
| Critical | 14    |
| High     | 10    |
| Medium   | 8     |
| Low      | 4     |
| **Total**| **36**|

**Estimated Remediation Effort:** 80–120 hours  
**Compatibility Score:** 12/100 (Very Low — full rewrite required)

---

## Project Inventory

### Web Forms Pages (11 total)
| Page | Code-Behind | Complexity | Description |
|------|-------------|------------|-------------|
| AddTour.aspx | AddTour.aspx.cs | Medium | Admin form to add new tour with file upload |
| AdminLogin2.aspx | AdminLogin2.aspx.cs | Simple | Hardcoded admin login |
| AdminProfile.aspx | AdminProfile.aspx.cs | Simple | Admin dashboard/navigation |
| allbooking.aspx | allbooking.aspx.cs | Medium | GridView of all bookings with Chart control |
| DisplayTours.aspx | DisplayTours.aspx.cs | Medium | GridView of tours with SqlDataSource |
| MainProfilePage.aspx | MainProfilePage.aspx.cs | Simple | User dashboard/navigation |
| mybooking.aspx | mybooking.aspx.cs | Medium | User's current bookings GridView |
| Order.aspx | Order.aspx.cs | Medium | Tour booking form |
| SignUpForm.aspx | SignUpForm.aspx.cs | Medium | User registration form |
| TourCrud.aspx | TourCrud.aspx.cs | Medium | Admin tour CRUD with GridView |
| usercrud.aspx | usercrud.aspx.cs | Medium | User profile GridView |
| userlogin.aspx | userlogin.aspx.cs | Medium | User login with SQL auth |

### User Controls: 0
### Master Pages: 0
### Global.asax: Not present
### packages.config: 1 package (Microsoft.CodeDom.Providers.DotNetCompilerPlatform 2.0.1)

---

## Detailed Issue Findings

---

### ISSUE-001 — [CRITICAL] System.Web Namespace Dependency (All Code-Behind Files)

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** High  

**Affected Files:**
- `AddTour.aspx.cs` — Lines 3–8
- `AdminLogin2.aspx.cs` — Lines 3–7
- `AdminProfile.aspx.cs` — Lines 3–7
- `allbooking.aspx.cs` — Lines 3–7
- `DisplayTours.aspx.cs` — Lines 3–8
- `MainProfilePage.aspx.cs` — Lines 3–7
- `mybooking.aspx.cs` — Lines 3–7
- `Order.aspx.cs` — Lines 3–8
- `SignUpForm.aspx.cs` — Lines 3–8
- `TourCrud.aspx.cs` — Lines 3–8
- `usercrud.aspx.cs` — Lines 3–7
- `userlogin.aspx.cs` — Lines 3–8

**Code Snippet (representative — AddTour.aspx.cs):**
```csharp
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
```

**Description:**  
Every code-behind file imports `System.Web`, `System.Web.UI`, and `System.Web.UI.WebControls`. These namespaces are part of the .NET Framework only and do not exist in .NET 8. The entire Web Forms page lifecycle (`System.Web.UI.Page`), server controls (`System.Web.UI.WebControls`), and HTTP infrastructure (`System.Web.HttpContext`) are unavailable in .NET 8.

**Recommendation:**  
Replace all `System.Web.UI.Page` base classes with Razor Pages (`PageModel`) or MVC Controllers. Replace `System.Web.UI.WebControls` server controls with HTML Tag Helpers or Razor syntax. Replace `System.Web.HttpContext` with `Microsoft.AspNetCore.Http.HttpContext` injected via `IHttpContextAccessor`.

---

### ISSUE-002 — [CRITICAL] Web Forms Page Inheritance (All Code-Behind Files)

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** High  

**Affected Files:** All 11 `.aspx.cs` files

**Code Snippet (AddTour.aspx.cs, Line 13):**
```csharp
public partial class AddTour : System.Web.UI.Page
```

**Description:**  
All code-behind classes inherit from `System.Web.UI.Page`, which is the core Web Forms page class. This class does not exist in .NET 8. The entire Web Forms page lifecycle (Init, Load, PreRender, Render, Unload) is a .NET Framework-only concept.

**Recommendation:**  
Migrate each page to a Razor Page (`PageModel`) or MVC Controller/View pair. The `Page_Load` event handler pattern must be replaced with `OnGet`/`OnPost` methods in Razor Pages or action methods in MVC controllers.

---

### ISSUE-003 — [CRITICAL] Page_Load Event Handler Pattern (Multiple Files)

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** High  

**Affected Files:**
- `AddTour.aspx.cs` — Line 15
- `AdminLogin2.aspx.cs` — Line 13
- `AdminProfile.aspx.cs` — Line 13
- `allbooking.aspx.cs` — Line 13
- `DisplayTours.aspx.cs` — Line 13
- `MainProfilePage.aspx.cs` — Line 13
- `mybooking.aspx.cs` — Line 13
- `Order.aspx.cs` — Line 15
- `SignUpForm.aspx.cs` — Line 15
- `TourCrud.aspx.cs` — Line 13
- `usercrud.aspx.cs` — Line 13
- `userlogin.aspx.cs` — Line 15

**Code Snippet (TourCrud.aspx.cs, Lines 13–18):**
```csharp
protected void Page_Load(object sender, EventArgs e)
{
    if (!Page.IsPostBack)
    {
        refreshdata();
    }
}
```

**Description:**  
The `Page_Load` event and `Page.IsPostBack` pattern are Web Forms-specific lifecycle concepts. `Page.IsPostBack` is used to distinguish between initial page load and form postback — a concept that does not exist in .NET 8 Razor Pages or MVC.

**Recommendation:**  
Replace `Page_Load` with `OnGet()` for initial page load logic in Razor Pages. Replace `Page.IsPostBack` checks with the HTTP method distinction (`OnGet` vs `OnPost`). In MVC, use separate `GET` and `POST` action methods.

---

### ISSUE-004 — [CRITICAL] Response.Redirect + Server.Transfer Anti-Pattern (Multiple Files)

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** Medium  

**Affected Files:**
- `AdminLogin2.aspx.cs` — Lines 17–18
- `Order.aspx.cs` — Lines 30–31
- `SignUpForm.aspx.cs` — Lines 37–38
- `userlogin.aspx.cs` — Lines 37–38, 47–48

**Code Snippet (Order.aspx.cs, Lines 29–32):**
```csharp
Response.Write("Registration Successful");
Response.Redirect("mybooking.aspx");
Server.Transfer("mybooking.aspx");
conn.Close();
```

**Description:**  
Multiple files call both `Response.Redirect()` and `Server.Transfer()` on the same code path, which is a logical error (only one will execute). Additionally, `Server.Transfer()` does not exist in ASP.NET Core. `Response.Write()` for inline HTML output is also a Web Forms pattern. `Response` and `Server` are `System.Web.HttpResponse` and `System.Web.HttpServerUtility` objects that do not exist in .NET 8.

**Recommendation:**  
Replace `Response.Redirect("page.aspx")` with `return RedirectToPage("/PageName")` in Razor Pages or `return Redirect("/path")` in MVC. Remove all `Server.Transfer()` calls — use `RedirectToPage` instead. Replace `Response.Write()` with proper model-based output in Razor views.

---

### ISSUE-005 — [CRITICAL] SQL Injection Vulnerability — Raw String Concatenation

**Category:** security  
**Breaking Change:** No (security issue, not migration blocker)  
**Effort:** Medium  

**Affected Files:**
- `userlogin.aspx.cs` — Lines 26–27

**Code Snippet (userlogin.aspx.cs, Lines 26–27):**
```csharp
string checkPasswordQuery = "select password from Userinfo where password='" 
    + txtPassword.Text + "' and email = '" + txtEmail.Text + "'";
SqlCommand passComm = new SqlCommand(checkPasswordQuery, conn);
```

**Description:**  
The user login query directly concatenates user input into a SQL string without parameterization. This is a critical SQL injection vulnerability. An attacker can bypass authentication or extract/destroy data. This must be fixed during migration.

**Recommendation:**  
Replace with parameterized queries using `SqlCommand.Parameters.AddWithValue()` (as done in other files), or migrate to Entity Framework Core with LINQ queries which are inherently parameterized. Example:
```csharp
string query = "SELECT password FROM Userinfo WHERE password=@password AND email=@email";
SqlCommand cmd = new SqlCommand(query, conn);
cmd.Parameters.AddWithValue("@password", txtPassword.Text);
cmd.Parameters.AddWithValue("@email", txtEmail.Text);
```

---

### ISSUE-006 — [CRITICAL] Hardcoded Admin Credentials

**Category:** security  
**Breaking Change:** No  
**Effort:** High  

**Affected Files:**
- `AdminLogin2.aspx.cs` — Lines 14–17

**Code Snippet (AdminLogin2.aspx.cs, Lines 14–17):**
```csharp
if (password.Text == "admin" && name.Text == "admin@gmail.com")
{
    Response.Redirect("AdminProfile.aspx");
    Server.Transfer("AdminProfile.aspx");
}
```

**Description:**  
Admin credentials are hardcoded directly in the source code (`password = "admin"`, `email = "admin@gmail.com"`). This is a critical security vulnerability. Additionally, the authentication check is in `Page_Load`, meaning it runs on every page load, not just on form submission.

**Recommendation:**  
Implement proper authentication using ASP.NET Core Identity. Store credentials securely (hashed passwords in database). Use `[Authorize]` attribute for admin-protected pages. Move authentication logic to a dedicated `OnPost` handler.

---

### ISSUE-007 — [CRITICAL] Plaintext Password Storage

**Category:** security  
**Breaking Change:** No  
**Effort:** High  

**Affected Files:**
- `SignUpForm.aspx.cs` — Line 30
- `usercrud.aspx` — Line 14 (Password column displayed in GridView)

**Code Snippet (SignUpForm.aspx.cs, Line 30):**
```csharp
com.Parameters.AddWithValue("@Password", password1.Text);
```

**Code Snippet (usercrud.aspx, Line 14):**
```xml
<asp:BoundField DataField="Password" HeaderText="Password" SortExpression="Password" />
```

**Description:**  
User passwords are stored in plaintext in the database and are even displayed in a GridView on the user profile page. This is a critical security vulnerability that violates all modern security standards.

**Recommendation:**  
Use ASP.NET Core Identity which handles password hashing automatically (PBKDF2 with HMAC-SHA256). Never store or display plaintext passwords. Remove the Password column from all GridViews.

---

### ISSUE-008 — [CRITICAL] System.Web.DataVisualization Chart Control

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** High  

**Affected Files:**
- `Web.config` — Lines 7–10, 14–17, 22–24, 28
- `allbooking.aspx` — Line 3

**Code Snippet (allbooking.aspx, Line 3):**
```xml
<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, 
    PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" 
    tagprefix="asp" %>
```

**Code Snippet (Web.config, Lines 7–10):**
```xml
<add name="ChartImageHandler" preCondition="integratedMode" verb="GET,HEAD,POST"
  path="ChartImg.axd" type="System.Web.UI.DataVisualization.Charting.ChartHttpHandler, 
  System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" />
```

**Description:**  
`System.Web.DataVisualization` is a .NET Framework-only assembly. The `ChartHttpHandler` is an HTTP handler registered in Web.config, which is also a .NET Framework-only concept. This assembly is not available in .NET 8.

**Recommendation:**  
Replace with a modern charting library compatible with .NET 8 such as:
- **Chart.js** (JavaScript, free) — render charts client-side
- **LiveCharts2** (NuGet: `LiveChartsCore.SkiaSharpView.AspNetCore`)
- **Syncfusion Charts** or **Telerik UI for ASP.NET Core**
Remove the `ChartImg.axd` HTTP handler registration from Web.config and migrate to `appsettings.json`.

---

### ISSUE-009 — [CRITICAL] Web.config Configuration File

**Category:** breaking-change / configuration  
**Breaking Change:** Yes  
**Effort:** Medium  

**Affected Files:**
- `Web.config` — All lines
- `Web.Debug.config`
- `Web.Release.config`

**Code Snippet (Web.config, Lines 33–35):**
```xml
<connectionStrings>
  <add name="dbconnection" connectionString="Data Source=(LocalDB)\MSSQLLocalDB;
    AttachDbFilename=C:\Users\gajer\source\repos\Tour_Management\Tour_Management\App_Data\tourdb.mdf;
    Integrated Security=True" providerName="System.Data.SqlClient"/>
</connectionStrings>
```

**Description:**  
`Web.config` is a .NET Framework-only configuration system. ASP.NET Core/.NET 8 uses `appsettings.json`. The connection string contains a hardcoded absolute path (`C:\Users\gajer\...`) which is environment-specific and will not work in any other environment. The `system.web`, `system.webServer`, `system.codedom` sections are all .NET Framework-only.

**Recommendation:**  
Create `appsettings.json` with the connection string. Use `IConfiguration` to access settings. Use relative paths or environment variables for database paths. Example:
```json
{
  "ConnectionStrings": {
    "dbconnection": "Server=(localdb)\\mssqllocaldb;Database=tourdb;Trusted_Connection=True;"
  }
}
```

---

### ISSUE-010 — [CRITICAL] SqlDataSource Server Control (Multiple .aspx Files)

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** High  

**Affected Files:**
- `DisplayTours.aspx` — Lines 14–15
- `TourCrud.aspx` — Lines 30–35
- `allbooking.aspx` — Line 30
- `mybooking.aspx` — Lines 28–29
- `usercrud.aspx` — Lines 20–25

**Code Snippet (TourCrud.aspx, Lines 30–35):**
```xml
<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
    ConnectionString="<%$ ConnectionStrings:dbconnection %>" 
    SelectCommand="SELECT * FROM [Tour]"
    UpdateCommand="UPDATE [Tour] Set [TOUR_NAME]=@TOUR_NAME,..."
    DeleteCommand="Delete from [Tour] Where [TOUR_ID]=@TOUR_ID">
</asp:SqlDataSource>
```

**Description:**  
`asp:SqlDataSource` is a Web Forms data source control that does not exist in .NET 8. It provides declarative data binding directly in markup, which is a Web Forms-only pattern. The `<%$ ConnectionStrings:dbconnection %>` expression syntax is also Web Forms-only.

**Recommendation:**  
Replace with Entity Framework Core or Dapper for data access. Create repository classes and service layer. Bind data in Razor Pages `OnGet()` methods and pass to the view via the PageModel. Use `<table>` with `@foreach` loops or a modern component library.

---

### ISSUE-011 — [CRITICAL] GridView Server Control (Multiple .aspx Files)

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** High  

**Affected Files:**
- `DisplayTours.aspx` — Lines 15–40
- `TourCrud.aspx` — Lines 8–29
- `allbooking.aspx` — Lines 12–29
- `mybooking.aspx` — Lines 8–27
- `usercrud.aspx` — Lines 8–19

**Code Snippet (DisplayTours.aspx, Lines 15–17):**
```xml
<asp:GridView ID="GridView1" runat="server" AutoGenerateColumns="False" 
    DataKeyNames="TOUR_ID" DataSourceID="SqlDataSource1" 
    Height="16px" Width="875px" BackColor="White" ...>
```

**Description:**  
`asp:GridView` is a Web Forms server control that does not exist in .NET 8. It provides server-side rendered HTML tables with built-in sorting, paging, editing, and deleting. The `runat="server"` attribute, `DataSourceID` binding, `BoundField`, `TemplateField`, and `AutoGenerateEditButton`/`AutoGenerateDeleteButton` are all Web Forms-only concepts.

**Recommendation:**  
Replace with HTML `<table>` elements rendered via Razor syntax with `@foreach` loops. For advanced grid functionality, consider:
- **Telerik Grid for ASP.NET Core**
- **DevExpress Grid**
- **AG Grid** (JavaScript)
- Custom Razor Pages with pagination and sorting logic

---

### ISSUE-012 — [CRITICAL] FileUpload Server Control and Server.MapPath

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** Medium  

**Affected Files:**
- `AddTour.aspx` — Line 57
- `AddTour.aspx.cs` — Lines 30–31

**Code Snippet (AddTour.aspx.cs, Lines 30–31):**
```csharp
FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + FileUpload1.FileName);
com.Parameters.AddWithValue("@pic", FileUpload1.FileName);
```

**Description:**  
`asp:FileUpload` is a Web Forms server control. `Server.MapPath()` is a `System.Web.HttpServerUtility` method that maps virtual paths to physical paths — it does not exist in .NET 8. The `~/` virtual path syntax is Web Forms-specific.

**Recommendation:**  
Replace `asp:FileUpload` with `<input type="file">` and `IFormFile` in ASP.NET Core. Replace `Server.MapPath("~/Tour_pics/")` with `IWebHostEnvironment.WebRootPath` or `ContentRootPath`. Example:
```csharp
// In Razor Page
public IFormFile TourImage { get; set; }
// In OnPost handler
var filePath = Path.Combine(_env.WebRootPath, "Tour_pics", TourImage.FileName);
using var stream = new FileStream(filePath, FileMode.Create);
await TourImage.CopyToAsync(stream);
```

---

### ISSUE-013 — [CRITICAL] Legacy .csproj Format (Non-SDK Style)

**Category:** breaking-change  
**Breaking Change:** Yes  
**Effort:** Medium  

**Affected Files:**
- `Tour_Management.csproj` — All lines

**Code Snippet (Tour_Management.csproj, Lines 1–3):**
```xml
<Project ToolsVersion="15.0" DefaultTargets="Build" 
    xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <Import Project="..\packages\Microsoft.CodeDom.Providers.DotNetCompilerPlatform.2.0.1\..."/>
```

**Description:**  
The project file uses the legacy MSBuild format (non-SDK style) with `ToolsVersion="15.0"` and explicit file listings. .NET 8 requires the SDK-style project format (`<Project Sdk="Microsoft.NET.Sdk.Web">`). The `ProjectTypeGuids` for Web Application (`{349c5851-65df-11da-9384-00065b846f21}`) are .NET Framework-specific. The `TargetFrameworkVersion` is `v4.7.2`.

**Recommendation:**  
Replace with SDK-style project file:
```xml
<Project Sdk="Microsoft.NET.Sdk.Web">
  <PropertyGroup>
    <TargetFramework>net8.0</TargetFramework>
    <Nullable>enable</Nullable>
    <ImplicitUsings>enable</ImplicitUsings>
  </PropertyGroup>
</Project>
```

---

### ISSUE-014 — [CRITICAL] Microsoft.CodeDom.Providers.DotNetCompilerPlatform Package

**Category:** package-compatibility  
**Breaking Change:** Yes  
**Effort:** Low  

**Affected Files:**
- `packages.config` — Line 3
- `Tour_Management.csproj` — Lines 47–50
- `Web.config` — Lines 41–47

**Code Snippet (packages.config, Line 3):**
```xml
<package id="Microsoft.CodeDom.Providers.DotNetCompilerPlatform" version="2.0.1" 
    targetFramework="net472" />
```

**Description:**  
`Microsoft.CodeDom.Providers.DotNetCompilerPlatform` is a .NET Framework-only package used for Roslyn compiler support in Web Forms. It is not needed in .NET 8 (Roslyn is built-in). The `packages.config` format itself is also deprecated in favor of `PackageReference` in SDK-style projects.

**Recommendation:**  
Remove this package entirely. Migrate from `packages.config` to `PackageReference` format in the SDK-style `.csproj` file. The Roslyn compiler is included by default in .NET 8 SDK.

---

### ISSUE-015 — [HIGH] ADO.NET Direct SQL — No ORM or Repository Pattern

**Category:** deprecated-api  
**Breaking Change:** No (works but not recommended)  
**Effort:** High  

**Affected Files:**
- `AddTour.aspx.cs` — Lines 20–38
- `Order.aspx.cs` — Lines 17–32
- `SignUpForm.aspx.cs` — Lines 20–38
- `TourCrud.aspx.cs` — Lines 20–30
- `userlogin.aspx.cs` — Lines 22–30

**Code Snippet (AddTour.aspx.cs, Lines 20–25):**
```csharp
SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
conn.Open();
string insertQuery = "insert into Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) 
    values(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";
SqlCommand com = new SqlCommand(insertQuery, conn);
```

**Description:**  
All data access uses raw ADO.NET with `SqlConnection`/`SqlCommand` directly in code-behind files. There is no separation of concerns, no repository pattern, no unit of work, and no ORM. Connections are not wrapped in `using` statements in all cases, risking connection leaks. `ConfigurationManager` is a `System.Configuration` class that requires the `System.Configuration.ConfigurationManager` NuGet package in .NET 8.

**Recommendation:**  
Migrate to Entity Framework Core 8.0.0 with a proper `DbContext`. Create repository interfaces and implementations. Use dependency injection to provide data access services. Wrap all connections in `using` statements or use EF Core's connection management. Replace `ConfigurationManager` with `IConfiguration`.

---

### ISSUE-016 — [HIGH] ConfigurationManager Usage

**Category:** deprecated-api  
**Breaking Change:** Yes  
**Effort:** Medium  

**Affected Files:**
- `AddTour.aspx.cs` — Line 20
- `DisplayTours.aspx.cs` — Line 3 (import)
- `Order.aspx.cs` — Line 17
- `SignUpForm.aspx.cs` — Line 20
- `TourCrud.aspx.cs` — Line 20
- `userlogin.aspx.cs` — Line 22

**Code Snippet (AddTour.aspx.cs, Line 20):**
```csharp
SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
```

**Description:**  
`System.Configuration.ConfigurationManager` reads from `Web.config`/`App.config`. In .NET 8, configuration is handled by `Microsoft.Extensions.Configuration` reading from `appsettings.json`. While `System.Configuration.ConfigurationManager` is available as a NuGet package for .NET 8, it is not the recommended approach and requires `Web.config` which is not supported in .NET 8 web applications.

**Recommendation:**  
Replace with `IConfiguration` injected via dependency injection:
```csharp
// In constructor
private readonly IConfiguration _configuration;
public MyService(IConfiguration configuration) => _configuration = configuration;

// Usage
var connStr = _configuration.GetConnectionString("dbconnection");
```

---

### ISSUE-017 — [HIGH] Page.IsPostBack Pattern

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** Medium  

**Affected Files:**
- `TourCrud.aspx.cs` — Line 16

**Code Snippet (TourCrud.aspx.cs, Lines 15–18):**
```csharp
if (!Page.IsPostBack)
{
    refreshdata();
}
```

**Description:**  
`Page.IsPostBack` is a Web Forms-specific property that indicates whether the page is being loaded in response to a client postback. This concept does not exist in Razor Pages or MVC — HTTP GET and POST are handled by separate methods.

**Recommendation:**  
In Razor Pages, move data loading logic to `OnGet()` method. In MVC, use separate `[HttpGet]` and `[HttpPost]` action methods. The `refreshdata()` call should be in `OnGet()`.

---

### ISSUE-018 — [HIGH] asp:Label, asp:TextBox, asp:Button Server Controls

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** High  

**Affected Files:**
- All `.aspx` files

**Code Snippet (AddTour.aspx, Lines 32–35):**
```xml
<asp:Label id="l1" runat="server" text="Name of Tour"/>
<asp:TextBox id="tour_name" required="true" ForeColor="Black" class="form-control" runat="server"/>
<asp:Button BackColor="#cc6600" ID="Register" runat="server" Text="Register" OnClick="Register_Click" />
```

**Description:**  
All form elements use ASP.NET Web Forms server controls (`asp:Label`, `asp:TextBox`, `asp:Button`, `asp:DropDownList`, `asp:HyperLink`, `asp:FileUpload`). These controls with `runat="server"` are Web Forms-only and do not exist in .NET 8. The `OnClick="Register_Click"` server-side event wiring is also Web Forms-specific.

**Recommendation:**  
Replace with standard HTML elements and Razor syntax:
- `asp:Label` → `<label>`
- `asp:TextBox` → `<input type="text" asp-for="PropertyName">`
- `asp:Button` → `<button type="submit">` or `<input type="submit">`
- `asp:DropDownList` → `<select asp-for="PropertyName" asp-items="...">`
- `asp:HyperLink` → `<a href="...">`
Use Tag Helpers (`asp-for`, `asp-action`, `asp-page`) for model binding.

---

### ISSUE-019 — [HIGH] RegularExpressionValidator Server Control

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** Low  

**Affected Files:**
- `AddTour.aspx` — Line 72

**Code Snippet (AddTour.aspx, Line 72):**
```xml
<asp:RegularExpressionValidator ID="RegularExpressionValidator1" Display="Dynamic" 
    ControlToValidate="tour_info" ValidationExpression="^[\s\S]{0,250}$" 
    runat="server" ErrorMessage="Characters less than 250">
</asp:RegularExpressionValidator>
```

**Description:**  
`asp:RegularExpressionValidator` is a Web Forms validation control that does not exist in .NET 8. Web Forms validation controls (RequiredFieldValidator, RangeValidator, etc.) are all .NET Framework-only.

**Recommendation:**  
Replace with Data Annotations on the model:
```csharp
[MaxLength(250, ErrorMessage = "Characters less than 250")]
public string TourInfo { get; set; }
```
Or use FluentValidation. Use `<span asp-validation-for="TourInfo">` Tag Helper for displaying validation messages.

---

### ISSUE-020 — [HIGH] <%@ Page %> Directive and AutoEventWireup

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** High  

**Affected Files:** All 11 `.aspx` files

**Code Snippet (AddTour.aspx, Line 1):**
```xml
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddTour.aspx.cs" 
    Inherits="Tour_Management.AddTour" %>
```

**Description:**  
The `<%@ Page %>` directive is the Web Forms page declaration syntax. `AutoEventWireup="true"` enables automatic event handler wiring (e.g., `Page_Load`). `CodeBehind` links the markup to the code-behind class. `Inherits` specifies the code-behind class. All of these are Web Forms-only concepts with no equivalent in .NET 8.

**Recommendation:**  
Replace `.aspx` files with `.cshtml` Razor Pages. The `@page` directive in Razor Pages replaces `<%@ Page %>`. The `@model` directive replaces `Inherits`. Code-behind is replaced by the `PageModel` class in a `.cshtml.cs` file.

---

### ISSUE-021 — [HIGH] <%@ Register %> Directive for Chart Assembly

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** Medium  

**Affected Files:**
- `allbooking.aspx` — Line 3

**Code Snippet (allbooking.aspx, Line 3):**
```xml
<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, 
    PublicKeyToken=31bf3856ad364e35" namespace="System.Web.UI.DataVisualization.Charting" 
    tagprefix="asp" %>
```

**Description:**  
The `<%@ Register %>` directive registers a third-party or framework assembly for use as server controls in the page. This is a Web Forms-only concept. The registered assembly `System.Web.DataVisualization` is .NET Framework-only.

**Recommendation:**  
Remove the `<%@ Register %>` directive. Replace the chart functionality with a JavaScript charting library (Chart.js, ApexCharts) or a .NET 8 compatible charting NuGet package.

---

### ISSUE-022 — [HIGH] <%# Eval() %> Data Binding Expressions

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** Medium  

**Affected Files:**
- `DisplayTours.aspx` — Line 26
- `TourCrud.aspx` — Line 22

**Code Snippet (DisplayTours.aspx, Line 26):**
```xml
<img src="Tour_pics/<%#Eval("pic") %>" style="width:200px;height:200px" />
```

**Description:**  
`<%# Eval("fieldName") %>` is a Web Forms data binding expression used within data-bound controls (GridView, Repeater, etc.). This syntax does not exist in .NET 8 Razor syntax.

**Recommendation:**  
Replace with Razor syntax in a `@foreach` loop:
```html
@foreach (var tour in Model.Tours)
{
    <img src="~/Tour_pics/@tour.Pic" style="width:200px;height:200px" />
}
```

---

### ISSUE-023 — [HIGH] <%$ ConnectionStrings:dbconnection %> Expression

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** Low  

**Affected Files:**
- `DisplayTours.aspx` — Line 14
- `TourCrud.aspx` — Line 30
- `allbooking.aspx` — Line 30
- `mybooking.aspx` — Line 28
- `usercrud.aspx` — Line 20

**Code Snippet (DisplayTours.aspx, Line 14):**
```xml
<asp:SqlDataSource ... ConnectionString="<%$ ConnectionStrings:dbconnection %>" ...>
```

**Description:**  
`<%$ ConnectionStrings:dbconnection %>` is a Web Forms expression builder that reads from `Web.config`. This syntax does not exist in .NET 8.

**Recommendation:**  
Use `IConfiguration.GetConnectionString("dbconnection")` in the service/repository layer. Connection strings should be in `appsettings.json`.

---

### ISSUE-024 — [MEDIUM] No Authentication/Authorization Middleware

**Category:** security  
**Breaking Change:** No  
**Effort:** High  

**Affected Files:**
- `AdminLogin2.aspx.cs` — Lines 13–18
- `userlogin.aspx.cs` — Lines 22–45
- All admin pages (AddTour, TourCrud, allbooking, AdminProfile)

**Description:**  
There is no authentication or authorization mechanism protecting admin pages. The admin login check is in `Page_Load` (runs on every load, not just POST). User login stores no session state after authentication. Any user can navigate directly to `AdminProfile.aspx`, `AddTour.aspx`, `TourCrud.aspx`, or `allbooking.aspx` without logging in.

**Recommendation:**  
Implement ASP.NET Core Identity for authentication. Use `[Authorize]` and `[Authorize(Roles = "Admin")]` attributes on protected pages. Configure cookie authentication in `Program.cs`. Store user identity in claims after successful login.

---

### ISSUE-025 — [MEDIUM] No Session State After Login

**Category:** webforms-migration  
**Breaking Change:** No  
**Effort:** Medium  

**Affected Files:**
- `userlogin.aspx.cs` — Lines 33–38 (commented-out session code)

**Code Snippet (userlogin.aspx.cs, Line 33):**
```csharp
//Session["New"] = txtEmail.Text;
```

**Description:**  
The session assignment after login is commented out, meaning no user identity is preserved after login. The application has no way to know which user is logged in on subsequent pages. This is a fundamental authentication gap.

**Recommendation:**  
In .NET 8, use ASP.NET Core Identity with cookie authentication. After successful login, call `SignInAsync()` to create an authentication cookie. Use `User.Identity.Name` or claims to identify the current user.

---

### ISSUE-026 — [MEDIUM] Hardcoded Absolute File Path in Connection String

**Category:** configuration  
**Breaking Change:** Yes  
**Effort:** Low  

**Affected Files:**
- `Web.config` — Line 34

**Code Snippet (Web.config, Line 34):**
```xml
connectionString="Data Source=(LocalDB)\MSSQLLocalDB;
AttachDbFilename=C:\Users\gajer\source\repos\Tour_Management\Tour_Management\App_Data\tourdb.mdf;
Integrated Security=True"
```

**Description:**  
The connection string contains a hardcoded absolute path to the developer's local machine (`C:\Users\gajer\...`). This will not work on any other machine or in any deployment environment.

**Recommendation:**  
Use `|DataDirectory|` substitution or a proper SQL Server connection string. In .NET 8, use environment-specific `appsettings.{Environment}.json` files or environment variables for connection strings.

---

### ISSUE-027 — [MEDIUM] No Async/Await Data Access

**Category:** deprecated-api  
**Breaking Change:** No  
**Effort:** Medium  

**Affected Files:**
- `AddTour.aspx.cs` — Lines 20–38
- `Order.aspx.cs` — Lines 17–32
- `SignUpForm.aspx.cs` — Lines 20–38
- `userlogin.aspx.cs` — Lines 22–30

**Description:**  
All database operations are synchronous (`conn.Open()`, `com.ExecuteNonQuery()`, `passComm.ExecuteScalar()`). In .NET 8, async/await is the standard pattern for I/O-bound operations. Synchronous database calls block threads and reduce scalability.

**Recommendation:**  
Use async ADO.NET methods (`OpenAsync()`, `ExecuteNonQueryAsync()`, `ExecuteScalarAsync()`) or Entity Framework Core async methods (`SaveChangesAsync()`, `ToListAsync()`, `FirstOrDefaultAsync()`). Make all page handlers `async Task`.

---

### ISSUE-028 — [MEDIUM] No Dependency Injection

**Category:** breaking-change  
**Breaking Change:** No  
**Effort:** High  

**Affected Files:** All code-behind files

**Description:**  
The application has no dependency injection. All dependencies (database connections, configuration) are created directly in code-behind methods. This makes the code untestable and tightly coupled. .NET 8 has a built-in DI container that should be used throughout.

**Recommendation:**  
Register services in `Program.cs` using `builder.Services`. Inject `IConfiguration`, `DbContext`, and service classes via constructor injection in Razor Page models and controllers.

---

### ISSUE-029 — [MEDIUM] No Error Handling

**Category:** breaking-change  
**Breaking Change:** No  
**Effort:** Medium  

**Affected Files:**
- `AddTour.aspx.cs` — Lines 20–38
- `Order.aspx.cs` — Lines 17–32
- `SignUpForm.aspx.cs` — Lines 20–38
- `userlogin.aspx.cs` — Lines 22–30

**Description:**  
No try-catch blocks exist around database operations. If a database operation fails, an unhandled exception will crash the page. Database connections may not be closed on error (no `using` statements in some cases).

**Recommendation:**  
Wrap all database operations in try-catch blocks. Use `using` statements for all `IDisposable` objects (connections, commands). Implement global exception handling middleware in .NET 8 using `app.UseExceptionHandler()`.

---

### ISSUE-030 — [MEDIUM] Designer Files (.aspx.designer.cs)

**Category:** webforms-migration  
**Breaking Change:** Yes  
**Effort:** Low  

**Affected Files:** All 11 `.aspx.designer.cs` files

**Description:**  
Designer files are auto-generated by Visual Studio for Web Forms pages. They declare the server control fields that correspond to controls in the `.aspx` markup. These files have no equivalent in .NET 8 Razor Pages.

**Recommendation:**  
Delete all `.aspx.designer.cs` files. In Razor Pages, model properties are declared directly in the `PageModel` class and bound using `[BindProperty]` attribute or `asp-for` Tag Helpers.

---

### ISSUE-031 — [MEDIUM] Properties/AssemblyInfo.cs

**Category:** breaking-change  
**Breaking Change:** No  
**Effort:** Low  

**Affected Files:**
- `Properties/AssemblyInfo.cs`

**Description:**  
In SDK-style .NET projects, `AssemblyInfo.cs` is auto-generated. Manually maintained `AssemblyInfo.cs` files can cause duplicate attribute errors when migrating to SDK-style projects.

**Recommendation:**  
Delete `Properties/AssemblyInfo.cs` or add `<GenerateAssemblyInfo>false</GenerateAssemblyInfo>` to the `.csproj` file. Assembly metadata can be specified in the `.csproj` `<PropertyGroup>`.

---

### ISSUE-032 — [LOW] App_Data Folder with .mdf Database File

**Category:** configuration  
**Breaking Change:** No  
**Effort:** Medium  

**Affected Files:**
- `App_Data/tourdb.mdf`
- `App_Data/tourdb_log.ldf`

**Description:**  
The application uses a LocalDB `.mdf` file stored in the `App_Data` folder. While this works for development, it is not suitable for production deployment. The `App_Data` folder is a Web Forms convention.

**Recommendation:**  
For development, use LocalDB with a proper connection string. For production, use a full SQL Server instance or Azure SQL. Consider using EF Core migrations to manage the database schema instead of a pre-existing `.mdf` file.

---

### ISSUE-033 — [LOW] No Input Validation Beyond RegularExpressionValidator

**Category:** security  
**Breaking Change:** No  
**Effort:** Medium  

**Affected Files:**
- `AddTour.aspx` — Only one validator for tour_info
- `SignUpForm.aspx` — No server-side validation
- `Order.aspx` — No server-side validation

**Description:**  
The application relies almost entirely on HTML5 `required` attributes for validation, with only one `RegularExpressionValidator` in `AddTour.aspx`. There is no server-side validation of input data, no length checks, no type validation, and no sanitization.

**Recommendation:**  
Implement Data Annotations on model classes (`[Required]`, `[MaxLength]`, `[EmailAddress]`, `[Range]`). Use FluentValidation for complex validation rules. Always validate on the server side regardless of client-side validation.

---

### ISSUE-034 — [LOW] Unreachable Code After Response.Redirect

**Category:** deprecated-api  
**Breaking Change:** No  
**Effort:** Low  

**Affected Files:**
- `Order.aspx.cs` — Lines 30–33
- `SignUpForm.aspx.cs` — Lines 37–40
- `userlogin.aspx.cs` — Lines 37–40, 47–48

**Code Snippet (Order.aspx.cs, Lines 29–33):**
```csharp
Response.Write("Registration Successful");
Response.Redirect("mybooking.aspx");
Server.Transfer("mybooking.aspx");  // UNREACHABLE
conn.Close();                        // UNREACHABLE
```

**Description:**  
`Response.Redirect()` throws a `ThreadAbortException` in .NET Framework (unless `endResponse: false` is passed), making subsequent code unreachable. `Server.Transfer()` and `conn.Close()` after `Response.Redirect()` will never execute, potentially causing connection leaks.

**Recommendation:**  
Remove `Server.Transfer()` calls entirely. Move `conn.Close()` to a `finally` block or use `using` statements. In .NET 8, `Response.Redirect()` does not throw `ThreadAbortException` — it sets the response status and the method continues executing, so explicit `return` statements are needed.

---

### ISSUE-035 — [LOW] Inline CSS Styles in .aspx Files

**Category:** webforms-migration  
**Breaking Change:** No  
**Effort:** Low  

**Affected Files:** All `.aspx` files

**Description:**  
All styling is done with inline `<style>` blocks in each `.aspx` file. There are no shared CSS files or CSS framework integration (Bootstrap CDN is referenced in some files but not consistently). This leads to code duplication and maintenance issues.

**Recommendation:**  
Create a shared CSS file in `wwwroot/css/`. Use a CSS framework (Bootstrap 5) consistently. Reference shared styles from the layout page (`_Layout.cshtml`).

---

### ISSUE-036 — [LOW] No Logging Infrastructure

**Category:** breaking-change  
**Breaking Change:** No  
**Effort:** Low  

**Affected Files:** All code-behind files

**Description:**  
The application has no logging. Errors are silently swallowed or displayed as raw `Response.Write()` messages. There is no structured logging, no log levels, and no log persistence.

**Recommendation:**  
Add `Microsoft.Extensions.Logging` (built into .NET 8). Inject `ILogger<T>` via constructor injection. Use Serilog (`Serilog.AspNetCore`) for structured logging to files, databases, or cloud services.

---

## Migration Roadmap

### Phase 1: Foundation Setup (Week 1–2) — ~20 hours
1. Create new .NET 8 solution with clean architecture (Domain, Application, Infrastructure, Web layers)
2. Set up SDK-style `.csproj` files for all projects
3. Create `appsettings.json` with connection strings
4. Set up `Program.cs` with middleware pipeline
5. Configure Entity Framework Core 8.0.0 with `DbContext`
6. Create domain entities: `Tour`, `UserInfo`, `Booking`
7. Set up EF Core migrations from existing database schema

### Phase 2: Data Access Layer (Week 2–3) — ~15 hours
1. Create `TourDbContext` with entity configurations
2. Implement repository interfaces in Domain layer
3. Implement repository classes in Infrastructure layer
4. Create service interfaces and implementations in Application layer
5. Register all services in DI container
6. Test data access with existing database

### Phase 3: Authentication & Security (Week 3–4) — ~20 hours
1. Implement ASP.NET Core Identity
2. Create admin and user roles
3. Implement secure password hashing
4. Create login/logout/registration Razor Pages
5. Apply `[Authorize]` attributes to protected pages
6. Fix SQL injection vulnerability in login

### Phase 4: UI Migration (Week 4–6) — ~40 hours
1. Create `_Layout.cshtml` master layout
2. Migrate each `.aspx` page to Razor Page (`.cshtml` + `.cshtml.cs`):
   - `userlogin.aspx` → `Login.cshtml`
   - `SignUpForm.aspx` → `Register.cshtml`
   - `MainProfilePage.aspx` → `Index.cshtml`
   - `DisplayTours.aspx` → `Tours/Index.cshtml`
   - `Order.aspx` → `Tours/Book.cshtml`
   - `mybooking.aspx` → `Bookings/MyBookings.cshtml`
   - `AdminLogin2.aspx` → (merged with Login.cshtml)
   - `AdminProfile.aspx` → `Admin/Index.cshtml`
   - `AddTour.aspx` → `Admin/AddTour.cshtml`
   - `TourCrud.aspx` → `Admin/ManageTours.cshtml`
   - `allbooking.aspx` → `Admin/AllBookings.cshtml`
   - `usercrud.aspx` → `Admin/ManageUsers.cshtml`
3. Replace all server controls with HTML + Tag Helpers
4. Implement file upload with `IFormFile`
5. Add Data Annotations validation

### Phase 5: Testing & Verification (Week 6–7) — ~15 hours
1. Write unit tests for service layer
2. Write integration tests for repositories
3. Test all CRUD operations
4. Test authentication flows
5. Verify file upload functionality
6. Performance testing

---

## Recommended Package References for .NET 8

```xml
<!-- Infrastructure Project -->
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Dapper" Version="2.1.28" />

<!-- Application Project -->
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="FluentValidation" Version="11.9.0" />

<!-- Web Project -->
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
```

---

## Files Requiring Changes

| File | Action | Priority |
|------|--------|----------|
| `Tour_Management.csproj` | Replace with SDK-style format | Critical |
| `Web.config` | Replace with `appsettings.json` | Critical |
| `packages.config` | Remove (use PackageReference) | Critical |
| All `.aspx` files (11) | Migrate to `.cshtml` Razor Pages | Critical |
| All `.aspx.cs` files (11) | Migrate to `PageModel` classes | Critical |
| All `.aspx.designer.cs` files (11) | Delete | High |
| `Properties/AssemblyInfo.cs` | Delete or update | Low |
| `userlogin.aspx.cs` | Fix SQL injection | Critical |
| `AdminLogin2.aspx.cs` | Remove hardcoded credentials | Critical |
| `SignUpForm.aspx.cs` | Fix plaintext password storage | Critical |
| `usercrud.aspx` | Remove password display | Critical |

---

## Migration Complexity Assessment

| Page | Complexity | Reason |
|------|------------|--------|
| AdminLogin2.aspx | Simple | Minimal logic, but security issues |
| AdminProfile.aspx | Simple | Navigation only |
| MainProfilePage.aspx | Simple | Navigation only |
| AddTour.aspx | Medium | File upload, form submission, ADO.NET |
| SignUpForm.aspx | Medium | Form submission, ADO.NET, validation |
| Order.aspx | Medium | Form submission, ADO.NET |
| userlogin.aspx | Medium | Authentication, SQL injection fix needed |
| DisplayTours.aspx | Medium | GridView, SqlDataSource, image display |
| TourCrud.aspx | Complex | GridView with edit/delete, SqlDataSource |
| allbooking.aspx | Complex | GridView, Chart control, SqlDataSource |
| mybooking.aspx | Medium | GridView with delete, SqlDataSource |
| usercrud.aspx | Complex | GridView with edit, complex SQL, password display |

---

*Report generated by ASP.NET Web Forms to .NET 8 Migration Analyzer*  
*Rules version: 1.1.0*
