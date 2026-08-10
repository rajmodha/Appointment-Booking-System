<%@ Page Title="Book Appointment" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="BookAppointment.aspx.cs" Inherits="BookAppointment" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-width:700px; margin:30px auto; padding:0 20px;">

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false" class="mc-card" style="text-align:center;">
            <p>This therapist could not be found.</p>
            <a href="~/FindTherapist.aspx" runat="server" class="mc-btn">Back to Search</a>
        </asp:Panel>

        <!-- ============ STEP 1: CHOOSE SLOT ============ -->
        <asp:Panel ID="pnlSelectSlot" runat="server" class="mc-card">
            <h2 style="color:var(--mc-primary-dark);">Book with <asp:Literal ID="litTherapistName" runat="server" /></h2>
            <p style="color:var(--mc-muted);">Fee: ₹<asp:Literal ID="litFee" runat="server" /> per session</p>

            <label>Choose a date</label>
            <asp:TextBox ID="txtDate" runat="server" CssClass="mc-form-control" TextMode="Date" AutoPostBack="true" OnTextChanged="txtDate_TextChanged" />

            <label>Available time slots</label>
            <asp:DropDownList ID="ddlTimeSlot" runat="server" CssClass="mc-form-control">
                <asp:ListItem Text="-- Select a date first --" Value="" />
            </asp:DropDownList>

            <label>Consultation type</label>
            <asp:DropDownList ID="ddlConsultationType" runat="server" CssClass="mc-form-control" />

            <asp:Label ID="lblSlotError" runat="server" CssClass="text-danger" Display="Dynamic" style="display:block; margin-bottom:10px;" />

            <asp:Button ID="btnProceedToPayment" runat="server" Text="Proceed to Payment" CssClass="mc-btn" Width="100%" OnClick="btnProceedToPayment_Click" />
        </asp:Panel>

        <!-- ============ STEP 2: UPI QR PAYMENT ============ -->
        <asp:Panel ID="pnlPayment" runat="server" class="mc-card" Visible="false" style="text-align:center;">
            <h2 style="color:var(--mc-primary-dark);">Scan &amp; Pay</h2>
            <p style="color:var(--mc-muted);">
                Appointment with <asp:Literal ID="litPayTherapistName" runat="server" /> on
                <asp:Literal ID="litPayDateTime" runat="server" />
            </p>

            <p style="font-size:28px; font-weight:700; color:var(--mc-primary);">
                ₹<asp:Literal ID="litPayAmount" runat="server" />
            </p>

            <asp:Image ID="imgUpiQr" runat="server" style="width:220px;height:220px;border:1px solid #eee;border-radius:12px;" />

            <p style="color:var(--mc-muted); font-size:14px; margin-top:12px;">
                Scan this QR with any UPI app (GPay, PhonePe, Paytm) — the amount is
                already filled in. After paying, enter the transaction reference below.
            </p>

            <asp:TextBox ID="txtUpiRef" runat="server" CssClass="mc-form-control" placeholder="UPI Transaction Reference No." />
            <asp:RequiredFieldValidator ID="rfvUpiRef" ControlToValidate="txtUpiRef" runat="server"
                Text="Enter your UPI transaction reference to confirm" CssClass="text-danger" Display="Dynamic" />

            <asp:Button ID="btnConfirmPayment" runat="server" Text="I've Completed Payment" CssClass="mc-btn" Width="100%" OnClick="btnConfirmPayment_Click" />

            <asp:Label ID="lblPaymentError" runat="server" CssClass="text-danger" Display="Dynamic" style="display:block; margin-top:10px;" />
        </asp:Panel>

        <!-- ============ STEP 3: CONFIRMATION ============ -->
        <asp:Panel ID="pnlConfirmed" runat="server" class="mc-card" Visible="false" style="text-align:center;">
            <h2 style="color:var(--mc-accent);">🎉 Booking Submitted!</h2>
            <p style="color:var(--mc-muted);">
                We've received your booking and transaction reference. Our team will verify
                your payment shortly, after which the therapist will review your request.
                You'll receive a confirmation email once your payment is verified, and you
                can track its status anytime from "My Appointments".
            </p>
            <a href="~/Patient/PatientDashboard.aspx" runat="server" class="mc-btn">Go to My Dashboard</a>
        </asp:Panel>

    </div>
</asp:Content>
