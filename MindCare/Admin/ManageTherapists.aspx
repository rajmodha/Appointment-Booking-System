<%@ Page Title="Manage Therapists" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageTherapists.aspx.cs" Inherits="ManageTherapists" %>
<%@ Register Src="~/Admin/AdminSidebar.ascx" TagPrefix="uc" TagName="AdminSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:AdminSidebar ID="AdminSidebar1" runat="server" />

        <div class="mc-main">
            <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:16px;">
                <h2 style="color:var(--mc-primary-dark); margin:0;">Manage Therapists</h2>

                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="mc-form-control" style="width:auto;" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Text="All" Value="" />
                    <asp:ListItem Text="Pending" Value="Pending" Selected="True" />
                    <asp:ListItem Text="Approved" Value="Approved" />
                    <asp:ListItem Text="Rejected" Value="Rejected" />
                </asp:DropDownList>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Display="Dynamic" style="display:block; margin-bottom:12px;" />

            <asp:Repeater ID="rptTherapists" runat="server" OnItemCommand="rptTherapists_ItemCommand" OnItemDataBound="rptTherapists_ItemDataBound">
                <ItemTemplate>
                    <div class="mc-card" style="display:flex; justify-content:space-between; align-items:center; flex-wrap:wrap; gap:10px;">
                        <div>
                            <h4 style="margin:0 0 4px 0;"><%# Eval("FullName") %></h4>
                            <p style="margin:0; color:var(--mc-muted); font-size:14px;">
                                ✉️ <%# Eval("Email") %> &nbsp;|&nbsp; 📞 <%# Eval("Phone") %>
                            </p>
                            <p style="margin:4px 0 0 0; color:var(--mc-muted); font-size:14px;">
                                <%# Eval("Specialization") %> &middot; ₹<%# Eval("Fees") %>/session
                            </p>
                        </div>
                        <div style="text-align:right;">
                            <span class='mc-badge mc-badge-<%# Eval("ApprovalStatus").ToString().ToLower() == "approved" ? "confirmed" : Eval("ApprovalStatus").ToString().ToLower() %>' style="display:inline-block; margin-bottom:8px;">
                                <%# Eval("ApprovalStatus") %>
                            </span>
                            <br />
                            <asp:LinkButton ID="btnApprove" runat="server" CommandName="Approve" CommandArgument='<%# Eval("TherapistId") %>'
                                CssClass="mc-btn" style="font-size:13px; padding:6px 16px;">Approve</asp:LinkButton>
                            <asp:LinkButton ID="btnReject" runat="server" CommandName="Reject" CommandArgument='<%# Eval("TherapistId") %>'
                                CssClass="mc-btn mc-btn-outline" style="font-size:13px; padding:6px 16px;"
                                OnClientClick="return confirm('Reject this therapist application?');">Reject</asp:LinkButton>
                            <asp:LinkButton ID="btnToggleActive" runat="server" CommandName="ToggleActive" CommandArgument='<%# Eval("TherapistId") %>'
                                CssClass="mc-btn mc-btn-outline" style="font-size:13px; padding:6px 16px;"
                                OnClientClick='<%# "return confirm(\"" + (Eval("IsActive").ToString() == "True" ? "Disable" : "Enable") + " this account?\");" %>'>
                                <%# Eval("IsActive").ToString() == "True" ? "Disable Account" : "Enable Account" %>
                            </asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoResults" runat="server" Visible="false" class="mc-card" style="text-align:center;">
                <p style="color:var(--mc-muted);">No therapists found for this filter.</p>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
