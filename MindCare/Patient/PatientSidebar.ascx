<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="PatientSidebar.ascx.cs" Inherits="PatientSidebar" %>

<div class="mc-sidebar">
    <p style="font-weight:700; color:var(--mc-primary-dark); margin-top:0;">Hi, <asp:Literal ID="litPatientName" runat="server" />👋</p>
    <a href="~/Patient/PatientDashboard.aspx" runat="server" id="lnkDashboard">Dashboard</a>
    <a href="~/Patient/MyAppointments.aspx" runat="server" id="lnkAppointments">My Appointments</a>
    <a href="~/Patient/CalendarView.aspx" runat="server" id="lnkCalendar">Calendar</a>
    <a href="~/Patient/MyProfile.aspx" runat="server" id="lnkProfile">My Profile</a>
    <a href="~/FindTherapist.aspx" runat="server">Book New Appointment</a>
</div>
