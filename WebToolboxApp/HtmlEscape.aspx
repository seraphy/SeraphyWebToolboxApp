<%@ Page Title="HtmlEscape"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="HtmlEscape.aspx.cs"
    Inherits="WebToolboxApp.HtmlEscape"
    validateRequest="false"
    %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h2><asp:Literal ID="Literal1" runat="server" Text="HtmlEscape" /></h2>
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
        <span>Tab Size:<asp:TextBox ID="TxtTabSize" Width="50" runat="server"></asp:TextBox></span>
        <span>Mode: <asp:DropDownList ID="DrpConvertMode" runat="server">
            <asp:ListItem>None</asp:ListItem>
            <asp:ListItem>Simple</asp:ListItem>
            <asp:ListItem>Full</asp:ListItem>
            <asp:ListItem>XML</asp:ListItem>
        </asp:DropDownList></span>
        <asp:Button ID="BtnCalcurate" runat="server" Text="Convert" OnClick="BtnCalcurate_Click" />
        <asp:Button ID="BtnClear" runat="server" Text="Clear" OnClick="BtnClear_Click" />
    </div>
    <div>
        ※ タブサイズが0の場合はタブ変換はしない.
        ※ (非ASCIIは全角とみなす。半角カタカナは無視する)
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
