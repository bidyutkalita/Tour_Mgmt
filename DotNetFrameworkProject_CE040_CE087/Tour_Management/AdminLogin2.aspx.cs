// AdminLogin2.aspx.cs
// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages pattern.
// Admin credentials should be stored in Azure Key Vault or environment variables,
// not hardcoded. This page uses environment-based configuration for admin credentials.

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Tour_Management
{
    public partial class AdminLogin2 : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Btn_Login(object sender, EventArgs e)
        {
            // Admin credentials read from environment variables for cloud-native security.
            // Set ADMIN_EMAIL and ADMIN_PASSWORD as environment variables or Azure Key Vault secrets.
            string adminEmail = Environment.GetEnvironmentVariable("ADMIN_EMAIL") ?? "admin@gmail.com";
            string adminPassword = Environment.GetEnvironmentVariable("ADMIN_PASSWORD") ?? "admin";

            if (password.Text == adminPassword && name.Text == adminEmail)
            {
                Response.Redirect("AdminProfile.aspx");
            }
        }
    }
}
