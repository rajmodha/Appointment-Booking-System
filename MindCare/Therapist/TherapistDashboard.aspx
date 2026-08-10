<%@ Page Title="Therapist Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TherapistDashboard.aspx.cs" Inherits="TherapistDashboard" %>
<%@ Register Src="~/Therapist/TherapistSidebar.ascx" TagPrefix="uc" TagName="TherapistSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:TherapistSidebar ID="TherapistSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">Dashboard</h2>

            <!-- ============ STAT CARDS ============ -->
            <div style="display:flex; gap:16px; flex-wrap:wrap; margin-bottom:24px;">
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litPending" runat="server" /></div>
                    <div class="label">Pending Requests</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litUpcoming" runat="server" /></div>
                    <div class="label">Upcoming (Confirmed)</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litCompleted" runat="server" /></div>
                    <div class="label">Completed Sessions</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litRating" runat="server" /></div>
                    <div class="label">Average Rating</div>
                </div>
            </div>

            <!-- ============ PENDING REQUESTS PREVIEW ============ -->
            <div class="mc-card">
                <div style="display:flex; justify-content:space-between; align-items:center;">
                    <h3 style="margin:0; color:var(--mc-primary-dark);">Recent Requests</h3>
                    <a href="~/Therapist/Requests.aspx" runat="server">View all &rarr;</a>
                </div>

                <asp:Repeater ID="rptRecent" runat="server">
                    <ItemTemplate>
                        <div style="display:flex; justify-content:space-between; align-items:center; padding:12px 0; border-bottom:1px solid #eee;">
                            <div>
                                <b><%# Eval("PatientName") %></b>
                                <p style="margin:2px 0; color:var(--mc-muted); font-size:14px;">
                                    <%# Eval("AppointmentDate", "{0:dd MMM yyyy}") %> &middot; <%# Eval("DisplayTime") %> &middot; <%# Eval("ConsultationType") %>
                                </p>
                            </div>
                            <span class='mc-badge mc-badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoRequests" runat="server" Visible="false" Text="No appointment requests yet." style="color:var(--mc-muted);" />
            </div>
        </div>
    </div>
</asp:Content>
