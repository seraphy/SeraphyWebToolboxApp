<%@ Page Title="GenRandom"
     Language="C#"
     MasterPageFile="~/Site.Master"
     AutoEventWireup="true" 
     CodeBehind="GenRandom.aspx.cs"
     Inherits="WebToolboxApp.Admin.GenRandom" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>LogView</h1>
    <fieldset>
        <legend>ランダムファイル生成</legend>
        <div style="width: 100px; display: inline-block; margin-right: 1em;">生成サイズ:</div>
        <asp:TextBox ID="TxtFileSize" runat="server" Text="1024" Width="80px"></asp:TextBox>
        <asp:Button ID="BtnGenerateRaw" runat="server" Text="RAW" OnClick="BtnGenerateRaw_Click" />
        <asp:Button ID="BtnGenerateBase64" runat="server" Text="BASE64" OnClick="BtnGenerate_Click" />
    </fieldset>
</asp:Content>
