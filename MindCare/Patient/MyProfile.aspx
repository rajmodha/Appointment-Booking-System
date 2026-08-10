<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="MyProfile.aspx.cs" Inherits="Patient_MyProfile" %>
<%@ Register Src="~/Patient/PatientSidebar.ascx" TagPrefix="uc" TagName="PatientSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:PatientSidebar ID="PatientSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">My Profile</h2>

            <div class="mc-card">
                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="mc-form-control" Enabled="false" />
                <p style="margin:-10px 0 16px 0; font-size:13px; color:var(--mc-muted);">Your email is used to log in and can't be changed here.</p>

                <label>Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="mc-form-control" />
                <asp:RequiredFieldValidator ID="rfvFullName" ControlToValidate="txtFullName" runat="server" Text="Full name is required" CssClass="text-danger" Display="Dynamic" />

                <label>Phone Number</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="mc-form-control" />

                <label>Date of Birth</label>
                <asp:TextBox ID="txtDateOfBirth" runat="server" CssClass="mc-form-control" TextMode="Date" />

                <label>Gender</label>
                <asp:DropDownList ID="ddlGender" runat="server" CssClass="mc-form-control">
                    <asp:ListItem Text="-- Select --" Value="" />
                    <asp:ListItem Text="Male" Value="Male" />
                    <asp:ListItem Text="Female" Value="Female" />
                    <asp:ListItem Text="Other" Value="Other" />
                </asp:DropDownList>

                <label>Address</label>
                <asp:TextBox ID="txtAddress" runat="server" CssClass="mc-form-control" TextMode="MultiLine" Rows="2" />

                <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Display="Dynamic" style="display:block; margin-bottom:10px;" />

                <asp:Button ID="btnSave" runat="server" Text="Save Changes" CssClass="mc-btn" OnClick="btnSave_Click" />
            </div>
        </div>
    </div>
</asp:Content>
