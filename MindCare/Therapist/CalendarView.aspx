<%@ Page Title="Calendar" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="CalendarView.aspx.cs" Inherits="Therapist_CalendarView" %>
<%@ Register Src="~/Therapist/TherapistSidebar.ascx" TagPrefix="uc" TagName="TherapistSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:TherapistSidebar ID="TherapistSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">My Calendar</h2>

            <div class="mc-card">
                <asp:Calendar ID="calAppointments" runat="server"
                    OnDayRender="calAppointments_DayRender"
                    OnSelectionChanged="calAppointments_SelectionChanged"
                    OnVisibleMonthChanged="calAppointments_VisibleMonthChanged"
                    Width="100%" BorderWidth="0" CellPadding="8" CellSpacing="4"
                    Font-Names="Segoe UI, Poppins, Arial, sans-serif">
                    <TitleStyle BackColor="#EDE7F6" ForeColor="#6355C7" Font-Bold="true" Height="40px" />
                    <DayHeaderStyle BackColor="#F7F5FF" ForeColor="#7A7A8C" Height="30px" />
                    <DayStyle BackColor="White" ForeColor="#3A3A4A" />
                    <OtherMonthDayStyle ForeColor="#CFCFE0" />
                    <TodayDayStyle BackColor="#E0F7F1" ForeColor="#3A3A4A" Font-Bold="true" />
                    <SelectedDayStyle BackColor="#7C6FE0" ForeColor="White" Font-Bold="true" />
                    <NextPrevStyle ForeColor="#6355C7" Font-Bold="true" />
                </asp:Calendar>

                <p style="margin-top:12px; font-size:13px; color:var(--mc-muted);">
                    🟢 Days with a session are marked below the date number. Click a date to see details.
                </p>
            </div>

            <div class="mc-card">
                <h3 style="margin-top:0; color:var(--mc-primary-dark);">
                    Sessions on <asp:Literal ID="litSelectedDate" runat="server" />
                </h3>

                <asp:Repeater ID="rptDayAppointments" runat="server">
                    <ItemTemplate>
                        <div style="display:flex; justify-content:space-between; align-items:center; padding:10px 0; border-bottom:1px solid #eee;">
                            <div>
                                <b><%# Eval("PatientName") %></b>
                                <p style="margin:2px 0 0 0; color:var(--mc-muted); font-size:14px;">
                                    <%# Eval("DisplayTime") %> &middot; <%# Eval("ConsultationType") %>
                                </p>
                            </div>
                            <span class='mc-badge mc-badge-<%# Eval("Status").ToString().ToLower() %>'><%# Eval("Status") %></span>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoneOnDate" runat="server" Visible="false" Text="No sessions on this date." style="color:var(--mc-muted);" />
            </div>
        </div>
    </div>
</asp:Content>
