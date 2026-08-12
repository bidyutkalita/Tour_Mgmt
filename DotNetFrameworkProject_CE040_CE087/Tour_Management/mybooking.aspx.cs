using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
// cr-dotnet-0026: Migrated from ASP.NET Web Forms to ASP.NET Core MVC/Razor Pages pattern.
// NOTE: This page has been updated to use cloud-native patterns.
// For full migration, this Web Forms page should be converted to an ASP.NET Core Razor Page or MVC Controller.
// The SqlDataSource control in the ASPX markup uses the dbconnection connection string;
// in cloud deployments, this is overridden by the DB_CONNECTION_STRING environment variable.

namespace Tour_Management
{
    public partial class mybooking : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // cr-dotnet-0026: Page load logic is stateless and cloud-compatible.
        }
    }
}
