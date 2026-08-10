<%@ Page Title="Verify Payments" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="VerifyPayments.aspx.cs" Inherits="VerifyPayments" %>
<%@ Register Src="~/Admin/AdminSidebar.ascx" TagPrefix="uc" TagName="AdminSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:AdminSidebar ID="AdminSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">Verify Payments</h2>
            <p style="color:var(--mc-muted);">
                These bookings are on hold until you confirm the UPI transaction reference the
                patient entered actually matches a real payment. The therapist won't see the
                request, and no confirmation email is sent, until you Approve here.
            </p>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Display="Dynamic" style="display:block; margin-bottom:12px;" />

            <asp:Repeater ID="rptPayments" runat="server" OnItemCommand="rptPayments_ItemCommand">
                <ItemTemplate>
                    <div class="mc-card" style="display:flex; justify-content:space-between; align-items:center; flex-wrap:wrap; gap:10px;">
                        <div>
                            <h4 style="margin:0 0 4px 0;"><%# Eval("PatientName") %> &rarr; <%# Eval("TherapistName") %></h4>
                            <p style="margin:0; color:var(--mc-muted); font-size:14px;">
                                <%# Eval("AppointmentDate", "{0:dd MMM yyyy}") %> &middot; <%# Eval("DisplayTime") %> &middot; <%# Eval("ConsultationType") %>
                            </p>
                            <p style="margin:4px 0 0 0; font-weight:600; color:var(--mc-primary);">₹<%# Eval("Amount") %></p>
                            <p style="margin:4px 0 0 0; font-size:14px;">
                                UPI Ref: <b><%# Eval("UpiTransactionRef") %></b>
                            </p>
                        </div>
                        <div style="text-align:right;">
                            <asp:LinkButton ID="btnApprove" runat="server" CommandName="Approve" CommandArgument='<%# Eval("PaymentId") %>'
                                CssClass="mc-btn" style="font-size:13px; padding:8px 20px;">Approve Payment</asp:LinkButton>
                            <br style="margin-bottom:6px;" />
                            <asp:LinkButton ID="btnReject" runat="server" CommandName="Reject" CommandArgument='<%# Eval("PaymentId") %>'
                                CssClass="mc-btn mc-btn-outline" style="font-size:13px; padding:8px 20px; margin-top:6px; display:inline-block;"
                                OnClientClick="return confirm('Reject this payment? The appointment will be cancelled.');">Reject Payment</asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoPayments" runat="server" Visible="false" class="mc-card" style="text-align:center;">
                <p style="color:var(--mc-muted);">No payments waiting for verification right now.</p>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
