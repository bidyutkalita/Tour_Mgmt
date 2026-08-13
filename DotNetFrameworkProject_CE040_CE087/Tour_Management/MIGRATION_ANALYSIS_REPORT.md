# ASP.NET Web Forms to .NET 8 Migration Analysis Report
## Tour Management Application

**Analysis Date:** 2025-01-30  
**Current Framework:** ASP.NET Web Forms 4.7.2  
**Target Framework:** .NET 8  
**Module:** Tour_Management  
**Analysis ID:** webform-migration-analysis-2025-01-30  

---

## Executive Summary

The Tour Management application is a classic ASP.NET Web Forms application targeting .NET Framework 4.7.2. The application provides tour management functionality including user registration/login, tour browsing, booking management, and admin capabilities. The codebase contains **11 Web Forms pages** with code-behind files, direct ADO.NET data access, hardcoded credentials, SQL injection vulnerabilities, and extensive System.Web dependencies — all of which are **incompatible with .NET 8**.

| Metric | Value |
|--------|-------|
| Total Issues Found | 42 |
| Critical Issues | 12 |
| High Issues | 14 |
| Medium Issues | 10 |
| Low Issues | 6 |
| Web Forms Pages | 11 |
| User Controls | 0 |
| Master Pages | 0 |
| Estimated Migration Effort | 80–120 hours |
| Migration Complexity | **Complex** |
| Compatibility Score | 15/100 |

---

## Issue Inventory

### CRITICAL Issues

---

#### ISSUE-001: System.Web Namespace — Not Available in .NET 8
- **Severity:** Critical  
- **Category:** webforms-migration / deprecated-api  
- **Breaking Change:** Yes  
- **Files Affected:** All 11 code-behind files  
- **Description:** Every code-behind file imports `System.Web`, `System.Web.UI`, and `System.Web.UI.WebControls`. These namespaces are part of the .NET Framework only and do not exist in .NET 8. The entire Web Forms page lifecycle (`System.Web.UI.Page`), server controls (`System.Web.UI.WebControls`), and HTTP infrastructure (`System.Web.HttpContext`) must be replaced with ASP.NET Core equivalents.
- **Code Snippet:**
```csharp
// AddTour.aspx.cs (line 5-8)
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

// All pages inherit from:
public partial class AddTour : System.Web.UI.Page
```
- **Recommendation:** Replace all `System.Web.UI.Page` base classes with Razor Pages (`PageModel`) or MVC Controllers. Replace all `System.Web.UI.WebControls` with HTML Tag Helpers or Razor syntax. Replace `System.Web.HttpContext` with `IHttpContextAccessor`.
- **Effort:** High

---

#### ISSUE-002: Web Forms Page Lifecycle — Page_Load Events
- **Severity:** Critical  
- **Category:** webforms-migration  
- **Breaking Change:** Yes  
- **Files Affected:** All 11 .aspx.cs files  
- **Description:** The Web Forms page lifecycle (`Page_Load`, `Page_PreRender`, `AutoEventWireup`, `IsPostBack`) does not exist in .NET 8. All pages use `protected void Page_Load(object sender, EventArgs e)` as the entry point. This pattern must be replaced with Razor Pages `OnGet`/`OnPost` handlers or MVC action methods.
- **Code Snippet:**
```csharp
// TourCrud.aspx.cs (lines 14-20)
protected void Page_Load(object sender, EventArgs e)
{
    if (!Page.IsPostBack)
    {
        refreshdata();
    }
}
```
- **Recommendation:** Replace `Page_Load` with `OnGet()` in Razor Pages. Replace `IsPostBack` checks with separate `OnGet`/`OnPost` handler methods. The `!IsPostBack` pattern maps to `OnGet()` and postback handlers map to `OnPost()`.
- **Effort:** High

---

#### ISSUE-003: SqlDataSource Server Control — Not Available in .NET 8
- **Severity:** Critical  
- **Category:** webforms-migration  
- **Breaking Change:** Yes  
- **Files Affected:** DisplayTours.aspx, TourCrud.aspx, usercrud.aspx, allbooking.aspx, mybooking.aspx  
- **Description:** `<asp:SqlDataSource>` is a Web Forms server control that provides declarative data binding directly in markup. It does not exist in .NET 8. All data access must be moved to code (services/repositories) and passed to views via models.
- **Code Snippet:**
```xml
<!-- DisplayTours.aspx (lines 18-19) -->
<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
    ConnectionString="<%$ ConnectionStrings:dbconnection %>" 
    SelectCommand="SELECT [TOUR_NAME], [pic], [PRICE], [DAYS], [LOCATIONS], [TOUR_ID] FROM [Tour]">
</asp:SqlDataSource>

<!-- TourCrud.aspx (lines 28-33) -->
<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
    ConnectionString="<%$ ConnectionStrings:dbconnection %>" 
    SelectCommand="SELECT * FROM [Tour]"
    UpdateCommand="UPDATE [Tour] Set [TOUR_NAME]=@TOUR_NAME,[PLACE]=@PLACE,[DAYS]=@DAYS,[PRICE]=@PRICE,[LOCATIONS]=@LOCATIONS,[TOUR_INFO]=@TOUR_INFO Where [TOUR_ID]=@TOUR_ID"
    DeleteCommand="Delete from [Tour] Where [TOUR_ID]=@TOUR_ID">
</asp:SqlDataSource>
```
- **Recommendation:** Replace `SqlDataSource` with EF Core repositories. Move all SQL queries to repository classes. Pass data to Razor Pages via `PageModel` properties. Use `IActionResult` or `PageResult` to render data.
- **Effort:** High

---

#### ISSUE-004: GridView Server Control — Not Available in .NET 8
- **Severity:** Critical  
- **Category:** webforms-migration  
- **Breaking Change:** Yes  
- **Files Affected:** DisplayTours.aspx, TourCrud.aspx, usercrud.aspx, allbooking.aspx, mybooking.aspx  
- **Description:** `<asp:GridView>` is a Web Forms server control with built-in paging, sorting, editing, and deleting. It does not exist in .NET 8. All GridView usages must be replaced with HTML tables rendered via Razor syntax or modern JavaScript grid components.
- **Code Snippet:**
```xml
<!-- TourCrud.aspx (lines 9-27) -->
<asp:GridView ID="GridView1" runat="server" 
    AutoGenerateColumns="False" 
    AutoGenerateDeleteButton="True" 
    AutoGenerateEditButton="True" 
    DataKeyNames="TOUR_ID" 
    DataSourceID="SqlDataSource1">
    <Columns>
        <asp:BoundField DataField="TOUR_ID" HeaderText="TOUR_ID" />
        ...
    </Columns>
</asp:GridView>
```
- **Recommendation:** Replace `GridView` with Razor `@foreach` loops rendering HTML `<table>` elements. For inline editing/deleting, use separate Edit/Delete Razor Pages or AJAX calls. Consider using a JavaScript data grid library (e.g., DataTables.js) for advanced features.
- **Effort:** High

---

#### ISSUE-005: Direct ADO.NET with SqlConnection — Must Migrate to EF Core
- **Severity:** Critical  
- **Category:** deprecated-api / data-access  
- **Breaking Change:** Yes  
- **Files Affected:** AddTour.aspx.cs, SignUpForm.aspx.cs, Order.aspx.cs, userlogin.aspx.cs, TourCrud.aspx.cs, DisplayTours.aspx.cs  
- **Description:** All data access uses raw ADO.NET (`SqlConnection`, `SqlCommand`, `SqlDataAdapter`) with `System.Data.SqlClient`. While `System.Data.SqlClient` has a .NET 8 compatible replacement (`Microsoft.Data.SqlClient`), the pattern of embedding raw SQL in code-behind files violates clean architecture principles and must be refactored into a proper data access layer using EF Core.
- **Code Snippet:**
```csharp
// AddTour.aspx.cs (lines 21-35)
SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
conn.Open();
string insertQuery = "insert into Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) values(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";
SqlCommand com = new SqlCommand(insertQuery, conn);
com.Parameters.AddWithValue("@TOUR_NAME", tour_name.Text);
...
com.ExecuteNonQuery();
conn.Close();
```
- **Recommendation:** Replace all ADO.NET code with EF Core. Create `TourManagementDbContext` inheriting from `DbContext`. Create entity classes (`Tour`, `UserInfo`, `Booking`). Create repository interfaces and implementations. Use `Microsoft.EntityFrameworkCore.SqlServer` 8.0.0.
- **Effort:** High

---

#### ISSUE-006: ConfigurationManager — Not Available in .NET 8
- **Severity:** Critical  
- **Category:** deprecated-api / configuration  
- **Breaking Change:** Yes  
- **Files Affected:** AddTour.aspx.cs (line 22), SignUpForm.aspx.cs (line 21), Order.aspx.cs (line 14), userlogin.aspx.cs (line 22), TourCrud.aspx.cs (line 14), DisplayTours.aspx.cs (line 4)  
- **Description:** `System.Configuration.ConfigurationManager` is used to read connection strings from `Web.config`. This class is not available in .NET 8 (it requires the `System.Configuration.ConfigurationManager` NuGet package as a compatibility shim, but the proper approach is to use `IConfiguration` with `appsettings.json`).
- **Code Snippet:**
```csharp
// AddTour.aspx.cs (line 22)
SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);

// DisplayTours.aspx.cs (line 4)
using System.Configuration;
```
- **Recommendation:** Replace `ConfigurationManager.ConnectionStrings["dbconnection"]` with `IConfiguration["ConnectionStrings:dbconnection"]` or use the Options pattern. Migrate `Web.config` connection strings to `appsettings.json`. Register `IConfiguration` via dependency injection in `Program.cs`.
- **Effort:** Medium

---

#### ISSUE-007: Web.config — Must Be Replaced with appsettings.json
- **Severity:** Critical  
- **Category:** configuration / webforms-migration  
- **Breaking Change:** Yes  
- **Files Affected:** Web.config  
- **Description:** `Web.config` is the configuration system for ASP.NET Web Forms and is not used in .NET 8. The connection string, app settings, compilation settings, HTTP handlers, and HTTP modules defined in `Web.config` must all be migrated to `appsettings.json` and `Program.cs`.
- **Code Snippet:**
```xml
<!-- Web.config (lines 27-29) -->
<connectionStrings>
    <add name="dbconnection" connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gajer\source\repos\Tour_Management\Tour_Management\App_Data\tourdb.mdf;Integrated Security=True" providerName="System.Data.SqlClient"/>
</connectionStrings>
<appSettings>
    <add key="ValidationSettings:UnobtrusiveValidationMode" value="None" />
    <add key="ChartImageHandler" value="storage=file;timeout=20;dir=c:\TempImageFiles\;" />
</appSettings>
```
- **Recommendation:** Create `appsettings.json` with connection strings section. Update connection string to use `Microsoft.Data.SqlClient` provider. Remove hardcoded absolute file paths. Configure environment-specific settings in `appsettings.Development.json`.
- **Effort:** Medium

---

#### ISSUE-008: Non-SDK-Style Project File — Must Be Converted
- **Severity:** Critical  
- **Category:** webforms-migration / project-configuration  
- **Breaking Change:** Yes  
- **Files Affected:** Tour_Management.csproj  
- **Description:** The project file uses the old MSBuild format with `ToolsVersion="15.0"` and `ProjectTypeGuids` for Web Application projects. .NET 8 requires SDK-style project files using `<Project Sdk="Microsoft.NET.Sdk.Web">`. The old format is incompatible with the .NET 8 SDK.
- **Code Snippet:**
```xml
<!-- Tour_Management.csproj (lines 1-5) -->
<Project ToolsVersion="15.0" DefaultTargets="Build" xmlns="http://schemas.microsoft.com/developer/msbuild/2003">
  <PropertyGroup>
    <ProjectTypeGuids>{349c5851-65df-11da-9384-00065b846f21};{fae04ec0-301f-11d3-bf4b-00c04f79efbc}</ProjectTypeGuids>
    <TargetFrameworkVersion>v4.7.2</TargetFrameworkVersion>
  </PropertyGroup>
```
- **Recommendation:** Replace the entire project file with an SDK-style `.csproj` using `<Project Sdk="Microsoft.NET.Sdk.Web">` and `<TargetFramework>net8.0</TargetFramework>`. Remove all `ProjectTypeGuids`, `ToolsVersion`, and explicit file includes (SDK-style projects include files automatically).
- **Effort:** Medium

---

#### ISSUE-009: SQL Injection Vulnerability — Hardcoded String Concatenation
- **Severity:** Critical  
- **Category:** security  
- **Breaking Change:** No (security fix required)  
- **Files Affected:** userlogin.aspx.cs  
- **Description:** The user login query uses string concatenation to build SQL queries, creating a critical SQL injection vulnerability. This must be fixed during migration.
- **Code Snippet:**
```csharp
// userlogin.aspx.cs (lines 24-25)
string checkPasswordQuery = "select password from Userinfo where password='" + txtPassword.Text + "' and email = '" + txtEmail.Text + "'";
SqlCommand passComm = new SqlCommand(checkPasswordQuery, conn);
```
- **Recommendation:** Replace with parameterized queries or EF Core LINQ queries. Additionally, passwords must never be stored or compared in plain text — implement proper password hashing using `BCrypt.Net` or ASP.NET Core Identity's `IPasswordHasher<T>`.
- **Effort:** Medium

---

#### ISSUE-010: Hardcoded Admin Credentials — Critical Security Issue
- **Severity:** Critical  
- **Category:** security  
- **Breaking Change:** No (security fix required)  
- **Files Affected:** AdminLogin2.aspx.cs  
- **Description:** Admin credentials are hardcoded directly in the source code. This is a critical security vulnerability that must be addressed during migration.
- **Code Snippet:**
```csharp
// AdminLogin2.aspx.cs (lines 14-18)
protected void Page_Load(object sender, EventArgs e)
{
    if (password.Text == "admin" && name.Text == "admin@gmail.com")
    {
        Response.Redirect("AdminProfile.aspx");
```
- **Recommendation:** Replace hardcoded credentials with ASP.NET Core Identity. Implement role-based authorization with `[Authorize(Roles = "Admin")]` attribute. Store admin credentials in the database with proper password hashing.
- **Effort:** High

---

#### ISSUE-011: Server.MapPath — Not Available in .NET 8
- **Severity:** Critical  
- **Category:** deprecated-api  
- **Breaking Change:** Yes  
- **Files Affected:** AddTour.aspx.cs  
- **Description:** `Server.MapPath()` is a `System.Web.HttpServerUtility` method that maps virtual paths to physical file system paths. It does not exist in .NET 8.
- **Code Snippet:**
```csharp
// AddTour.aspx.cs (line 36)
FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + FileUpload1.FileName);
```
- **Recommendation:** Replace `Server.MapPath("~/Tour_pics/")` with `IWebHostEnvironment.WebRootPath` or `IWebHostEnvironment.ContentRootPath`. Inject `IWebHostEnvironment` via constructor injection in the Razor Page model. Use `Path.Combine(env.WebRootPath, "Tour_pics", fileName)`.
- **Effort:** Low

---

#### ISSUE-012: Response.Write / Response.Redirect / Server.Transfer — Incompatible Patterns
- **Severity:** Critical  
- **Category:** webforms-migration / deprecated-api  
- **Breaking Change:** Yes  
- **Files Affected:** AddTour.aspx.cs, SignUpForm.aspx.cs, Order.aspx.cs, userlogin.aspx.cs, AdminLogin2.aspx.cs  
- **Description:** `Response.Write()`, `Response.Redirect()`, and `Server.Transfer()` are Web Forms patterns. While `Response.Redirect()` has an ASP.NET Core equivalent, `Response.Write()` for inline output and `Server.Transfer()` do not exist in .NET 8. Additionally, calling both `Response.Redirect()` and `Server.Transfer()` on the same request (as seen in multiple files) is a logic error.
- **Code Snippet:**
```csharp
// Order.aspx.cs (lines 28-30) — Logic error: both called sequentially
Response.Write("Registration Successful");
Response.Redirect("mybooking.aspx");
Server.Transfer("mybooking.aspx");  // Never reached

// SignUpForm.aspx.cs (lines 34-36) — Same pattern
Response.Redirect("userlogin.aspx");
Server.Transfer("usercrud.aspx");  // Never reached
```
- **Recommendation:** Replace `Response.Write()` with `TempData["Message"]` and display in the view. Replace `Response.Redirect()` with `return RedirectToPage("/Booking/MyBooking")`. Remove all `Server.Transfer()` calls. Fix the logic error where both redirect methods are called.
- **Effort:** Medium

---

### HIGH Issues

---

#### ISSUE-013: FileUpload Server Control — Not Available in .NET 8
- **Severity:** High  
- **Category:** webforms-migration  
- **Breaking Change:** Yes  
- **Files Affected:** AddTour.aspx, AddTour.aspx.cs  
- **Description:** `<asp:FileUpload>` is a Web Forms server control for file uploads. It does not exist in .NET 8.
- **Code Snippet:**
```xml
<!-- AddTour.aspx (line 55) -->
<asp:FileUpload ID="FileUpload1" Style="background-image: url('../Pics/add.png');" runat="server"/>
```
```csharp
// AddTour.aspx.cs (line 36)
FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + FileUpload1.FileName);
```
- **Recommendation:** Replace with standard HTML `<input type="file">` and use `IFormFile` in the Razor Page model. Use `IWebHostEnvironment` to resolve the upload path. Implement file validation (type, size) in the page model.
- **Effort:** Medium

---

#### ISSUE-014: asp:Label, asp:TextBox, asp:Button Server Controls — Not Available in .NET 8
- **Severity:** High  
- **Category:** webforms-migration  
- **Breaking Change:** Yes  
- **Files Affected:** All .aspx files  
- **Description:** All Web Forms server controls (`<asp:Label>`, `<asp:TextBox>`, `<asp:Button>`, `<asp:DropDownList>`, `<asp:HyperLink>`, `<asp:RegularExpressionValidator>`) must be replaced with standard HTML elements and ASP.NET Core Tag Helpers.
- **Code Snippet:**
```xml
<!-- SignUpForm.aspx (lines 22-24) -->
<asp:Label ID="Label1" runat="server" Text="Email"/>
<asp:TextBox ID="email" TextMode="Email" runat="server" required="true" ForeColor="Black" class="form-control"/>
<asp:Button ID="Register" runat="server" Text="Register" OnClick="Register_Click" />
```
- **Recommendation:** Replace `<asp:Label>` with `<label asp-for="...">`. Replace `<asp:TextBox>` with `<input asp-for="...">`. Replace `<asp:Button>` with `<button type="submit">`. Replace `<asp:DropDownList>` with `<select asp-for="..." asp-items="...">`. Replace `<asp:RegularExpressionValidator>` with Data Annotations and `<span asp-validation-for="...">`.
- **Effort:** High

---

#### ISSUE-015: asp:RegularExpressionValidator — Not Available in .NET 8
- **Severity:** High  
- **Category:** webforms-migration  
- **Breaking Change:** Yes  
- **Files Affected:** AddTour.aspx  
- **Description:** Web Forms validation controls (`<asp:RegularExpressionValidator>`, `<asp:RequiredFieldValidator>`, etc.) do not exist in .NET 8.
- **Code Snippet:**
```xml
<!-- AddTour.aspx (line 72) -->
<asp:RegularExpressionValidator ID="RegularExpressionValidator1" Display="Dynamic" 
    ControlToValidate="tour_info" 
    ValidationExpression="^[\s\S]{0,250}$" 
    runat="server" 
    ErrorMessage="Characters less than 250">
</asp:RegularExpressionValidator>
```
- **Recommendation:** Replace with Data Annotations on the ViewModel/DTO: `[MaxLength(250, ErrorMessage = "Characters less than 250")]`. Add `<span asp-validation-for="TourInfo" class="text-danger"></span>` in the Razor view. Include jQuery Validation scripts for client-side validation.
- **Effort:** Low

---

#### ISSUE-016: System.Web.DataVisualization.Charting — Not Available in .NET 8
- **Severity:** High  
- **Category:** deprecated-api / package-compatibility  
- **Breaking Change:** Yes  
- **Files Affected:** allbooking.aspx, Web.config, Tour_Management.csproj  
- **Description:** `System.Web.DataVisualization` (the Chart control) is a .NET Framework-only assembly. It is registered in `Web.config` and referenced in `allbooking.aspx`. There is no direct equivalent in .NET 8.
- **Code Snippet:**
```xml
<!-- allbooking.aspx (line 3) -->
<%@ Register assembly="System.Web.DataVisualization, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35" 
    namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>

<!-- Web.config (lines 5-9) -->
<add name="ChartImageHandler" preCondition="integratedMode" verb="GET,HEAD,POST" path="ChartImg.axd" 
    type="System.Web.UI.DataVisualization.Charting.ChartHttpHandler, System.Web.DataVisualization, ..."/>
```
- **Recommendation:** Replace with a modern charting library. Options include: `LiveCharts2` (NuGet: `LiveChartsCore.SkiaSharpView.AspNetCore`), or client-side libraries like `Chart.js` or `ApexCharts` via CDN. Remove all `System.Web.DataVisualization` references from `Web.config` and `.csproj`.
- **Effort:** Medium

---

#### ISSUE-017: Microsoft.CodeDom.Providers.DotNetCompilerPlatform — Not Needed in .NET 8
- **Severity:** High  
- **Category:** package-compatibility  
- **Breaking Change:** Yes  
- **Files Affected:** packages.config, Tour_Management.csproj, Web.config  
- **Description:** `Microsoft.CodeDom.Providers.DotNetCompilerPlatform` is a Web Forms-specific package that provides Roslyn compiler support for runtime compilation of .aspx files. It is not needed in .NET 8 and must be removed.
- **Code Snippet:**
```xml
<!-- packages.config (line 3) -->
<package id="Microsoft.CodeDom.Providers.DotNetCompilerPlatform" version="2.0.1" targetFramework="net472" />

<!-- Web.config (lines 33-38) -->
<system.codedom>
    <compilers>
        <compiler language="c#;cs;csharp" extension=".cs" 
            type="Microsoft.CodeDom.Providers.DotNetCompilerPlatform.CSharpCodeProvider, ..." />
    </compilers>
</system.codedom>
```
- **Recommendation:** Remove the package reference entirely. .NET 8 uses the Roslyn compiler natively. Remove the `<system.codedom>` section from `Web.config` (which will be replaced by `appsettings.json` anyway).
- **Effort:** Low

---

#### ISSUE-018: Passwords Stored in Plain Text — Security Vulnerability
- **Severity:** High  
- **Category:** security  
- **Breaking Change:** No (security fix required)  
- **Files Affected:** SignUpForm.aspx.cs, userlogin.aspx.cs, usercrud.aspx  
- **Description:** User passwords are stored and compared in plain text. The `usercrud.aspx` GridView even displays the `Password` column directly in the UI.
- **Code Snippet:**
```csharp
// SignUpForm.aspx.cs (line 29)
com.Parameters.AddWithValue("@Password", password1.Text);  // Plain text storage

// userlogin.aspx.cs (lines 24-30)
string checkPasswordQuery = "select password from Userinfo where password='" + txtPassword.Text + "'...";
if (password == txtPassword.Text)  // Plain text comparison
```
```xml
<!-- usercrud.aspx (line 14) -->
<asp:BoundField DataField="Password" HeaderText="Password" SortExpression="Password" />
```
- **Recommendation:** Implement ASP.NET Core Identity for user management. Use `IPasswordHasher<T>` for password hashing. Never display passwords in the UI. Remove the Password column from the user management grid.
- **Effort:** High

---

#### ISSUE-019: No Authentication/Authorization Mechanism
- **Severity:** High  
- **Category:** security / webforms-migration  
- **Breaking Change:** No (new feature required)  
- **Files Affected:** All pages  
- **Description:** The application has no proper authentication or authorization. Admin pages (`AdminProfile.aspx`, `AddTour.aspx`, `TourCrud.aspx`, `allbooking.aspx`) are accessible without authentication. The admin login check is done in `Page_Load` with hardcoded credentials. User login sets no session state.
- **Code Snippet:**
```csharp
// AdminLogin2.aspx.cs (lines 14-19) — No session, no cookie, no token
protected void Page_Load(object sender, EventArgs e)
{
    if (password.Text == "admin" && name.Text == "admin@gmail.com")
    {
        Response.Redirect("AdminProfile.aspx");
    }
}
```
- **Recommendation:** Implement ASP.NET Core Identity with cookie authentication. Add `[Authorize]` attribute to protected pages. Create `Admin` and `User` roles. Configure authentication middleware in `Program.cs`. Implement proper login/logout with `SignInManager<T>`.
- **Effort:** High

---

#### ISSUE-020: Session State Not Implemented (Commented Out)
- **Severity:** High  
- **Category:** webforms-migration  
- **Breaking Change:** No  
- **Files Affected:** userlogin.aspx.cs  
- **Description:** Session state usage is commented out, meaning there is no way to track logged-in users across pages. This is a functional gap that must be addressed during migration.
- **Code Snippet:**
```csharp
// userlogin.aspx.cs (line 31)
//Session["New"] = txtEmail.Text;  // Commented out — no session tracking
```
- **Recommendation:** Implement ASP.NET Core Identity with cookie-based authentication. Use `HttpContext.User.Identity.Name` to access the current user. For temporary data, use `TempData`. For distributed scenarios, use `IDistributedCache`.
- **Effort:** Medium

---

#### ISSUE-021: runat="server" Attribute — Not Valid in .NET 8
- **Severity:** High  
- **Category:** webforms-migration  
- **Breaking Change:** Yes  
- **Files Affected:** All .aspx files  
- **Description:** The `runat="server"` attribute is a Web Forms-specific attribute that marks HTML elements and controls for server-side processing. It does not exist in Razor Pages or MVC views.
- **Code Snippet:**
```xml
<!-- Order.aspx (line 6) -->
<form runat="server">
<div class="container" runat="server">

<!-- AddTour.aspx (line 3) -->
<head runat="server">
```
- **Recommendation:** Remove all `runat="server"` attributes. Replace `<form runat="server">` with `<form method="post">` and add `@Html.AntiForgeryToken()` or use the `asp-antiforgery="true"` Tag Helper. Replace `<head runat="server">` with a standard `<head>` tag.
- **Effort:** Medium

---

#### ISSUE-022: <%@ Page %> Directive — Not Valid in .NET 8
- **Severity:** High  
- **Category:** webforms-migration  
- **Breaking Change:** Yes  
- **Files Affected:** All 11 .aspx files  
- **Description:** The `<%@ Page %>` directive is Web Forms-specific and links the markup to the code-behind class. Razor Pages use `@page` and `@model` directives instead.
- **Code Snippet:**
```xml
<!-- AddTour.aspx (line 1) -->
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="AddTour.aspx.cs" Inherits="Tour_Management.AddTour" %>

<!-- allbooking.aspx (lines 1-3) -->
<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="allbooking.aspx.cs" Inherits="Tour_Management.allbooking" %>
<%@ Register assembly="System.Web.DataVisualization, ..." namespace="System.Web.UI.DataVisualization.Charting" tagprefix="asp" %>
```
- **Recommendation:** Replace `.aspx` files with `.cshtml` Razor Pages. Replace `<%@ Page %>` with `@page` and `@model TourManagement.Web.Pages.Tours.IndexModel`. Replace `<%@ Register %>` with appropriate `@using` statements or Tag Helper registrations.
- **Effort:** High

---

#### ISSUE-023: <%# Eval() %> Data Binding Expressions — Not Valid in .NET 8
- **Severity:** High  
- **Category:** webforms-migration  
- **Breaking Change:** Yes  
- **Files Affected:** DisplayTours.aspx, TourCrud.aspx  
- **Description:** Web Forms data binding expressions (`<%# Eval("fieldName") %>`) are used inside `TemplateField` controls. These do not exist in Razor Pages.
- **Code Snippet:**
```xml
<!-- DisplayTours.aspx (lines 22-24) -->
<asp:TemplateField HeaderText="pic">
    <ItemTemplate>
        <img src="Tour_pics/<%#Eval("pic") %>" style="width:200px;height:200px" />
    </ItemTemplate>
</asp:TemplateField>
```
- **Recommendation:** Replace with Razor syntax in `.cshtml` files: `<img src="~/Tour_pics/@item.Pic" style="width:200px;height:200px" />` inside a `@foreach` loop iterating over the model collection.
- **Effort:** Low

---

#### ISSUE-024: <%$ ConnectionStrings:dbconnection %> Expression — Not Valid in .NET 8
- **Severity:** High  
- **Category:** webforms-migration / configuration  
- **Breaking Change:** Yes  
- **Files Affected:** DisplayTours.aspx, TourCrud.aspx, usercrud.aspx, allbooking.aspx, mybooking.aspx  
- **Description:** The `<%$ ConnectionStrings:dbconnection %>` expression is a Web Forms resource expression that reads from `Web.config`. It does not exist in .NET 8.
- **Code Snippet:**
```xml
<!-- DisplayTours.aspx (line 18) -->
<asp:SqlDataSource ID="SqlDataSource1" runat="server" 
    ConnectionString="<%$ ConnectionStrings:dbconnection %>" ...>
```
- **Recommendation:** Remove all `SqlDataSource` controls. Connection strings should be read from `appsettings.json` via `IConfiguration` and passed to `DbContext` through dependency injection.
- **Effort:** Low (resolved as part of SqlDataSource migration)

---

#### ISSUE-025: CodeFile vs CodeBehind Attribute Inconsistency
- **Severity:** High  
- **Category:** webforms-migration  
- **Breaking Change:** No (pre-existing issue)  
- **Files Affected:** AdminLogin2.aspx  
- **Description:** `AdminLogin2.aspx` uses `CodeFile="AdminLogin2.aspx.cs"` (Web Site project model) while all other pages use `CodeBehind="..."` (Web Application project model). This inconsistency indicates a copy-paste error in the original project.
- **Code Snippet:**
```xml
<!-- AdminLogin2.aspx (line 1) -->
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminLogin2.aspx.cs" Inherits="Tour_Management.AdminLogin2" %>
```
- **Recommendation:** This inconsistency is moot during migration since all `.aspx` files will be replaced with `.cshtml` Razor Pages. Document this as a pre-existing issue.
- **Effort:** Low

---

#### ISSUE-026: No Global.asax Found — Application Startup Logic Missing
- **Severity:** High  
- **Category:** webforms-migration  
- **Breaking Change:** No  
- **Files Affected:** (missing file)  
- **Description:** No `Global.asax` file was found in the project. This means there is no application-level startup configuration, error handling, or routing configuration. In .NET 8, all startup logic goes in `Program.cs`.
- **Recommendation:** Create `Program.cs` with full ASP.NET Core startup configuration including: service registration, middleware pipeline, authentication, EF Core DbContext, AutoMapper, and routing.
- **Effort:** Medium

---

### MEDIUM Issues

---

#### ISSUE-027: No Master Page / Layout Page
- **Severity:** Medium  
- **Category:** webforms-migration  
- **Breaking Change:** No  
- **Files Affected:** All .aspx files  
- **Description:** The application has no master page. Navigation menus are duplicated inline in `AdminProfile.aspx` and `MainProfilePage.aspx`. In .NET 8, a shared `_Layout.cshtml` should be created.
- **Code Snippet:**
```xml
<!-- AdminProfile.aspx (lines 52-58) — Duplicated navigation -->
<ul class="nav navbar nav-tabs">
    <li class="pull-left"><a href="AdminProfile.aspx"> Profile </a></li>
    <li class="pull-left"><a href="AddTour.aspx"> Add Tours</a></li>
    ...
</ul>
```
- **Recommendation:** Create `Pages/Shared/_Layout.cshtml` with Bootstrap navigation. Create separate layouts for admin and user sections. Use `_ViewStart.cshtml` to apply the layout globally.
- **Effort:** Medium

---

#### ISSUE-028: Hardcoded Absolute File Path in Connection String
- **Severity:** Medium  
- **Category:** configuration  
- **Breaking Change:** No  
- **Files Affected:** Web.config  
- **Description:** The connection string contains a hardcoded absolute path to the developer's local machine (`C:\Users\gajer\source\repos\...`). This will not work on any other machine.
- **Code Snippet:**
```xml
<!-- Web.config (line 28) -->
<add name="dbconnection" connectionString="Data Source=(LocalDB)\MSSQLLocalDB;AttachDbFilename=C:\Users\gajer\source\repos\Tour_Management\Tour_Management\App_Data\tourdb.mdf;Integrated Security=True" .../>
```
- **Recommendation:** Replace with a proper SQL Server connection string using `|DataDirectory|` substitution or a server-based connection. In `appsettings.json`, use `|DataDirectory|` or configure a proper SQL Server instance. Use environment variables for production connection strings.
- **Effort:** Low

---

#### ISSUE-029: No Dependency Injection — Tight Coupling
- **Severity:** Medium  
- **Category:** architecture  
- **Breaking Change:** No  
- **Files Affected:** All code-behind files  
- **Description:** All code-behind files directly instantiate `SqlConnection` and other dependencies. There is no dependency injection, making the code untestable and tightly coupled.
- **Recommendation:** Implement constructor injection in Razor Page models. Register services in `Program.cs`. Create service interfaces and implementations following the repository pattern.
- **Effort:** High

---

#### ISSUE-030: No Error Handling
- **Severity:** Medium  
- **Category:** code-quality  
- **Breaking Change:** No  
- **Files Affected:** AddTour.aspx.cs, SignUpForm.aspx.cs, Order.aspx.cs, userlogin.aspx.cs  
- **Description:** Database operations have no try-catch error handling. If a database operation fails, the application will throw an unhandled exception.
- **Code Snippet:**
```csharp
// AddTour.aspx.cs (lines 21-40) — No try-catch
SqlConnection conn = new SqlConnection(...);
conn.Open();
// ... operations ...
com.ExecuteNonQuery();
conn.Close();  // Not called if exception occurs — connection leak
```
- **Recommendation:** Wrap all database operations in try-catch-finally blocks or use `using` statements. Implement global exception handling middleware in `Program.cs`. Use `ILogger<T>` for error logging.
- **Effort:** Medium

---

#### ISSUE-031: SqlConnection Not Disposed Properly
- **Severity:** Medium  
- **Category:** code-quality  
- **Breaking Change:** No  
- **Files Affected:** AddTour.aspx.cs, SignUpForm.aspx.cs, Order.aspx.cs, userlogin.aspx.cs, TourCrud.aspx.cs  
- **Description:** `SqlConnection` objects are opened but not wrapped in `using` statements. If an exception occurs before `conn.Close()`, the connection is never closed, causing connection pool exhaustion.
- **Code Snippet:**
```csharp
// AddTour.aspx.cs (lines 21-40)
SqlConnection conn = new SqlConnection(...);
conn.Open();
// ... if exception here, conn.Close() never called
conn.Close();
```
- **Recommendation:** Use `using` statements for all `IDisposable` objects. With EF Core, the `DbContext` is managed by the DI container with scoped lifetime, eliminating this issue.
- **Effort:** Low (resolved as part of EF Core migration)

---

#### ISSUE-032: No Input Validation Beyond HTML Required Attribute
- **Severity:** Medium  
- **Category:** security / code-quality  
- **Breaking Change:** No  
- **Files Affected:** AddTour.aspx, SignUpForm.aspx, Order.aspx  
- **Description:** Server-side input validation is minimal. Only one `RegularExpressionValidator` exists (for tour info length). All other fields rely only on the HTML `required` attribute, which can be bypassed.
- **Recommendation:** Implement FluentValidation or Data Annotations for server-side validation. Add `ModelState.IsValid` checks in all POST handlers. Implement anti-forgery token validation.
- **Effort:** Medium

---

#### ISSUE-033: No Async/Await — Synchronous Database Operations
- **Severity:** Medium  
- **Category:** code-quality / performance  
- **Breaking Change:** No  
- **Files Affected:** All code-behind files with database operations  
- **Description:** All database operations are synchronous, blocking threads and reducing scalability.
- **Code Snippet:**
```csharp
// AddTour.aspx.cs (line 38)
com.ExecuteNonQuery();  // Synchronous — blocks thread
```
- **Recommendation:** Use async/await throughout. Replace `ExecuteNonQuery()` with `ExecuteNonQueryAsync()`. With EF Core, use `SaveChangesAsync()`, `ToListAsync()`, `FirstOrDefaultAsync()`, etc.
- **Effort:** Low (natural outcome of EF Core migration)

---

#### ISSUE-034: TourCrud.aspx — Incomplete/Broken Data Binding
- **Severity:** Medium  
- **Category:** code-quality  
- **Breaking Change:** No  
- **Files Affected:** TourCrud.aspx.cs  
- **Description:** The `refreshdata()` method in `TourCrud.aspx.cs` has all GridView data binding code commented out. The GridView is bound via `SqlDataSource` in the markup, but the code-behind method is incomplete and non-functional.
- **Code Snippet:**
```csharp
// TourCrud.aspx.cs (lines 18-35)
public void refreshdata()
{
    SqlConnection conn = new SqlConnection(...);
    conn.Open();
    string insertQuery = "select * from Tour";
    SqlCommand com = new SqlCommand(insertQuery, conn);
    // GridView1.DataSource = insertQuery;  // COMMENTED OUT
    // GridView1.DataBind();               // COMMENTED OUT
    // ... all remaining code commented out
}
```
- **Recommendation:** During migration, implement proper data retrieval using EF Core repository. The `refreshdata()` method should be replaced with a proper `OnGet()` handler that populates the page model's `Tours` property.
- **Effort:** Low

---

#### ISSUE-035: No Logging Infrastructure
- **Severity:** Medium  
- **Category:** code-quality  
- **Breaking Change:** No  
- **Files Affected:** All files  
- **Description:** The application has no logging infrastructure. No errors, warnings, or informational messages are logged anywhere.
- **Recommendation:** Implement `Microsoft.Extensions.Logging` with `ILogger<T>`. Configure Serilog as the logging provider in `Program.cs`. Add structured logging for all important operations (user login, tour creation, booking, errors).
- **Effort:** Medium

---

### LOW Issues

---

#### ISSUE-036: No Pagination for GridViews
- **Severity:** Low  
- **Category:** performance  
- **Breaking Change:** No  
- **Files Affected:** DisplayTours.aspx, TourCrud.aspx, allbooking.aspx, mybooking.aspx, usercrud.aspx  
- **Description:** All GridViews load all records without pagination, which will cause performance issues as data grows.
- **Recommendation:** Implement server-side pagination using EF Core's `Skip()` and `Take()` methods. Add pagination controls to Razor Pages.
- **Effort:** Medium

---

#### ISSUE-037: Inline CSS Styles — Should Be Externalized
- **Severity:** Low  
- **Category:** code-quality  
- **Breaking Change:** No  
- **Files Affected:** All .aspx files  
- **Description:** All CSS is defined inline within `<style>` tags in each page. There is no shared stylesheet.
- **Recommendation:** Create `wwwroot/css/site.css` with shared styles. Use Bootstrap 5 for responsive design. Reference the stylesheet from `_Layout.cshtml`.
- **Effort:** Low

---

#### ISSUE-038: No HTTPS Enforcement
- **Severity:** Low  
- **Category:** security  
- **Breaking Change:** No  
- **Files Affected:** Web.config, (missing) Program.cs  
- **Description:** No HTTPS redirection is configured.
- **Recommendation:** Add `app.UseHttpsRedirection()` and `app.UseHsts()` in `Program.cs`. Configure HTTPS in `appsettings.json`.
- **Effort:** Low

---

#### ISSUE-039: No Anti-Forgery Token Protection
- **Severity:** Low  
- **Category:** security  
- **Breaking Change:** No  
- **Files Affected:** All forms  
- **Description:** No CSRF protection is implemented on any form.
- **Recommendation:** Razor Pages automatically include anti-forgery token validation for POST requests. Ensure all forms use `<form method="post">` with the `asp-antiforgery="true"` Tag Helper or `@Html.AntiForgeryToken()`.
- **Effort:** Low (automatic in Razor Pages)

---

#### ISSUE-040: Designer Files — Not Needed in .NET 8
- **Severity:** Low  
- **Category:** webforms-migration  
- **Breaking Change:** No  
- **Files Affected:** All *.aspx.designer.cs files (11 files)  
- **Description:** `.aspx.designer.cs` files are auto-generated Web Forms files that declare server control fields. They are not needed in .NET 8 Razor Pages.
- **Recommendation:** Delete all `.aspx.designer.cs` files during migration. They will be replaced by strongly-typed Razor Page models.
- **Effort:** Low

---

#### ISSUE-041: App_Data Folder with MDF File — Not Recommended for Production
- **Severity:** Low  
- **Category:** configuration  
- **Breaking Change:** No  
- **Files Affected:** App_Data/tourdb.mdf  
- **Description:** The application uses a LocalDB `.mdf` file stored in the `App_Data` folder. This is a development-only pattern and not suitable for production.
- **Recommendation:** Migrate to a proper SQL Server instance. Use EF Core migrations to create the database schema. Store the connection string in `appsettings.json` with environment-specific overrides.
- **Effort:** Medium

---

#### ISSUE-042: Missing DOCTYPE and HTML Structure Issues
- **Severity:** Low  
- **Category:** code-quality  
- **Breaking Change:** No  
- **Files Affected:** userlogin.aspx  
- **Description:** `userlogin.aspx` is missing the `<!DOCTYPE html>` declaration and has an unclosed `</div>` tag (line 30: `</div>` without matching opening tag).
- **Code Snippet:**
```html
<!-- userlogin.aspx (lines 1-35) — Missing DOCTYPE, unclosed div -->
<html xmlns="http://www.w3.org/1999/xhtml">
...
    </div>  <!-- Line 30: No matching opening div -->
```
- **Recommendation:** Fix HTML structure during migration to Razor Pages. All Razor Pages should have proper HTML5 structure via the shared `_Layout.cshtml`.
- **Effort:** Low

---

## Web Forms Component Inventory

| File | Type | Complexity | Migration Target | Key Issues |
|------|------|------------|-----------------|------------|
| AddTour.aspx / .cs | Web Forms Page | Medium | Pages/Tours/Create.cshtml | FileUpload, Server.MapPath, ADO.NET |
| AdminLogin2.aspx / .cs | Web Forms Page | Simple | Pages/Admin/Login.cshtml | Hardcoded credentials, no auth |
| AdminProfile.aspx / .cs | Web Forms Page | Simple | Pages/Admin/Index.cshtml | No auth protection |
| allbooking.aspx / .cs | Web Forms Page | Medium | Pages/Admin/Bookings.cshtml | SqlDataSource, GridView, DataVisualization |
| DisplayTours.aspx / .cs | Web Forms Page | Medium | Pages/Tours/Index.cshtml | SqlDataSource, GridView, Eval() |
| MainProfilePage.aspx / .cs | Web Forms Page | Simple | Pages/User/Index.cshtml | No auth, duplicated nav |
| mybooking.aspx / .cs | Web Forms Page | Medium | Pages/Bookings/Index.cshtml | SqlDataSource, GridView, delete |
| Order.aspx / .cs | Web Forms Page | Medium | Pages/Bookings/Create.cshtml | ADO.NET, Response.Write |
| SignUpForm.aspx / .cs | Web Forms Page | Medium | Pages/Account/Register.cshtml | Plain text password, ADO.NET |
| TourCrud.aspx / .cs | Web Forms Page | Complex | Pages/Admin/Tours/Index.cshtml | SqlDataSource, GridView, broken code |
| usercrud.aspx / .cs | Web Forms Page | Medium | Pages/Admin/Users/Index.cshtml | SqlDataSource, GridView, password display |
| userlogin.aspx / .cs | Web Forms Page | Medium | Pages/Account/Login.cshtml | SQL injection, plain text password |

---

## Migration Roadmap

### Phase 1: Foundation (Week 1-2) — ~20 hours
1. Create new .NET 8 solution with clean architecture (Domain, Application, Infrastructure, Web projects)
2. Convert project file to SDK-style format
3. Create `appsettings.json` with connection strings
4. Create `Program.cs` with middleware pipeline
5. Set up EF Core with `TourManagementDbContext`
6. Create domain entities: `Tour`, `UserInfo`, `Booking`
7. Create EF Core migrations from existing database schema

### Phase 2: Data Access Layer (Week 2-3) — ~20 hours
1. Create repository interfaces in Domain layer
2. Implement EF Core repositories in Infrastructure layer
3. Create service interfaces and implementations in Application layer
4. Set up AutoMapper profiles for DTO mappings
5. Configure dependency injection in `Program.cs`

### Phase 3: Authentication & Security (Week 3-4) — ~20 hours
1. Implement ASP.NET Core Identity
2. Create `IdentityDbContext` extending `TourManagementDbContext`
3. Implement password hashing for existing users
4. Create Login/Register Razor Pages with Identity
5. Add role-based authorization (Admin, User roles)
6. Protect admin pages with `[Authorize(Roles = "Admin")]`

### Phase 4: UI Migration (Week 4-6) — ~40 hours
1. Create `_Layout.cshtml` with Bootstrap 5 navigation
2. Migrate each .aspx page to Razor Page (.cshtml + PageModel)
3. Replace all server controls with HTML + Tag Helpers
4. Implement file upload with `IFormFile`
5. Add Data Annotations validation
6. Implement pagination for list pages

### Phase 5: Testing & Documentation (Week 6-7) — ~20 hours
1. Write unit tests for services
2. Write integration tests for repositories
3. Create `docs/MIGRATION_NOTES.md`
4. Create `docs/ARCHITECTURE.md`
5. Update `README.md`

---

## Target Architecture

```
TourManagement/
├── src/
│   ├── TourManagement.Domain/
│   │   ├── Entities/
│   │   │   ├── Tour.cs
│   │   │   ├── UserInfo.cs
│   │   │   └── Booking.cs
│   │   ├── Interfaces/
│   │   │   ├── Repositories/
│   │   │   │   ├── ITourRepository.cs
│   │   │   │   ├── IUserRepository.cs
│   │   │   │   └── IBookingRepository.cs
│   │   │   └── Services/
│   │   │       ├── ITourService.cs
│   │   │       ├── IUserService.cs
│   │   │       └── IBookingService.cs
│   │   └── Exceptions/
│   ├── TourManagement.Application/
│   │   ├── Services/
│   │   │   ├── TourService.cs
│   │   │   ├── UserService.cs
│   │   │   └── BookingService.cs
│   │   ├── DTOs/
│   │   │   ├── TourDto.cs
│   │   │   ├── UserDto.cs
│   │   │   └── BookingDto.cs
│   │   ├── Mappings/
│   │   │   └── MappingProfile.cs
│   │   └── Validators/
│   ├── TourManagement.Infrastructure/
│   │   ├── Data/
│   │   │   ├── TourManagementDbContext.cs
│   │   │   └── Configurations/
│   │   ├── Repositories/
│   │   │   ├── TourRepository.cs
│   │   │   ├── UserRepository.cs
│   │   │   └── BookingRepository.cs
│   │   └── Extensions/
│   │       └── ServiceCollectionExtensions.cs
│   └── TourManagement.Web/
│       ├── Pages/
│       │   ├── Account/
│       │   │   ├── Login.cshtml
│       │   │   └── Register.cshtml
│       │   ├── Admin/
│       │   │   ├── Index.cshtml
│       │   │   ├── Tours/
│       │   │   │   ├── Index.cshtml
│       │   │   │   ├── Create.cshtml
│       │   │   │   ├── Edit.cshtml
│       │   │   │   └── Delete.cshtml
│       │   │   └── Bookings.cshtml
│       │   ├── Tours/
│       │   │   └── Index.cshtml
│       │   ├── Bookings/
│       │   │   ├── Index.cshtml
│       │   │   └── Create.cshtml
│       │   └── Shared/
│       │       ├── _Layout.cshtml
│       │       └── _AdminLayout.cshtml
│       ├── wwwroot/
│       │   ├── css/site.css
│       │   ├── js/site.js
│       │   └── Tour_pics/
│       ├── Program.cs
│       └── appsettings.json
└── tests/
    ├── TourManagement.UnitTests/
    └── TourManagement.IntegrationTests/
```

---

## Required NuGet Packages (.NET 8)

### Infrastructure Project
```xml
<PackageReference Include="Microsoft.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.SqlServer" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
<PackageReference Include="Microsoft.Data.SqlClient" Version="5.2.0" />
```

### Application Project
```xml
<PackageReference Include="AutoMapper" Version="12.0.1" />
<PackageReference Include="FluentValidation" Version="11.9.0" />
```

### Web Project
```xml
<PackageReference Include="Microsoft.AspNetCore.Identity.EntityFrameworkCore" Version="8.0.0" />
<PackageReference Include="Serilog.AspNetCore" Version="8.0.0" />
<PackageReference Include="Microsoft.EntityFrameworkCore.Design" Version="8.0.0" />
```

### Packages to REMOVE
```xml
<!-- REMOVE: Not compatible with .NET 8 -->
<PackageReference Include="Microsoft.CodeDom.Providers.DotNetCompilerPlatform" Version="2.0.1" />
<!-- REMOVE: System.Web.DataVisualization — no .NET 8 equivalent -->
<!-- REMOVE: All System.Web.* references -->
```

---

## Code Migration Examples

### Example 1: AddTour.aspx → Pages/Admin/Tours/Create.cshtml

**Before (Web Forms):**
```csharp
// AddTour.aspx.cs
protected void Register_Click(object sender, EventArgs e)
{
    SqlConnection conn = new SqlConnection(ConfigurationManager.ConnectionStrings["dbconnection"].ConnectionString);
    conn.Open();
    string insertQuery = "insert into Tour(TOUR_NAME,PLACE,DAYS,PRICE,LOCATIONS,TOUR_INFO,pic) values(@TOUR_NAME,@PLACE,@DAYS,@PRICE,@LOCATIONS,@TOUR_INFO,@pic)";
    SqlCommand com = new SqlCommand(insertQuery, conn);
    com.Parameters.AddWithValue("@TOUR_NAME", tour_name.Text);
    FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + FileUpload1.FileName);
    com.Parameters.AddWithValue("@pic", FileUpload1.FileName);
    com.ExecuteNonQuery();
    Response.Write("ADD Successful");
    conn.Close();
}
```

**After (Razor Pages):**
```csharp
// Pages/Admin/Tours/Create.cshtml.cs
[Authorize(Roles = "Admin")]
public class CreateModel : PageModel
{
    private readonly ITourService _tourService;
    private readonly IWebHostEnvironment _env;
    private readonly ILogger<CreateModel> _logger;

    [BindProperty]
    public TourCreateViewModel Input { get; set; } = new();

    public CreateModel(ITourService tourService, IWebHostEnvironment env, ILogger<CreateModel> logger)
    {
        _tourService = tourService;
        _env = env;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        try
        {
            string? picFileName = null;
            if (Input.PicFile != null && Input.PicFile.Length > 0)
            {
                picFileName = Guid.NewGuid() + Path.GetExtension(Input.PicFile.FileName);
                var uploadPath = Path.Combine(_env.WebRootPath, "Tour_pics", picFileName);
                using var stream = new FileStream(uploadPath, FileMode.Create);
                await Input.PicFile.CopyToAsync(stream, cancellationToken);
            }
            var dto = new TourCreateDto
            {
                TourName = Input.TourName,
                Place = Input.Place,
                Days = Input.Days,
                Price = Input.Price,
                Locations = Input.Locations,
                TourInfo = Input.TourInfo,
                Pic = picFileName
            };
            await _tourService.CreateAsync(dto, cancellationToken);
            TempData["Success"] = "Tour added successfully.";
            return RedirectToPage("/Admin/Tours/Index");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating tour");
            ModelState.AddModelError(string.Empty, "An error occurred while saving the tour.");
            return Page();
        }
    }
}
```

### Example 2: userlogin.aspx → Pages/Account/Login.cshtml

**Before (Web Forms — SQL Injection + Plain Text Password):**
```csharp
string checkPasswordQuery = "select password from Userinfo where password='" + txtPassword.Text + "' and email = '" + txtEmail.Text + "'";
SqlCommand passComm = new SqlCommand(checkPasswordQuery, conn);
string password = passComm.ExecuteScalar()?.ToString() ?? "";
if (password == txtPassword.Text)
{
    Response.Redirect("MainProfilePage.aspx");
}
```

**After (Razor Pages with Identity):**
```csharp
// Pages/Account/Login.cshtml.cs
public class LoginModel : PageModel
{
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly ILogger<LoginModel> _logger;

    [BindProperty]
    public LoginViewModel Input { get; set; } = new();

    public LoginModel(SignInManager<ApplicationUser> signInManager, ILogger<LoginModel> logger)
    {
        _signInManager = signInManager;
        _logger = logger;
    }

    public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
    {
        if (!ModelState.IsValid) return Page();
        var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, lockoutOnFailure: true);
        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} logged in.", Input.Email);
            return RedirectToPage("/User/Index");
        }
        ModelState.AddModelError(string.Empty, "Invalid login attempt.");
        return Page();
    }
}
```

---

## Migration Readiness Assessment

| Category | Current State | Target State | Readiness |
|----------|--------------|--------------|-----------|
| Framework | .NET Framework 4.7.2 | .NET 8 | ❌ Not Ready |
| Project Format | Old MSBuild | SDK-Style | ❌ Not Ready |
| UI Layer | Web Forms (.aspx) | Razor Pages (.cshtml) | ❌ Not Ready |
| Data Access | Raw ADO.NET | EF Core 8 | ❌ Not Ready |
| Configuration | Web.config | appsettings.json | ❌ Not Ready |
| Authentication | Hardcoded/None | ASP.NET Core Identity | ❌ Not Ready |
| Security | SQL Injection, Plain Text Passwords | Parameterized/Hashed | ❌ Not Ready |
| Architecture | Monolithic Code-Behind | Clean Architecture | ❌ Not Ready |
| Logging | None | Serilog/ILogger | ❌ Not Ready |
| Testing | None | xUnit + Moq | ❌ Not Ready |
| Async/Await | None | Full Async | ❌ Not Ready |
| Dependency Injection | None | Built-in DI | ❌ Not Ready |

**Overall Migration Readiness: 15/100 — Requires Complete Rewrite**

The application requires a full migration effort. No existing code can be directly reused in .NET 8 without significant modification. The business logic (tour management, user registration, booking) is straightforward and well-understood, which will facilitate the migration despite the technical debt.
