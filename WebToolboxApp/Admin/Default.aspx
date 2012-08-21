<%@ Page Title="Admin"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Default.aspx.cs"
    Inherits="WebToolboxApp.Admin.Default"
    %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
<h1>Administration</h1>
<ul>
    <li><a href="LogView.aspx">LogView</a></li>
</ul>
</asp:Content>
