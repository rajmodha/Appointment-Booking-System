<%@ Page Title="Admin Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AdminDashboard.aspx.cs" Inherits="AdminDashboard" %>
<%@ Register Src="~/Admin/AdminSidebar.ascx" TagPrefix="uc" TagName="AdminSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:AdminSidebar ID="AdminSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">System Overview</h2>

            <!-- ============ STAT CARDS ============ -->
            <div style="display:flex; gap:16px; flex-wrap:wrap; margin-bottom:24px;">
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litTotalPatients" runat="server" /></div>
                    <div class="label">Total Patients</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litTotalTherapists" runat="server" /></div>
                    <div class="label">Approved Therapists</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litPendingApprovals" runat="server" /></div>
                    <div class="label">Pending Approvals</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litPendingPayments" runat="server" /></div>
                    <div class="label">Payments to Verify</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litRefundsPending" runat="server" /></div>
                    <div class="label">Refunds Pending</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litUnreadMessages" runat="server" /></div>
                    <div class="label">Unread Messages</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litTotalAppointments" runat="server" /></div>
                    <div class="label">Total Appointments</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litTotalRevenue" runat="server" /></div>
                    <div class="label">Revenue Collected (₹)</div>
                </div>
            </div>

            <!-- ============ PENDING THERAPIST APPROVALS PREVIEW ============ -->
            <div class="mc-card">
                <div style="display:flex; justify-content:space-between; align-items:center;">
                    <h3 style="margin:0; color:var(--mc-primary-dark);">Pending Therapist Approvals</h3>
                    <a href="~/Admin/ManageTherapists.aspx" runat="server">Manage all &rarr;</a>
                </div>

                <asp:Repeater ID="rptPending" runat="server">
                    <ItemTemplate>
                        <div style="display:flex; justify-content:space-between; align-items:center; padding:12px 0; border-bottom:1px solid #eee;">
                            <div>
                                <b><%# Eval("FullName") %></b>
                                <p style="margin:2px 0; color:var(--mc-muted); font-size:14px;"><%# Eval("Email") %></p>
                            </div>
                            <span class="mc-badge mc-badge-pending">Pending</span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoPending" runat="server" Visible="false" Text="No pending therapist approvals right now." style="color:var(--mc-muted);" />
            </div>
        </div>
    </div>
</asp:Content>
