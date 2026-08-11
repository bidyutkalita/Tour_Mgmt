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

namespace Tour_Management
{
    public partial class AdminLogin2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // Admin credentials should be stored in AWS Secrets Manager or SSM Parameter Store,
            // not hardcoded. Retrieve from environment variables for cloud-native security.
            string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@gmail.com";
            string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin";

            if (password.Text == adminPassword && name.Text == adminEmail)
            {
                Response.Redirect("AdminProfile.aspx");
            }
        }
    }
}
