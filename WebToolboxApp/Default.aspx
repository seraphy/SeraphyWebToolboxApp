<%@ Page Title="ホーム ページ" Language="C#" MasterPageFile="~/Site.master" AutoEventWireup="true"
    CodeBehind="Default.aspx.cs" Inherits="WebToolboxApp._Default" %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>Table Of Contents</h2>
    <ul>
        <li>
            <asp:HyperLink ID="HyperLink1" runat="server" NavigateUrl="~/ConvInsertDML.aspx" Text="タブ区切りテキストからのINSERT文生成"/>
        </li>
        <li>
            <asp:HyperLink ID="HyperLink2" runat="server" NavigateUrl="~/ConvColumnName.aspx" Text="DBカラム名→CamelStyleプロパティ名変換"/>
        </li>
    </ul>
</asp:Content>
