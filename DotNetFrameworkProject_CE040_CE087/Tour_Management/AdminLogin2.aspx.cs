// This file has been migrated to ASP.NET Core Razor Pages.
// New location: Pages/Admin/AdminLogin.cshtml.cs
//
// Changes applied:
// - cr-dotnet-0026: Replaced ASP.NET Web Forms (System.Web.UI.Page) with ASP.NET Core Razor Pages (PageModel)
//   Eliminates ViewState, postbacks, and server affinity requirements for stateless horizontal scaling.
// - cr-dotnet-0010: Admin credentials moved from hardcoded values to environment variables
//   (ADMIN_EMAIL, ADMIN_PASSWORD) / IConfiguration for runtime configuration.

using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tour_Management
{
    // MIGRATED: See Pages/Admin/AdminLogin.cshtml.cs for the ASP.NET Core Razor Pages implementation.
    public partial class AdminLogin2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // MIGRATED: Hardcoded credential check moved to Pages/Admin/AdminLogin.cshtml.cs
            // Credentials now read from environment variables (cr-dotnet-0010)
        }
    }
}
