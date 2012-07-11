<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="ConvInsertDML.aspx.cs" MasterPageFile="~/Site.master" Inherits="WebToolboxApp.ConvInsertDML" %>
<asp:Content ID="HeaderContent" runat="server" ContentPlaceHolderID="HeadContent">
</asp:Content>

<asp:Content ID="BodyContent" runat="server" ContentPlaceHolderID="MainContent">
    <h2>タブ区切りテキストからのINSERT文生成</h2>
    <div>
        <div>
            <div style="width: 100px; text-align: right; margin-right: 20px; float: left;">TableName:</div>
            <asp:TextBox ID="TxtTableName" runat="server" meta:resourcekey="TxtTableName"></asp:TextBox>
        </div>
        <div>
            <div style="width: 100px; text-align: right; margin-right: 20px; float: left;">Columns:</div>
            <asp:TextBox ID="TxtColumns" runat="server" TextMode="MultiLine" meta:resourcekey="TxtColumns"></asp:TextBox>
        </div>
        <div>
            <div style="width: 100px; text-align: right; margin-right: 20px; float: left;">Data Rows:</div>
            <asp:TextBox ID="TxtDataRows" runat="server" TextMode="MultiLine" meta:resourcekey="TxtDataRows"></asp:TextBox>
        </div>
        <div>
            <div style="width: 100px; text-align: right; margin-right: 20px; float: left;">&nbsp;</div>
            <asp:CheckBox ID="CheckTabOnly" runat="server" Checked="true" meta:resourcekey="CheckTabOnly"/>
        </div>
    </div>
    <div>
        <asp:Button ID="BtnGenerate" runat="server" meta:resourcekey="BtnGenerate"
            onclick="BtnGenerate_Click" />
        <asp:Button ID="BtnClear" runat="server" meta:resourcekey="BtnClear" 
            onclick="BtnClear_Click"/>
    </div>
    <div>
        <asp:TextBox Columns="100" ID="TxtSQL" runat="server" Rows="20" TextMode="MultiLine" Visible="false"></asp:TextBox>
    </div>
</asp:Content>
