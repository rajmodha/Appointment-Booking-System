<%@ Page Title="My Profile" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="Profile.aspx.cs" Inherits="Therapist_Profile" %>
<%@ Register Src="~/Therapist/TherapistSidebar.ascx" TagPrefix="uc" TagName="TherapistSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:TherapistSidebar ID="TherapistSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">My Profile</h2>

            <div class="mc-card">
                <h3 style="margin-top:0; color:var(--mc-primary-dark);">Profile Photo</h3>

                <asp:Image ID="imgCurrentPhoto" runat="server" style="width:110px;height:110px;border-radius:50%;object-fit:cover;border:3px solid var(--mc-mint);display:block;margin-bottom:16px;" />

                <asp:FileUpload ID="fileProfilePhoto" runat="server" CssClass="mc-form-control" />
                <p style="margin:8px 0 0 0; font-size:13px; color:var(--mc-muted);">
                    JPG, PNG, GIF or WEBP, up to 2 MB. Choosing a new photo replaces your current one when you click Save Profile below.
                </p>
                <asp:Label ID="lblPhotoError" runat="server" CssClass="text-danger" Display="Dynamic" style="display:block; margin-top:8px;" />
            </div>

            <div class="mc-card">
                <h3 style="margin-top:0; color:var(--mc-primary-dark);">Personal Details</h3>

                <label>Email</label>
                <asp:TextBox ID="txtEmail" runat="server" CssClass="mc-form-control" Enabled="false" />
                <p style="margin:-10px 0 16px 0; font-size:13px; color:var(--mc-muted);">Your email is used to log in and can't be changed here.</p>

                <label>Full Name</label>
                <asp:TextBox ID="txtFullName" runat="server" CssClass="mc-form-control" />
                <asp:RequiredFieldValidator ID="rfvFullName" ControlToValidate="txtFullName" runat="server" Text="Full name is required" CssClass="text-danger" Display="Dynamic" />

                <label>Phone Number</label>
                <asp:TextBox ID="txtPhone" runat="server" CssClass="mc-form-control" />
            </div>

            <div class="mc-card">
                <p style="color:var(--mc-muted); margin-top:0;">
                    This information is shown to patients on your public profile.
                    Approval status: <asp:Literal ID="litApprovalStatus" runat="server" />
                </p>

                <label>Category / Specialization Area</label>
                <asp:DropDownList ID="ddlCategory" runat="server" CssClass="mc-form-control" />

                <label>Specialization (short description)</label>
                <asp:TextBox ID="txtSpecialization" runat="server" CssClass="mc-form-control" placeholder="e.g. Anxiety, Stress Management" />

                <label>Qualification</label>
                <asp:TextBox ID="txtQualification" runat="server" CssClass="mc-form-control" placeholder="e.g. M.Phil Clinical Psychology" />

                <label>Language(s) Spoken</label>
                <asp:TextBox ID="txtLanguage" runat="server" CssClass="mc-form-control" placeholder="e.g. English, Hindi" />

                <label>Location</label>
                <asp:TextBox ID="txtLocation" runat="server" CssClass="mc-form-control" placeholder="e.g. Mumbai" />

                <label>Consultation Fee (₹ per session)</label>
                <asp:TextBox ID="txtFees" runat="server" CssClass="mc-form-control" TextMode="Number" placeholder="e.g. 800" />

                <label>Consultation Mode</label>
                <asp:DropDownList ID="ddlConsultationType" runat="server" CssClass="mc-form-control">
                    <asp:ListItem Text="Online Only" Value="Online" />
                    <asp:ListItem Text="Offline Only" Value="Offline" />
                    <asp:ListItem Text="Both Online and Offline" Value="Both" />
                </asp:DropDownList>

                <label>Bio (shown on your public profile)</label>
                <asp:TextBox ID="txtBio" runat="server" CssClass="mc-form-control" TextMode="MultiLine" Rows="4" placeholder="Tell patients a little about your approach and experience..." />

                <asp:Label ID="lblMessage" runat="server" CssClass="text-success" Display="Dynamic" style="display:block; margin-bottom:10px;" />

                <asp:Button ID="btnSave" runat="server" Text="Save Profile" CssClass="mc-btn" OnClick="btnSave_Click" />
            </div>
        </div>
    </div>
</asp:Content>
