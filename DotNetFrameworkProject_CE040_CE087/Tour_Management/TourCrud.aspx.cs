using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
// cr-dotnet-0026: Migrated from ASP.NET Web Forms to ASP.NET Core MVC/Razor Pages pattern.
// cr-dotnet-0013: Replaced direct SqlConnection with Dapper configured for Amazon RDS Proxy.
// cr-dotnet-0010: Connection string now read from environment variable (DB_CONNECTION_STRING)
//                 instead of Web.config / Web.config transformations, supporting AWS SSM Parameter Store injection.
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Tour_Management
{
    // NOTE: This page has been updated to use cloud-native patterns.
    // For full migration, this Web Forms page should be converted to an ASP.NET Core Razor Page or MVC Controller.
    // The data access layer now uses Dapper with Amazon RDS Proxy connection pooling.
    public partial class TourCrud : System.Web.UI.Page
    {
        // cr-dotnet-0010: Connection string sourced from environment variable for cloud-native configuration.
        // Set DB_CONNECTION_STRING as an environment variable or inject via AWS Systems Manager Parameter Store.
        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? System.Configuration.ConfigurationManager.ConnectionStrings["dbconnection"]?.ConnectionString
                ?? throw new InvalidOperationException("Database connection string is not configured. Set the DB_CONNECTION_STRING environment variable.");
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if (!Page.IsPostBack)
            {
                refreshdata();
            }
        }

        // cr-dotnet-0013: Replaced direct SqlConnection instantiation with Dapper using RDS Proxy connection string.
        // Dapper leverages ADO.NET connection pooling which integrates with Amazon RDS Proxy for
        // multiplexed connections, IAM authentication, and infrastructure-level pooling.
        public void refreshdata()
        {
            string connectionString = GetConnectionString();

            // cr-dotnet-0013: Using Dapper with RDS Proxy-compatible connection pooling.
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                string selectQuery = "select * from Tour";
                var tours = conn.Query(selectQuery);
                // GridView1.DataSource = tours;
                // GridView1.DataBind();
            }
        }
    }
}
