<%@ Page Language="C#"
    AutoEventWireup="true"
    CodeBehind="ConvColumnName.aspx.cs"
    MasterPageFile="~/Site.master"
    Inherits="WebToolboxApp.ConvColumnName"
    %>

<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2><asp:Literal ID="Literal1" runat="server" Text="<%$ Resources: Title %>" /></h2>
    <div>
        <asp:TextBox
            ID="TxtDBColumns"
            runat="server"
            TextMode="MultiLine"
            Columns="100"
            Rows="20"
            ></asp:TextBox>
    </div>
    <div>
        <asp:Button ID="BtnConvert" runat="server" Text="Convert" 
            onclick="BtnConvert_Click" />
        <asp:Button ID="BtnClear" runat="server" Text="Clear" 
            onclick="BtnClear_Click" />
        <asp:CheckBox ID="InitialMode" runat="server" Text="<%$ Resources: Txt.InitialMode %>" Checked="true"/>
    </div>
    <div>
        <asp:TextBox
            ID="TxtConvColumns"
            runat="server"
            TextMode="MultiLine"
            Columns="100"
            Rows="20"
            Visible="false"
          ></asp:TextBox>
    </div>
</asp:Content>
