<%@ Page Title="Reports" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Reports.aspx.cs" Inherits="Reports" %>
<%@ Register Src="~/Admin/AdminSidebar.ascx" TagPrefix="uc" TagName="AdminSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:AdminSidebar ID="AdminSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">Reports</h2>

            <!-- ============ APPOINTMENTS BY STATUS ============ -->
            <div class="mc-card">
                <h3 style="margin-top:0; color:var(--mc-primary-dark);">Appointments by Status</h3>
                <asp:Repeater ID="rptStatusBreakdown" runat="server">
                    <ItemTemplate>
                        <div style="display:flex; justify-content:space-between; padding:8px 0; border-bottom:1px solid #eee;">
                            <span class='mc-badge mc-badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span>
                            <b><%# Eval("Total") %></b>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <!-- ============ BOOKINGS BY CATEGORY ============ -->
            <div class="mc-card">
                <h3 style="margin-top:0; color:var(--mc-primary-dark);">Bookings by Therapy Category</h3>
                <asp:Repeater ID="rptCategoryBreakdown" runat="server">
                    <ItemTemplate>
                        <div style="display:flex; justify-content:space-between; padding:8px 0; border-bottom:1px solid #eee;">
                            <span><%# Eval("CategoryName") %></span>
                            <b><%# Eval("Total") %></b>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
            </div>

            <!-- ============ TOP RATED THERAPISTS ============ -->
            <div class="mc-card">
                <h3 style="margin-top:0; color:var(--mc-primary-dark);">Top Rated Therapists</h3>
                <asp:Repeater ID="rptTopTherapists" runat="server">
                    <ItemTemplate>
                        <div style="display:flex; justify-content:space-between; padding:8px 0; border-bottom:1px solid #eee;">
                            <span><%# Eval("FullName") %></span>
                            <b>⭐ <%# Eval("AvgRating", "{0:0.0}") %> (<%# Eval("TotalReviews") %>)</b>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoRatings" runat="server" Visible="false" Text="No ratings submitted yet." style="color:var(--mc-muted);" />
            </div>
        </div>
    </div>
</asp:Content>
