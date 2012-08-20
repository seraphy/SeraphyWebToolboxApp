<%@ Page Title="Login"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="Login.aspx.cs"
    Inherits="WebToolboxApp.Login.Login"
    %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
    <%-- http://code.google.com/p/crypto-js/ --%>
<script type="text/javascript" src="http://crypto-js.googlecode.com/svn/tags/3.0.2/build/rollups/sha1.js"></script>
<script type="text/javascript" src="http://crypto-js.googlecode.com/svn/tags/3.0.2/build/components/enc-base64-min.js"></script>
<script type="text/javascript">
    function convPassword() {
        var pwd = document.getElementById('password').value;
        var salt = document.getElementById('SALT').value;
        var hash = CryptoJS.SHA1(salt + '@' + pwd);
        var strHash = hash.toString(CryptoJS.enc.Base64);
        document.getElementById('HashedPassword').value = strHash;
    }
</script>
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
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
