<%@ Page Title="Contact Messages" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ContactMessages.aspx.cs" Inherits="Admin_ContactMessages" %>
<%@ Register Src="~/Admin/AdminSidebar.ascx" TagPrefix="uc" TagName="AdminSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:AdminSidebar ID="AdminSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">Contact Messages</h2>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Display="Dynamic" style="display:block; margin-bottom:12px;" />

            <asp:Repeater ID="rptMessages" runat="server" OnItemCommand="rptMessages_ItemCommand">
                <ItemTemplate>
                    <div class="mc-card" style='<%# Eval("IsRead").ToString() == "False" ? "border-left:4px solid var(--mc-primary);" : "" %>'>
                        <div style="display:flex; justify-content:space-between; flex-wrap:wrap; gap:10px;">
                            <div>
                                <h4 style="margin:0 0 4px 0;">
                                    <%# Eval("Subject") %>
                                    <asp:Literal runat="server" Visible='<%# Eval("IsRead").ToString() == "False" %>' Text=" <span class='mc-badge mc-badge-pending'>New</span>" />
                                </h4>
                                <p style="margin:0; color:var(--mc-muted); font-size:14px;">
                                    <%# Eval("FullName") %> &middot; <%# Eval("Email") %> &middot; <%# Eval("SubmittedOn", "{0:dd MMM yyyy, h:mm tt}") %>
                                </p>
                                <p style="margin:10px 0 0 0; white-space:pre-wrap;"><%# Eval("Message") %></p>
                            </div>
                            <div style="text-align:right; min-width:120px;">
                                <asp:LinkButton ID="btnMarkRead" runat="server" CommandName="MarkRead" CommandArgument='<%# Eval("MessageId") %>'
                                    CssClass="mc-btn" style="font-size:13px; padding:6px 16px;"
                                    Visible='<%# Eval("IsRead").ToString() == "False" %>'>Mark as Read</asp:LinkButton>

                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CommandArgument='<%# Eval("MessageId") %>'
                                    CssClass="mc-btn mc-btn-outline" style="font-size:13px; padding:6px 16px; margin-top:6px; display:inline-block;"
                                    OnClientClick="return confirm('Delete this message permanently?');">Delete</asp:LinkButton>
                            </div>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoMessages" runat="server" Visible="false" class="mc-card" style="text-align:center;">
                <p style="color:var(--mc-muted);">No contact messages yet.</p>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
