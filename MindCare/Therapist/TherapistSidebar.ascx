<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="TherapistSidebar.ascx.cs" Inherits="TherapistSidebar" %>

<div class="mc-sidebar">
    <p style="font-weight:700; color:var(--mc-primary-dark); margin-top:0;">Hi, Dr. <asp:Literal ID="litTherapistName" runat="server" />👋</p>
    <a href="~/Therapist/TherapistDashboard.aspx" runat="server" id="lnkDashboard">Dashboard</a>
    <a href="~/Therapist/Requests.aspx" runat="server" id="lnkRequests">Appointment Requests</a>
    <a href="~/Therapist/CalendarView.aspx" runat="server" id="lnkCalendar">Calendar</a>
    <a href="~/Therapist/Availability.aspx" runat="server" id="lnkAvailability">My Availability</a>
    <a href="~/Therapist/Profile.aspx" runat="server" id="lnkProfile">My Profile</a>
</div>
