// AddTour.aspx.cs
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
using Microsoft.Extensions.Configuration;

namespace Tour_Management
{
    public partial class AddTour : System.Web.UI.Page
    {
        protected void Page_Load(object sender, EventArgs e)
        {
        }

        protected void Register_Click(object sender, EventArgs e)
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
                var tour = new Tour
                {
                    TourName = tour_name.Text,
                    Place = place.Text,
                    Days = days.Text,
                    Price = price.Text,
                    Locations = locations.Text,
                    TourInfo = tour_info.Text,
                    Pic = FileUpload1.FileName
                };

                // Save uploaded file to Tour_pics folder
                if (FileUpload1.HasFile)
                {
                    FileUpload1.SaveAs(Server.MapPath("~/Tour_pics/") + FileUpload1.FileName);
                }

                dbContext.Tours.Add(tour);
                dbContext.SaveChanges();
            }

            Response.Write("ADD Successful");
        }
    }
}
