<%@ Page Title="Manage Patients" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManagePatients.aspx.cs" Inherits="ManagePatients" %>
<%@ Register Src="~/Admin/AdminSidebar.ascx" TagPrefix="uc" TagName="AdminSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:AdminSidebar ID="AdminSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">Manage Patients</h2>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Display="Dynamic" style="display:block; margin-bottom:12px;" />

            <asp:Repeater ID="rptPatients" runat="server" OnItemCommand="rptPatients_ItemCommand">
                <ItemTemplate>
                    <div class="mc-card" style="display:flex; justify-content:space-between; align-items:center; flex-wrap:wrap; gap:10px;">
                        <div>
                            <h4 style="margin:0 0 4px 0;"><%# Eval("FullName") %></h4>
                            <p style="margin:0; color:var(--mc-muted); font-size:14px;">
                                ✉️ <%# Eval("Email") %> &nbsp;|&nbsp; 📞 <%# Eval("Phone") %>
                            </p>
                            <p style="margin:4px 0 0 0; color:var(--mc-muted); font-size:14px;">
                                <%# Eval("TotalAppointments") %> appointment(s) booked
                            </p>
                        </div>
                        <div style="text-align:right;">
                            <span class='mc-badge <%# Eval("IsActive").ToString() == "True" ? "mc-badge-confirmed" : "mc-badge-cancelled" %>' style="display:inline-block; margin-bottom:8px;">
                                <%# Eval("IsActive").ToString() == "True" ? "Active" : "Disabled" %>
                            </span>
                            <br />
                            <asp:LinkButton ID="btnToggleActive" runat="server" CommandName="ToggleActive" CommandArgument='<%# Eval("PatientId") %>'
                                CssClass="mc-btn mc-btn-outline" style="font-size:13px; padding:6px 16px;"
                                OnClientClick='<%# "return confirm(\"" + (Eval("IsActive").ToString() == "True" ? "Disable" : "Enable") + " this account?\");" %>'>
                                <%# Eval("IsActive").ToString() == "True" ? "Disable Account" : "Enable Account" %>
                            </asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoResults" runat="server" Visible="false" class="mc-card" style="text-align:center;">
                <p style="color:var(--mc-muted);">No patients registered yet.</p>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
