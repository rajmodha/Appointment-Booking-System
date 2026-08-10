<%@ Page Title="Contact Us" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Contact.aspx.cs" Inherits="Contact" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div style="max-width:600px; margin:40px auto; padding:0 20px;">
        <div class="mc-card">
            <h2 style="color:var(--mc-primary-dark); text-align:center;">Get in touch</h2>
            <p style="text-align:center; color:var(--mc-muted);">Questions, feedback, or need help? Send us a message.</p>

            <asp:Panel ID="pnlForm" runat="server">
                <label>Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="mc-form-control" />
                <asp:RequiredFieldValidator ID="rfvFullName" ControlToValidate="txtFullName" runat="server" Text="Name is required" CssClass="text-danger" Display="Dynamic" />

                <label>Email Address</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="mc-form-control" />
                <asp:RequiredFieldValidator ID="rfvEmail" ControlToValidate="txtEmail" runat="server" Text="Email is required" CssClass="text-danger" Display="Dynamic" />
                <asp:RegularExpressionValidator ID="revEmail" ControlToValidate="txtEmail" runat="server"
                    ValidationExpression="^[^@\s]+@[^@\s]+\.[^@\s]+$" Text="Enter a valid email" CssClass="text-danger" Display="Dynamic" />

                <label>Subject</label>
                <asp:TextBox ID="txtSubject" runat="server" CssClass="mc-form-control" placeholder="e.g. Question about booking" />

                <label>Message</label>
                <asp:TextBox ID="txtMessage" runat="server" CssClass="mc-form-control" TextMode="MultiLine" Rows="5" />
                <asp:RequiredFieldValidator ID="rfvMessage" ControlToValidate="txtMessage" runat="server" Text="Please enter a message" CssClass="text-danger" Display="Dynamic" />

                <asp:Button ID="btnSend" runat="server" Text="Send Message" CssClass="mc-btn" Width="100%" OnClick="btnSend_Click" />
            </asp:Panel>

            <asp:Panel ID="pnlThankYou" runat="server" Visible="false" style="text-align:center;">
                <h3 style="color:var(--mc-accent);">Message sent! 💌</h3>
                <p style="color:var(--mc-muted);">We'll get back to you as soon as we can.</p>
            </asp:Panel>
        </div>
    </div>
</asp:Content>
