<%@ Page Title="SqlEnclosure"
     Language="C#"
     MasterPageFile="~/Site.Master"
     AutoEventWireup="true"
     CodeBehind="SqlEnclosure.aspx.cs"
     Inherits="WebToolboxApp.SqlEnclosure" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Literal ID="Literal1" runat="server" Text="SqlEnclosure" /></h2>
    <div>
        <asp:TextBox
            ID="TxtSource"
            runat="server"
            TextMode="MultiLine"
            Columns="100"
            Rows="15"
            ></asp:TextBox>
    </div>
    <div>
        <div style="width: 100px; display:inline-block; text-align: right; margin-right: 1em;">Pattern:</div><asp:TextBox ID="TxtPattern" runat="server" Width="250px"></asp:TextBox><br />
        <div style="width: 100px; display:inline-block; text-align: right; margin-right: 1em;">Replace Chars:</div><asp:TextBox ID="TxtReplaceChars" runat="server" Width="100px"></asp:TextBox>
    </div>
    <div>
        <asp:Button ID="BtnConvert" runat="server" Text="Convert" OnClick="BtnConvert_Click" />
        <asp:Button ID="BtnDeconvert" runat="server" Text="Convert" OnClick="BtnDeconvert_Click" />
        <asp:Button ID="BtnClear" runat="server" Text="Clear" OnClick="BtnClear_Click" />
    </div>
    <div>
        <asp:TextBox
            ID="TxtResult"
            runat="server"
            TextMode="MultiLine"
            Columns="100"
            Rows="15"
          ></asp:TextBox>
    </div>
</asp:Content>
