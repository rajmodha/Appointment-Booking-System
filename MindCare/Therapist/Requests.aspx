<%@ Page Title="Appointment Requests" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Requests.aspx.cs" Inherits="Therapist_Requests" %>
<%@ Register Src="~/Therapist/TherapistSidebar.ascx" TagPrefix="uc" TagName="TherapistSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:TherapistSidebar ID="TherapistSidebar1" runat="server" />

        <div class="mc-main">
            <div style="display:flex; justify-content:space-between; align-items:center; margin-bottom:16px;">
                <h2 style="color:var(--mc-primary-dark); margin:0;">Appointment Requests</h2>

                <asp:DropDownList ID="ddlStatusFilter" runat="server" CssClass="mc-form-control" style="width:auto;" AutoPostBack="true" OnSelectedIndexChanged="ddlStatusFilter_SelectedIndexChanged">
                    <asp:ListItem Text="All Statuses" Value="" />
                    <asp:ListItem Text="Pending" Value="Pending" Selected="True" />
                    <asp:ListItem Text="Confirmed" Value="Confirmed" />
                    <asp:ListItem Text="Completed" Value="Completed" />
                    <asp:ListItem Text="Cancelled" Value="Cancelled" />
                    <asp:ListItem Text="Rejected" Value="Rejected" />
                </asp:DropDownList>
            </div>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Display="Dynamic" style="display:block; margin-bottom:12px;" />

            <asp:Repeater ID="rptRequests" runat="server" OnItemCommand="rptRequests_ItemCommand" OnItemDataBound="rptRequests_ItemDataBound">
                <ItemTemplate>
                    <div class="mc-card">
                        <div style="display:flex; justify-content:space-between; flex-wrap:wrap; gap:10px;">
                            <div>
                                <h4 style="margin:0 0 4px 0;"><%# Eval("PatientName") %></h4>
                                <p style="margin:0; color:var(--mc-muted); font-size:14px;">
                                    📞 <%# Eval("PatientPhone") %> &nbsp;|&nbsp; ✉️ <%# Eval("PatientEmail") %>
                                </p>
                                <p style="margin:4px 0 0 0; color:var(--mc-muted); font-size:14px;">
                                    <%# Eval("AppointmentDate", "{0:dd MMM yyyy}") %> &middot; <%# Eval("DisplayTime") %> &middot; <%# Eval("ConsultationType") %>
                                </p>
                                <p style="margin:4px 0 0 0; font-weight:600; color:var(--mc-primary);">₹<%# Eval("Amount") %></p>
                                <asp:Literal ID="litNotes" runat="server" Visible='<%# !string.IsNullOrEmpty(Eval("Notes").ToString()) %>' Text='<%# "<p style=\"margin:6px 0 0 0; font-style:italic; color:var(--mc-muted);\">\"" + Eval("Notes") + "\"</p>" %>' />
                            </div>
                            <div style="text-align:right; min-width:160px;">
                                <span class='mc-badge mc-badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span>
                            </div>
                        </div>

                        <!-- Meeting link box: shown for Online appointments that are Pending (about
                             to be accepted) or Confirmed-but-missing-a-link yet -->
                        <asp:Panel ID="pnlMeetingLink" runat="server" style="margin-top:12px;">
                            <label style="font-size:13px;">Meeting Link (for online sessions)</label>
                            <asp:TextBox ID="txtMeetingLink" runat="server" CssClass="mc-form-control" placeholder="https://meet.google.com/..." Text='<%# Eval("MeetingLink") %>' />
                        </asp:Panel>

                        <div style="margin-top:10px; display:flex; gap:10px; flex-wrap:wrap;">
                            <asp:LinkButton ID="btnAccept" runat="server" CommandName="Accept" CommandArgument='<%# Eval("AppointmentId") %>'
                                CssClass="mc-btn" style="font-size:13px; padding:8px 20px;">Accept</asp:LinkButton>

                            <asp:LinkButton ID="btnReject" runat="server" CommandName="Reject" CommandArgument='<%# Eval("AppointmentId") %>'
                                CssClass="mc-btn mc-btn-outline" style="font-size:13px; padding:8px 20px;"
                                OnClientClick="return confirm('Reject this appointment request?');">Reject</asp:LinkButton>

                            <asp:LinkButton ID="btnSaveLink" runat="server" CommandName="SaveLink" CommandArgument='<%# Eval("AppointmentId") %>'
                                CssClass="mc-btn" style="font-size:13px; padding:8px 20px;">Save Meeting Link</asp:LinkButton>

                            <asp:LinkButton ID="btnComplete" runat="server" CommandName="Complete" CommandArgument='<%# Eval("AppointmentId") %>'
                                CssClass="mc-btn" style="font-size:13px; padding:8px 20px;"
                                OnClientClick="return confirm('Mark this session as completed?');">Mark Completed</asp:LinkButton>

                            <asp:LinkButton ID="btnCancel" runat="server" CommandName="Cancel" CommandArgument='<%# Eval("AppointmentId") %>'
                                CssClass="mc-btn mc-btn-outline" style="font-size:13px; padding:8px 20px;"
                                OnClientClick="return confirm('Cancel this already-confirmed session? The patient will be notified and Admin will be asked to process a refund.');">Cancel Session</asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoRequests" runat="server" Visible="false" class="mc-card" style="text-align:center;">
                <p style="color:var(--mc-muted);">No appointments found for this filter.</p>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
