<%@ Page Title="Login"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="WebToolboxApp.Login.Login"
    %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">

    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <%-- https://github.com/wwwtyro/cryptico (NEW BSD LICENSE) --%>
            <asp:ScriptReference Path="~/Scripts/cryptico.min.js" />
        </Scripts>
    </asp:ScriptManagerProxy>

    <script type="text/javascript">
        function convPassword() {
            var pwd = $('#password').val();
            var salt = $('#SALT').val();
            var strHash = SHA1(salt + '@' + pwd);
            $('#HashedPassword').val(strHash);
        }
    </script>
    <asp:HiddenField ID="SALT" ClientIDMode="Static" runat="server" />
    <asp:HiddenField ID="HashedPassword" ClientIDMode="Static" runat="server" />
    <table>
        <tr>
            <td>User</td>
            <td>
                <asp:TextBox ID="UserName" runat="server" Columns="20"></asp:TextBox>
            </td>
        </tr>
        <tr>
            <td>password</td>
            <td>
                <%-- パスワードはサーバーに送信させない --%>
                <input id="password" type="password" size="20"/>
            </td>
        </tr>
        <tr>
            <td colspan="2">
                <asp:Button
                    ID="BtnLogin"
                    runat="server"
                    Text="Login"
                    OnClientClick="convPassword()"
                    />
            </td>
        </tr>
    </table>
</asp:Content>
