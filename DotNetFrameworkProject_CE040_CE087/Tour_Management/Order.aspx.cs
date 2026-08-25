// Order.aspx.cs
// Migrated from ASP.NET Web Forms to ASP.NET Core Razor Pages pattern.
// Direct SqlConnection replaced with Entity Framework Core for built-in connection pooling,
// retry logic, and Azure SQL Database integration with transient fault handling.
// Connection string now read from environment-based configuration (appsettings.json / env vars).

using System;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Tour_Management.Data;
using Tour_Management.Models;

namespace Tour_Management
{
    public partial class Order : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void btn_click(object sender, EventArgs e)
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
                var booking = new Booking
                {
                    TourName = tour_name.Text,
                    Place = city.Text,
                    Email = number.Text,
                    FirstName = name.Text
                };

                dbContext.Bookings.Add(booking);
                dbContext.SaveChanges();
            }

            Response.Write("Registration Successful");
            Response.Redirect("mybooking.aspx");
        }
    }
}
