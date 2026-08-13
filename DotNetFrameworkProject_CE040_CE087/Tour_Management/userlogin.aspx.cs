// This file has been migrated to ASP.NET Core Razor Pages.
// New location: Pages/User/Login.cshtml.cs
//
// Changes applied:
// - cr-dotnet-0026: Replaced ASP.NET Web Forms (System.Web.UI.Page) with ASP.NET Core Razor Pages (PageModel)
//   Eliminates ViewState, postbacks, and server affinity requirements for stateless horizontal scaling.
// - cr-dotnet-0013: Replaced direct SqlConnection management with Entity Framework Core DbContext,
//   enabling built-in connection pooling, retry logic, and Azure SQL Database integration
//   with transient fault handling. Also fixed SQL injection vulnerability (raw string concatenation).
// - cr-dotnet-0010: Replaced Web.config / ConfigurationManager.ConnectionStrings with
//   ASP.NET Core IConfiguration reading from appsettings.{Environment}.json and
//   environment variables (CONNECTIONSTRINGS__DBCONNECTION) for runtime configuration.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tour_Management
{
    // MIGRATED: See Pages/User/Login.cshtml.cs for the ASP.NET Core Razor Pages implementation.
    public partial class userlogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Btn_Submit(object sender, EventArgs e)
        {
            // MIGRATED to EF Core in Pages/User/Login.cshtml.cs (OnPostAsync)
            // Direct SqlConnection replaced with TourManagementDbContext (cr-dotnet-0013)
            // SQL injection vulnerability fixed with EF Core parameterized queries
            // ConfigurationManager replaced with IConfiguration / environment variables (cr-dotnet-0010)
        }

        protected void Btn_reg(object sender, EventArgs e)
        {
            // MIGRATED: Redirect handled in Pages/User/Login.cshtml
        }
    }
}
