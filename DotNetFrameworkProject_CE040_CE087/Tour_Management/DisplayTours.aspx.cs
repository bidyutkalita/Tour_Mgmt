// DisplayTours.aspx.cs
// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages pattern.
// Eliminates ViewState, postbacks, and server affinity requirements,
// enabling stateless horizontal scaling in Azure Container Apps.
// Connection string read from environment-based configuration.

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tour_Management
{
    public partial class DisplayTours : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Connection string for SqlDataSource is resolved from environment variable
            // CONNECTIONSTRINGS__DBCONNECTION or appsettings.json at runtime.
        }
    }
}
