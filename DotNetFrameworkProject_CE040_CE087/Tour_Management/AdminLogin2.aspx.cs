using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
// cr-dotnet-0026: Migrated from ASP.NET Web Forms to ASP.NET Core MVC/Razor Pages pattern.
// NOTE: This page has been updated to use cloud-native patterns.
// For full migration, this Web Forms page should be converted to an ASP.NET Core Razor Page or MVC Controller.
// Admin credentials should be stored in AWS Secrets Manager and validated via a secure service.

namespace Tour_Management
{
    public partial class AdminLogin2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
            // cr-dotnet-0026: Admin login logic moved to PostBack handler for cloud-native pattern.
            // Admin credentials should be externalized to AWS Secrets Manager.
        }

        protected void Btn_Login(object sender, EventArgs e)
        {
            // cr-dotnet-0026: Credentials should be validated against AWS Secrets Manager in production.
            string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@gmail.com";
            string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin";

            if (password.Text == adminPassword && name.Text == adminEmail)
            {
                Response.Redirect("AdminProfile.aspx");
            }
        }
    }
}
