<%@ Page Title="My Appointments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyAppointments.aspx.cs" Inherits="MyAppointments" %>
<%@ Register Src="~/Patient/PatientSidebar.ascx" TagPrefix="uc" TagName="PatientSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:PatientSidebar ID="PatientSidebar1" runat="server" />

        <div class="mc-main">
            <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:16px;">
                <h2 style="color:var(--mc-primary-dark); margin:0;">My Appointments</h2>

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

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Display="Dynamic" style="display:block; margin-bottom:12px;" />

            <asp:Repeater ID="rptAppointments" runat="server" OnItemCommand="rptAppointments_ItemCommand">
                <ItemTemplate>
                    <div class="mc-card" style="display:flex; justify-content:space-between; align-items:center; flex-wrap:wrap; gap:10px;">
                        <div>
                            <h4 style="margin:0 0 4px 0;"><%# Eval("TherapistName") %></h4>
                            <p style="margin:0; color:var(--mc-muted); font-size:14px;">
                                <%# Eval("AppointmentDate", "{0:dd MMM yyyy}") %> &middot; <%# Eval("DisplayTime") %> &middot; <%# Eval("ConsultationType") %>
                            </p>
                            <p style="margin:4px 0 0 0; font-weight:600; color:var(--mc-primary);">₹<%# Eval("Amount") %></p>
                        </div>

                        <div style="text-align:right;">
                            <%# Eval("Status").ToString() == "Pending" && Eval("PaymentStatus").ToString() != "Success"
                                ? "<span class='mc-badge mc-badge-pending' style='display:inline-block; margin-bottom:8px;'>Awaiting Payment Verification</span>"
                                : "<span class='mc-badge mc-badge-" + Eval("Status").ToString().ToLower() + "' style='display:inline-block; margin-bottom:8px;'>" + Eval("Status") + "</span>" %>
                            <br />

                            <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CommandArgument='<%# Eval("AppointmentId") %>'
                                CssClass="mc-btn mc-btn-outline" style="font-size:13px; padding:6px 16px;"
                                Visible='<%# Eval("Status").ToString() == "Pending" || Eval("Status").ToString() == "Confirmed" %>'
                                OnClientClick="return confirm('Cancel this appointment?');">Cancel</asp:LinkButton>

                            <asp:HyperLink ID="hlFeedback" runat="server"
                                CssClass="mc-btn" style="font-size:13px; padding:6px 16px;"
                                Visible='<%# Eval("Status").ToString() == "Completed" && Eval("HasFeedback").ToString() == "0" %>'
                                NavigateUrl='<%# "~/Patient/Feedback.aspx?appointmentId=" + Eval("AppointmentId") %>'>Leave Feedback</asp:HyperLink>

                            <asp:Literal ID="litMeetingLink" runat="server"
                                Visible='<%# Eval("Status").ToString() == "Confirmed" && Eval("ConsultationType").ToString() == "Online" && Eval("MeetingLink") != DBNull.Value %>'
                                Text='<%# "<br/><a href=\"" + Eval("MeetingLink") + "\" target=\"_blank\">Join Meeting Link</a>" %>' />
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoAppointments" runat="server" Visible="false" class="mc-card" style="text-align:center;">
                <p style="color:var(--mc-muted);">No appointments found for this filter.</p>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
