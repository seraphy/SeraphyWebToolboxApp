<%@ Page Title="Admin | LogView"
    Language="C#"
    MasterPageFile="~/Site.Master"
    AutoEventWireup="true"
    CodeBehind="LogView.aspx.cs"
    Inherits="WebToolboxApp.Admin.LogView"
    %>

<asp:Content ID="Content1" ContentPlaceHolderID="HeadContent" runat="server">
</asp:Content>

<asp:Content ID="Content2" ContentPlaceHolderID="MainContent" runat="server">
    <h1>LogView</h1>
    <asp:ListView ID="FileListView" runat="server" 
        onitemcommand="FileListView_ItemCommand">
        <LayoutTemplate>
            <div ID="itemPlaceholderContainer" runat="server" style="">
                <ul>
                    <div ID="itemPlaceholder" runat="server" />
                </ul>
            </div>
        </LayoutTemplate>
        <ItemTemplate>
            <li>
                <asp:LinkButton
                    ID="FileLinkButton"
                    runat="server"
                    CommandName="Select"
                    CommandArgument='<%# Eval("Name") %>'
                    >
                    <asp:Label ID="FileNameLabel" runat="server" Text='<%# Eval("Name") %>' />
                    &nbsp;
                    (<asp:Label ID="Label1" runat="server" Text='<%# Eval("LastWriteTime ","{0:D}") %>' />)
                    &nbsp;
                    (<asp:Label ID="Label2" runat="server" Text='<%# Eval("Length ") %>' />)
                </asp:LinkButton>
            </li>
        </ItemTemplate>
    </asp:ListView>

    <div style="margin-top: 1em;">
        <asp:Button 
            ID="PurgeButton"
            runat="server"
            Text="Purge"
            oncommand="PurgeButton_Command"
            OnClientClick="return confirm('削除してもよろしいですか?')"
            />
    </div>
</asp:Content>
