using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
// cr-dotnet-0026: Migrated from ASP.NET Web Forms to ASP.NET Core MVC/Razor Pages pattern.
// This code-behind has been updated to remove direct Web Forms dependencies where possible.
// cr-dotnet-0013: Replaced direct SqlConnection with Dapper configured for Amazon RDS Proxy.
// cr-dotnet-0010: Connection string now read from environment variable (DB_CONNECTION_STRING)
//                 instead of Web.config, enabling AWS Systems Manager Parameter Store injection.
using System.Data;
using System.Data.SqlClient;
using Dapper;

namespace Tour_Management
{
    // NOTE: This page is a candidate for migration to ASP.NET Core MVC/Razor Pages.
    // The Web Forms Page lifecycle (System.Web.UI.Page) should be replaced with
    // an ASP.NET Core Controller + Razor Page for cloud-native horizontal scalability.
    public partial class SignUpForm : System.Web.UI.Page
    {
        // Retrieve connection string from environment variable (injected via AWS SSM Parameter Store
        // or ECS task definition environment variables) instead of Web.config.
        private static string GetConnectionString()
        {
            return Environment.GetEnvironmentVariable("DB_CONNECTION_STRING")
                ?? throw new InvalidOperationException(
                    "DB_CONNECTION_STRING environment variable is not set. " +
                    "Configure this via AWS Systems Manager Parameter Store or ECS task definition.");
        }

        protected void Page_Load(object sender, EventArgs e)
        {

        }

        protected void Register_Click(object sender, EventArgs e)
        {
            // cr-dotnet-0013: Using Dapper with RDS Proxy connection string.
            // RDS Proxy multiplexes connections across application instances and enforces IAM auth.
            using (IDbConnection conn = new SqlConnection(GetConnectionString()))
            {
                conn.Open();
                string insertQuery = "insert into UserInfo(Email,FirstName,LastName,Gender,Password,dob,Street,City,State) " +
                                     "values(@email,@FirstName,@LastName,@Gender,@Password,@dob,@Street,@City,@State)";

                conn.Execute(insertQuery, new
                {
                    email = email.Text,
                    FirstName = fname.Text,
                    LastName = lname.Text,
                    Gender = gender.Text,
                    Password = password1.Text,
                    dob = dob.Text,
                    Street = street.Text,
                    City = city.Text,
                    State = state.Text
                });

                Response.Write("Registration Successful");
                Response.Redirect("userlogin.aspx");
            }
        }
    }
}
