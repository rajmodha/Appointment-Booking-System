<%@ Page Title="Leave Feedback" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Feedback.aspx.cs" Inherits="Patient_Feedback" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-width:550px; margin:30px auto; padding:0 20px;">

        <asp:Panel ID="pnlInvalid" runat="server" Visible="false" class="mc-card" style="text-align:center;">
            <p><asp:Literal ID="litInvalidReason" runat="server" /></p>
            <a href="~/Patient/MyAppointments.aspx" runat="server" class="mc-btn">Back to My Appointments</a>
        </asp:Panel>

        <asp:Panel ID="pnlForm" runat="server" class="mc-card">
            <h2 style="color:var(--mc-primary-dark);">How was your session?</h2>
            <p style="color:var(--mc-muted);">With <asp:Literal ID="litTherapistName" runat="server" /> on <asp:Literal ID="litSessionDate" runat="server" /></p>

            <label>Rating</label>
            <asp:RadioButtonList ID="rblRating" runat="server" RepeatDirection="Horizontal" CssClass="mc-form-control" style="border:none;">
                <asp:ListItem Text="⭐ 1" Value="1" />
                <asp:ListItem Text="⭐ 2" Value="2" />
                <asp:ListItem Text="⭐ 3" Value="3" />
                <asp:ListItem Text="⭐ 4" Value="4" />
                <asp:ListItem Text="⭐ 5" Value="5" Selected="True" />
            </asp:RadioButtonList>

            <label>Comments (optional)</label>
            <asp:TextBox ID="txtComments" runat="server" CssClass="mc-form-control" TextMode="MultiLine" Rows="4" placeholder="Share your experience..." />

            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Display="Dynamic" style="display:block; margin-bottom:10px;" />

            <asp:Button ID="btnSubmit" runat="server" Text="Submit Feedback" CssClass="mc-btn" Width="100%" OnClick="btnSubmit_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlThankYou" runat="server" Visible="false" class="mc-card" style="text-align:center;">
            <h2 style="color:var(--mc-accent);">Thank you for your feedback! 💜</h2>
            <a href="~/Patient/MyAppointments.aspx" runat="server" class="mc-btn">Back to My Appointments</a>
        </asp:Panel>

    </div>
</asp:Content>
