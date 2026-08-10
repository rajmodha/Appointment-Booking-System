<%@ Control Language="C#" AutoEventWireup="true" CodeBehind="AdminSidebar.ascx.cs" Inherits="AdminSidebar" %>

<div class="mc-sidebar">
    <p style="font-weight:700; color:var(--mc-primary-dark); margin-top:0;">Admin Panel</p>
    <a href="~/Admin/AdminDashboard.aspx" runat="server" id="lnkDashboard">Dashboard</a>
    <a href="~/Admin/ManageTherapists.aspx" runat="server" id="lnkTherapists">Manage Therapists</a>
    <a href="~/Admin/ManagePatients.aspx" runat="server" id="lnkPatients">Manage Patients</a>
    <a href="~/Admin/ManageCategories.aspx" runat="server" id="lnkCategories">Therapy Categories</a>
    <a href="~/Admin/AllAppointments.aspx" runat="server" id="lnkAppointments">All Appointments</a>
    <a href="~/Admin/VerifyPayments.aspx" runat="server" id="lnkPayments">Verify Payments</a>
    <a href="~/Admin/Refunds.aspx" runat="server" id="lnkRefunds">Refunds</a>
    <a href="~/Admin/ContactMessages.aspx" runat="server" id="lnkMessages">Contact Messages</a>
    <a href="~/Admin/ViewFeedback.aspx" runat="server" id="lnkFeedback">Feedback</a>
    <a href="~/Admin/Reports.aspx" runat="server" id="lnkReports">Reports</a>
    <a href="~/Admin/MyProfile.aspx" runat="server" id="lnkProfile">My Profile</a>
</div>
