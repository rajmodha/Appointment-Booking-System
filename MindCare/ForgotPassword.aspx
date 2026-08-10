<%@ Page Title="Forgot Password" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ForgotPassword.aspx.cs" Inherits="ForgotPassword" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-auth-box">
        <h2 style="color:var(--mc-primary-dark); text-align:center;">Forgot your password?</h2>
        <p style="text-align:center; color:var(--mc-muted);">Enter your account email and we'll send you a reset link.</p>

        <asp:Panel ID="pnlForm" runat="server">
            <asp:TextBox ID="txtEmail" runat="server" CssClass="mc-form-control" placeholder="Email Address" />
            <asp:RequiredFieldValidator ID="rfvEmail" ControlToValidate="txtEmail" runat="server" Text="Email is required" CssClass="text-danger" Display="Dynamic" />

            <asp:Button ID="btnSendLink" runat="server" Text="Send Reset Link" CssClass="mc-btn" Width="100%" OnClick="btnSendLink_Click" />
        </asp:Panel>

        <asp:Panel ID="pnlConfirmation" runat="server" Visible="false" style="text-align:center;">
            <p>If an account exists for that email, we've sent a password reset link to it.
               The link is valid for <b>1 hour</b>.</p>
            <a href="~/Login.aspx" runat="server" class="mc-btn">Back to Login</a>
        </asp:Panel>

        <p style="text-align:center; margin-top:16px;">
            <a href="~/Login.aspx" runat="server">Back to Login</a>
        </p>
    </div>
</asp:Content>
