<%@ Page Title="SQLSerberTblSizing"
     Language="C#"
     MasterPageFile="~/Site.Master"
     AutoEventWireup="true"
     CodeBehind="SQLServerTblSizing.aspx.cs"
     Inherits="WebToolboxApp.SQLServerTblSizing" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Literal ID="Literal1" runat="server" Text="SQLSerberTblSizing" /></h2>
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
        <div style="width: 100px; display:inline-block; text-align: right; margin-right: 1em;">NumOfRows:</div><asp:TextBox ID="TxtNumOfRows" runat="server" Width="250px"></asp:TextBox><br />
        <div style="width: 100px; display:inline-block; text-align: right; margin-right: 1em;">Fill_Factor:</div><asp:TextBox ID="TxtFillFactor" runat="server" Width="250px"></asp:TextBox><br />
    </div>
    <div>
        <asp:Button ID="BtnCalcurate" runat="server" Text="Convert" OnClick="BtnCalcurate_Click" />
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
    <p><a href="http://msdn.microsoft.com/ja-jp/library/ms178085.aspx">※ msdn:クラスター化インデックスのサイズの見積もり</a></p>
</asp:Content>
