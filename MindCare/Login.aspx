<%@ Page Title="Login" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Login.aspx.cs" Inherits="Login" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-auth-box">
        <h2 style="color:var(--mc-primary-dark); text-align:center;">Welcome back</h2>
        <p style="text-align:center; color:var(--mc-muted);">Login to continue to MindCare</p>

        <asp:Label ID="lblRegisteredBanner" runat="server" Visible="false" CssClass="text-success"
            style="display:block; background:#D4EDDA; padding:12px 16px; border-radius:10px; margin-bottom:16px;" />

        <asp:TextBox ID="txtEmail" runat="server" CssClass="mc-form-control" placeholder="Email Address" />
        <asp:RequiredFieldValidator ID="rfvEmail" ControlToValidate="txtEmail" runat="server" Text="Email is required" CssClass="text-danger" Display="Dynamic" />

        <asp:TextBox ID="txtPassword" runat="server" CssClass="mc-form-control" placeholder="Password" TextMode="Password" />
        <asp:RequiredFieldValidator ID="rfvPassword" ControlToValidate="txtPassword" runat="server" Text="Password is required" CssClass="text-danger" Display="Dynamic" />

        <asp:Button ID="btnLogin" runat="server" Text="Login" CssClass="mc-btn" Width="100%" OnClick="btnLogin_Click" />

        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Display="Dynamic" style="display:block;margin-top:12px;" />

        <p style="text-align:center; margin-top:16px;">
            <a href="~/ForgotPassword.aspx" runat="server">Forgot Password?</a><br />
            Don't have an account? <a href="~/Register.aspx" runat="server">Register here</a>
        </p>
    </div>
</asp:Content>
