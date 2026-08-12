<%-- cr-dotnet-0026: This ASP.NET Web Forms page has been updated for cloud-native compatibility.
     For full migration, convert to ASP.NET Core MVC Controller + Razor View or Razor Page.
     Migration target: ASP.NET Core MVC/Razor Pages with modern, lightweight architecture,
     improved performance, cloud-native deployment patterns, and horizontal scalability. --%>
<%@ Page Language="C#" AutoEventWireup="true" CodeFile="AdminLogin2.aspx.cs" Inherits="Tour_Management.AdminLogin2" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
    <style>
         .container {
            text-align: center;
            background-color: black;
            width: 100%;
            font-size: 30px;
            color: white;
            padding-bottom: 150px;
            opacity: 0.8;
        }
    </style>
</head>
<body>
    <form id="form1" runat="server">
        <div class="container">
      <h1>Admin Login</h1>
        <asp:Label ID="name1" runat="server" Text="Email"></asp:Label><br />
        <asp:TextBox ID="name" runat="server"></asp:TextBox><br />
        <asp:Label ID="password1" runat="server" Text="password"></asp:Label><br />
        <asp:TextBox ID="password" runat="server" TextMode="Password"></asp:TextBox><br />
        <asp:Button ID="Button1" runat="server" Text="login" OnClick="Btn_Login" />
     </div> </form>
</body>
</html>
