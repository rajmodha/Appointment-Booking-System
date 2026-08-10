<%@ Page Title="Register" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Register.aspx.cs" Inherits="Register" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-auth-box">
        <h2 style="color:var(--mc-primary-dark); text-align:center;">Create your MindCare account</h2>

        <div class="form-group" style="margin-bottom:16px;">
            <label>I am registering as:</label><br />
            <asp:RadioButtonList ID="rblRole" runat="server" RepeatLayout="Flow" RepeatDirection="Horizontal" CssClass="mc-role-toggle">
                <asp:ListItem Text="Patient" Value="3" Selected="True" />
                <asp:ListItem Text="Therapist" Value="2" />
            </asp:RadioButtonList>
        </div>

        <asp:TextBox ID="txtFullName" runat="server" CssClass="mc-form-control" placeholder="Full Name" />
        <asp:RequiredFieldValidator ID="rfvFullName" ControlToValidate="txtFullName" runat="server" Text="Full name is required" CssClass="text-danger" Display="Dynamic" />

        <asp:TextBox ID="txtEmail" runat="server" CssClass="mc-form-control" placeholder="Email Address" />
        <asp:RequiredFieldValidator ID="rfvEmail" ControlToValidate="txtEmail" runat="server" Text="Email is required" CssClass="text-danger" Display="Dynamic" />
        <asp:RegularExpressionValidator ID="revEmail" ControlToValidate="txtEmail" runat="server"
            ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" Text="Enter a valid email" CssClass="text-danger" Display="Dynamic" />

        <asp:TextBox ID="txtPhone" runat="server" CssClass="mc-form-control" placeholder="Phone Number" />

        <asp:TextBox ID="txtPassword" runat="server" CssClass="mc-form-control" placeholder="Password" TextMode="Password" />
        <asp:RequiredFieldValidator ID="rfvPassword" ControlToValidate="txtPassword" runat="server" Text="Password is required" CssClass="text-danger" Display="Dynamic" />

        <asp:TextBox ID="txtConfirmPassword" runat="server" CssClass="mc-form-control" placeholder="Confirm Password" TextMode="Password" />
        <asp:CompareValidator ID="cvConfirmPassword" ControlToValidate="txtConfirmPassword" ControlToCompare="txtPassword" runat="server"
            Text="Passwords do not match" CssClass="text-danger" Display="Dynamic" />

        <asp:Button ID="btnRegister" runat="server" Text="Register" CssClass="mc-btn" Width="100%" OnClick="btnRegister_Click" />

        <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Display="Dynamic" style="display:block;margin-top:12px;" />

        <p style="text-align:center; margin-top:16px;">
            Already have an account? <a href="~/Login.aspx" runat="server">Login here</a>
        </p>
    </div>
</asp:Content>
