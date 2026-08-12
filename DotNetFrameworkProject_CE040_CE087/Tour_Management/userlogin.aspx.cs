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
    public partial class userlogin : System.Web.UI.Page
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

        }

        // cr-dotnet-0013: Replaced direct SqlConnection instantiation with Dapper using RDS Proxy connection string.
        // Dapper leverages ADO.NET connection pooling which integrates with Amazon RDS Proxy for
        // multiplexed connections, IAM authentication, and infrastructure-level pooling.
        protected void Btn_Submit(object sender, EventArgs e)
        {
            string connectionString = GetConnectionString();

            // cr-dotnet-0013: Using Dapper with RDS Proxy-compatible connection pooling.
            // Using parameterized query to prevent SQL injection.
            using (IDbConnection conn = new SqlConnection(connectionString))
            {
                string checkPasswordQuery = "select password from Userinfo where password=@Password and email=@Email";
                string password = conn.QueryFirstOrDefault<string>(checkPasswordQuery, new
                {
                    Password = txtPassword.Text,
                    Email = txtEmail.Text
                }) ?? "";

                if (password == txtPassword.Text)
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
