<%@ Page Title="Feedback" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ViewFeedback.aspx.cs" Inherits="ViewFeedback" %>
<%@ Register Src="~/Admin/AdminSidebar.ascx" TagPrefix="uc" TagName="AdminSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:AdminSidebar ID="AdminSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">Patient Feedback</h2>

            <asp:Repeater ID="rptFeedback" runat="server">
                <ItemTemplate>
                    <div class="mc-card">
                        <div style="display:flex; justify-content:space-between;">
                            <div>
                                <b><%# Eval("PatientName") %></b>
                                <span style="color:var(--mc-muted);"> reviewed </span>
                                <b><%# Eval("TherapistName") %></b>
                            </div>
                            <span style="font-weight:700; color:var(--mc-primary);"><%# Eval("Rating") %>/5 ⭐</span>
                        </div>
                        <p style="margin:8px 0 0 0; color:var(--mc-muted);">
                            <%# string.IsNullOrEmpty(Eval("Comments").ToString()) ? "(no comment left)" : Eval("Comments") %>
                        </p>
                        <p style="margin:6px 0 0 0; font-size:12px; color:var(--mc-muted);">
                            <%# Eval("CreatedOn", "{0:dd MMM yyyy}") %>
                        </p>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoFeedback" runat="server" Visible="false" class="mc-card" style="text-align:center;">
                <p style="color:var(--mc-muted);">No feedback submitted yet.</p>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
