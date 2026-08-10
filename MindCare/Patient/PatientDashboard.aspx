<%@ Page Title="My Dashboard" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="PatientDashboard.aspx.cs" Inherits="PatientDashboard" %>
<%@ Register Src="~/Patient/PatientSidebar.ascx" TagPrefix="uc" TagName="PatientSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:PatientSidebar ID="PatientSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">My Dashboard</h2>

            <!-- ============ STAT CARDS ============ -->
            <div style="display:flex; gap:16px; flex-wrap:wrap; margin-bottom:24px;">
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litTotal" runat="server" /></div>
                    <div class="label">Total Appointments</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litUpcoming" runat="server" /></div>
                    <div class="label">Upcoming</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litCompleted" runat="server" /></div>
                    <div class="label">Completed</div>
                </div>
                <div class="mc-stat-card" style="flex:1; min-width:140px;">
                    <div class="num"><asp:Literal ID="litCancelled" runat="server" /></div>
                    <div class="label">Cancelled</div>
                </div>
            </div>

            <!-- ============ RECENT APPOINTMENTS ============ -->
            <div class="mc-card">
                <div style="display:flex; justify-content:space-between; align-items:center;">
                    <h3 style="margin:0; color:var(--mc-primary-dark);">Recent Appointments</h3>
                    <a href="~/Patient/MyAppointments.aspx" runat="server">View all &rarr;</a>
                </div>

                <asp:Repeater ID="rptRecent" runat="server">
                    <ItemTemplate>
                        <div style="display:flex; justify-content:space-between; align-items:center; padding:12px 0; border-bottom:1px solid #eee;">
                            <div>
                                <b><%# Eval("TherapistName") %></b>
                                <p style="margin:2px 0; color:var(--mc-muted); font-size:14px;">
                                    <%# Eval("AppointmentDate", "{0:dd MMM yyyy}") %> &middot; <%# Eval("DisplayTime") %>
                                </p>
                            </div>
                            <span class='mc-badge mc-badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoAppointments" runat="server" Visible="false" Text="You haven't booked any appointments yet." style="color:var(--mc-muted);" />
            </div>
        </div>
    </div>
</asp:Content>
