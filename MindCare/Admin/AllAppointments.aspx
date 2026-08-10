<%@ Page Title="All Appointments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="AllAppointments.aspx.cs" Inherits="AllAppointments" %>
<%@ Register Src="~/Admin/AdminSidebar.ascx" TagPrefix="uc" TagName="AdminSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:AdminSidebar ID="AdminSidebar1" runat="server" />

        <div class="mc-main">
            <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:16px;">
                <h2 style="color:var(--mc-primary-dark); margin:0;">All Appointments</h2>

                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="mc-form-control" style="width:auto;" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Text="All Statuses" Value="" />
                    <asp:ListItem Text="Pending" Value="Pending" />
                    <asp:ListItem Text="Confirmed" Value="Confirmed" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="Cancelled" Value="Cancelled" />
                    <asp:ListItem Text="Rescheduled" Value="Rescheduled" />
                    <asp:ListItem Text="Rejected" Value="Rejected" />
                </asp:DropDownList>
            </div>

            <table style="width:100%; border-collapse:collapse; background:var(--mc-white); border-radius:var(--mc-radius); overflow:hidden; box-shadow:var(--mc-shadow);">
                <thead>
                    <tr style="background:var(--mc-lavender); text-align:left;">
                        <th style="padding:12px;">Patient</th>
                        <th style="padding:12px;">Therapist</th>
                        <th style="padding:12px;">Date &amp; Time</th>
                        <th style="padding:12px;">Type</th>
                        <th style="padding:12px;">Amount</th>
                        <th style="padding:12px;">Status</th>
                    </tr>
                </thead>
                <asp:Repeater ID="rptAppointments" runat="server">
                    <ItemTemplate>
                        <tr style="border-top:1px solid #eee;">
                            <td style="padding:12px;"><%# Eval("PatientName") %></td>
                            <td style="padding:12px;"><%# Eval("TherapistName") %></td>
                            <td style="padding:12px;"><%# Eval("AppointmentDate", "{0:dd MMM yyyy}") %> &middot; <%# Eval("DisplayTime") %></td>
                            <td style="padding:12px;"><%# Eval("ConsultationType") %></td>
                            <td style="padding:12px;">₹<%# Eval("Amount") %></td>
                            <td style="padding:12px;"><span class='mc-badge mc-badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span></td>
                        </tr>
                    </ItemTemplate>
                </asp:Repeater>
            </table>

            <asp:Panel ID="pnlNoResults" runat="server" Visible="false" class="mc-card" style="text-align:center; margin-top:16px;">
                <p style="color:var(--mc-muted);">No appointments found for this filter.</p>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
