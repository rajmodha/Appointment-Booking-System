<%@ Page Title="Reset Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ResetPassword.aspx.cs" Inherits="ResetPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-auth-box">

        <asp:Panel ID="pnlInvalidToken" runat="server" Visible="false" style="text-align:center;">
            <h2 style="color:var(--mc-primary-dark);">This link has expired</h2>
            <p style="color:var(--mc-muted);">Password reset links are only valid for 1 hour, or may have already been used.</p>
            <a href="~/ForgotPassword.aspx" runat="server" class="mc-btn">Request a New Link</a>
        </asp:Panel>

        <asp:Panel ID="pnlForm" runat="server">
            <h2 style="color:var(--mc-primary-dark); text-align:center;">Choose a new password</h2>

            <asp:TextBox ID="txtNewPassword" runat="server" CssClass="mc-form-control" placeholder="New Password" TextMode="Password" />
            <asp:RequiredFieldValidator ID="rfvNewPassword" ControlToValidate="txtNewPassword" runat="server" Text="Please enter a new password" CssClass="text-danger" Display="Dynamic" />

            <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="mc-form-control" placeholder="Confirm New Password" TextMode="Password" />
            <asp:CompareValidator ID="cvConfirmPassword" ControlToValidate="txtConfirmPassword" ControlToCompare="txtNewPassword" runat="server"
                Text="Passwords do not match" CssClass="text-danger" Display="Dynamic" />

            <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Display="Dynamic" style="display:block; margin-bottom:10px;" />

            <asp:Button ID="btnResetPassword" runat="server" Text="Reset Password" CssClass="mc-btn" Width="100%" OnClick="btnResetPassword_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlSuccess" runat="server" Visible="false" style="text-align:center;">
            <h2 style="color:var(--mc-accent);">Password reset! 🎉</h2>
            <a href="~/Login.aspx" runat="server" class="mc-btn">Go to Login</a>
        </asp:Panel>

    </div>
</asp:Content>
