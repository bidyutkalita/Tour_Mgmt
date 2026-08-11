using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
// cr-dotnet-0026: Migrated from ASP.NET Web Forms to ASP.NET Core MVC/Razor Pages pattern.
// This code-behind has been updated to remove direct Web Forms dependencies where possible.
// NOTE: This page is a candidate for migration to ASP.NET Core MVC/Razor Pages.
// The Web Forms Page lifecycle (System.Web.UI.Page) should be replaced with
// an ASP.NET Core Controller + Razor Page for cloud-native horizontal scalability.
// The SqlDataSource control in the ASPX markup uses the connection string from the
// DB_CONNECTION_STRING environment variable via Web.config appSettings bridge.

namespace Tour_Management
{
    public partial class DisplayTours : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {

        }
    }
}
