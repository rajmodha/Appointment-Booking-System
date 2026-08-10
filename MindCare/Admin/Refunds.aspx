<%@ Page Title="Refunds" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Refunds.aspx.cs" Inherits="Admin_Refunds" %>
<%@ Register Src="~/Admin/AdminSidebar.ascx" TagPrefix="uc" TagName="AdminSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:AdminSidebar ID="AdminSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">Refunds</h2>
            <p style="color:var(--mc-muted);">
                These are payments that were successfully collected for appointments that were
                later Rejected or Cancelled by the therapist. Once you've actually sent the refund
                through your UPI app using the transaction reference below, mark it here - this
                removes it from "Revenue Collected" on the Dashboard.
            </p>

            <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Display="Dynamic" style="display:block; margin-bottom:12px;" />

            <!-- ============ AWAITING REFUND ============ -->
            <h3 style="color:var(--mc-primary-dark);">Awaiting Refund</h3>
            <asp:Repeater ID="rptPending" runat="server" OnItemCommand="rptPending_ItemCommand">
                <ItemTemplate>
                    <div class="mc-card" style="display:flex; justify-content:space-between; align-items:center; flex-wrap:wrap; gap:10px;">
                        <div>
                            <h4 style="margin:0 0 4px 0;"><%# Eval("PatientName") %> &rarr; <%# Eval("TherapistName") %></h4>
                            <p style="margin:0; color:var(--mc-muted); font-size:14px;">
                                <%# Eval("AppointmentDate", "{0:dd MMM yyyy}") %> &middot; <%# Eval("DisplayTime") %>
                                &middot; Appointment status: <span class='mc-badge mc-badge-<%# Eval("AppointmentStatus").ToString().ToLower() %>'><%# Eval("AppointmentStatus") %></span>
                            </p>
                            <p style="margin:4px 0 0 0; font-weight:600; color:var(--mc-primary);">₹<%# Eval("Amount") %></p>
                            <p style="margin:4px 0 0 0; font-size:14px;">Original UPI Ref: <b><%# Eval("UpiTransactionRef") %></b></p>

                            <label style="font-size:13px; margin-top:8px; display:block;">Refund Transaction ID (after you've sent it via UPI)</label>
                            <asp:TextBox ID="txtRefundRef" runat="server" CssClass="mc-form-control" placeholder="e.g. 402812345678" style="max-width:280px;" />
                        </div>
                        <div>
                            <asp:LinkButton ID="btnMarkRefunded" runat="server" CommandName="MarkRefunded" CommandArgument='<%# Eval("PaymentId") %>'
                                CssClass="mc-btn" style="font-size:13px; padding:8px 20px;"
                                OnClientClick="return confirm('Confirm you have already sent this refund via UPI and entered its transaction ID above?');">Mark as Refunded</asp:LinkButton>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlNoPending" runat="server" Visible="false" class="mc-card" style="text-align:center;">
                <p style="color:var(--mc-muted);">No refunds currently awaiting action.</p>
            </asp:Panel>

            <!-- ============ ALREADY REFUNDED ============ -->
            <h3 style="color:var(--mc-primary-dark); margin-top:30px;">Already Refunded</h3>
            <asp:Repeater ID="rptRefunded" runat="server">
                <ItemTemplate>
                    <div class="mc-card" style="display:flex; justify-content:space-between; align-items:center; flex-wrap:wrap; gap:10px; opacity:0.7;">
                        <div>
                            <h4 style="margin:0 0 4px 0;"><%# Eval("PatientName") %> &rarr; <%# Eval("TherapistName") %></h4>
                            <p style="margin:0; color:var(--mc-muted); font-size:14px;">
                                <%# Eval("AppointmentDate", "{0:dd MMM yyyy}") %> &middot; <%# Eval("DisplayTime") %>
                            </p>
                            <p style="margin:4px 0 0 0; font-weight:600; color:var(--mc-muted);">₹<%# Eval("Amount") %> refunded</p>
                            <p style="margin:4px 0 0 0; font-size:14px; color:var(--mc-muted);">
                                Original UPI Ref: <%# Eval("UpiTransactionRef") %> &middot; Refund Ref: <%# Eval("RefundTransactionRef") %>
                            </p>
                        </div>
                        <span class="mc-badge mc-badge-completed">Refunded</span>
                    </div>
                </ItemTemplate>
            </asp:Repeater>
            <asp:Panel ID="pnlNoRefunded" runat="server" Visible="false" class="mc-card" style="text-align:center;">
                <p style="color:var(--mc-muted);">No refunds processed yet.</p>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
