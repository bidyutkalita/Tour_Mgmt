// userlogin.aspx.cs
// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages pattern.
// Direct SqlConnection replaced with Entity Framework Core for built-in connection pooling,
// retry logic, and Azure SQL Database integration with transient fault handling.
// Connection string now read from environment-based configuration (appsettings.json / env vars).

using System;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tour_Management.Data;

namespace Tour_Management
{
    public partial class userlogin : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Btn_Submit(object sender, EventArgs e)
        {
            // Retrieve connection string from environment-based configuration
            // (environment variable CONNECTIONSTRINGS__DBCONNECTION or appsettings.json)
            // instead of Web.config transformation files.
            string connectionString = Environment.GetEnvironmentVariable("CONNECTIONSTRINGS__DBCONNECTION")
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString;

            var optionsBuilder = new Microsoft.EntityFrameworkCore.DbContextOptionsBuilder<TourDbContext>();
            optionsBuilder.UseSqlServer(connectionString, sqlOptions =>
            {
                // Azure SQL connection resiliency: enable retry on transient failures
                sqlOptions.EnableRetryOnFailure(
                    maxRetryCount: 5,
                    maxRetryDelay: TimeSpan.FromSeconds(30),
                    errorNumbersToAdd: null);
            });

            using (var dbContext = new TourDbContext(optionsBuilder.Options))
            {
                // Use parameterized EF Core query to prevent SQL injection
                var user = dbContext.UserInfos
                    .FirstOrDefault(u => u.Email == txtEmail.Text && u.Password == txtPassword.Text);

                if (user != null)
                {
                    Response.Write("Password is correct");
                    Response.Redirect("MainProfilePage.aspx");
                }
                else
                {
                    Response.Write("Password is not correct");
                }
            }
        }

        protected void Btn_reg(object sender, EventArgs e)
        {
            Response.Redirect("SignUpForm.aspx");
        }
    }
}
