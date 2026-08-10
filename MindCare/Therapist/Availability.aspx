<%@ Page Title="My Availability" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Availability.aspx.cs" Inherits="Therapist_Availability" %>
<%@ Register Src="~/Therapist/TherapistSidebar.ascx" TagPrefix="uc" TagName="TherapistSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:TherapistSidebar ID="TherapistSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">My Availability</h2>

            <!-- ============ ADD NEW SLOT ============ -->
            <div class="mc-card">
                <h3 style="margin-top:0; color:var(--mc-primary-dark);">Add a Time Slot</h3>
                <div style="display:flex; gap:12px; flex-wrap:wrap; align-items:flex-end;">
                    <div style="flex:1; min-width:140px;">
                        <label>Day</label>
                        <asp:DropDownList ID="ddlDay" runat="server" CssClass="mc-form-control">
                            <asp:ListItem Text="Monday" Value="Monday" />
                            <asp:ListItem Text="Tuesday" Value="Tuesday" />
                            <asp:ListItem Text="Wednesday" Value="Wednesday" />
                            <asp:ListItem Text="Thursday" Value="Thursday" />
                            <asp:ListItem Text="Friday" Value="Friday" />
                            <asp:ListItem Text="Saturday" Value="Saturday" />
                            <asp:ListItem Text="Sunday" Value="Sunday" />
                        </asp:DropDownList>
                    </div>
                    <div style="flex:1; min-width:120px;">
                        <label>Start Time</label>
                        <asp:TextBox ID="txtStartTime" runat="server" CssClass="mc-form-control" TextMode="Time" />
                    </div>
                    <div style="flex:1; min-width:120px;">
                        <label>End Time</label>
                        <asp:TextBox ID="txtEndTime" runat="server" CssClass="mc-form-control" TextMode="Time" />
                    </div>
                    <div>
                        <asp:Button ID="btnAddSlot" runat="server" Text="Add Slot" CssClass="mc-btn" OnClick="btnAddSlot_Click" />
                    </div>
                </div>
                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Display="Dynamic" style="display:block; margin-top:10px;" />
            </div>

            <!-- ============ EXISTING SLOTS ============ -->
            <div class="mc-card">
                <h3 style="margin-top:0; color:var(--mc-primary-dark);">Current Weekly Schedule</h3>

                <asp:Repeater ID="rptAvailability" runat="server" OnItemCommand="rptAvailability_ItemCommand">
                    <ItemTemplate>
                        <div style="display:flex; justify-content:space-between; align-items:center; padding:10px 0; border-bottom:1px solid #eee;">
                            <div>
                                <b><%# Eval("DayOfWeek") %></b>
                                <span style="color:var(--mc-muted); margin-left:10px;"><%# Eval("StartTime") %> - <%# Eval("EndTime") %></span>
                            </div>
                            <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CommandArgument='<%# Eval("AvailabilityId") %>'
                                CssClass="mc-btn mc-btn-outline" style="font-size:13px; padding:6px 16px;"
                                OnClientClick="return confirm('Remove this time slot?');">Remove</asp:LinkButton>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoSlots" runat="server" Visible="false" Text="You haven't set any availability yet - add your first slot above." style="color:var(--mc-muted);" />
            </div>
        </div>
    </div>
</asp:Content>
