<%@ Page Title="" 
    Language="C#"
    MasterPageFile="~/Site.Master" 
    AutoEventWireup="true" 
    CodeBehind="ChangePassword.aspx.cs" 
    Inherits="WebToolboxApp.Login.ChangePassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <asp:ScriptManagerProxy ID="ScriptManagerProxy1" runat="server">
        <Scripts>
            <%-- http://www-cs-students.stanford.edu/~tjw/jsbn/ (BSD LICENSE) --%>
            <asp:ScriptReference Path="~/Scripts/jsbn.js" />
            <asp:ScriptReference Path="~/Scripts/prng4.js" />
            <asp:ScriptReference Path="~/Scripts/rng.js" />
            <asp:ScriptReference Path="~/Scripts/rsa.js" />
            <asp:ScriptReference Path="~/Scripts/base64.js" />
        </Scripts>
    </asp:ScriptManagerProxy>
    <script type="text/javascript">
        function changePassword() {
            var newPass = $('#NEW_PASS').val();
            var newPassVerify = $('#NEW_PASS_VERIFY').val();

            if (newPass != newPassVerify) {
                alert('Invalid Password');
                return false;
            }

            var pubMod = $('#TxtPublicModules').val();
            var pubExp = $('#TxtPublicExponent').val();

            var rsa = new RSAKey();
            rsa.setPublic(pubMod, pubExp);

            var resNew = rsa.encrypt(newPass);

            if (resNew) {
                $('#ENCRYPTED_NEW_PASS').val(resNew);
                return true;
            }

            alert('ERROR');
            return false;
        }
    </script>

    <asp:HiddenField ID="TxtPublicModules" runat="server" ClientIDMode="Static" />
    <asp:HiddenField ID="TxtPublicExponent" runat="server" ClientIDMode="Static" />

    <h1>パスワードの変更</h1>
    <asp:Label ID="Message" runat="server" Text=""></asp:Label>
    <table border="1">
        <tr>
            <th>新しいパスワード</th>
            <%-- パスワードはサーバーに送信させない --%>
            <td><input type="password" id="NEW_PASS" /></td>
        </tr>
        <tr>
            <th>新しいパスワード(確認)</th>
            <%-- パスワードはサーバーに送信させない --%>
            <td><input type="password" id="NEW_PASS_VERIFY" /></td>
        </tr>
    </table>

    <asp:Button ID="BtnChangePassword" runat="server"
         Text="Update"
         OnClientClick="return changePassword()" 
         OnClick="BtnChangePassword_Click"
         style="height: 21px"/>

    <asp:HiddenField ID="ENCRYPTED_NEW_PASS" runat="server" ClientIDMode="Static" />
</asp:Content>
