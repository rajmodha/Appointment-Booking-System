<%@ Page Title="Therapist Profile" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="TherapistDetails.aspx.cs" Inherits="TherapistDetails" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div style="max-width:900px; margin:30px auto; padding:0 20px;">

        <asp:Panel ID="pnlNotFound" runat="server" Visible="false" class="mc-card" style="text-align:center;">
            <p>This therapist profile could not be found.</p>
            <a href="~/FindTherapist.aspx" runat="server" class="mc-btn">Back to Search</a>
        </asp:Panel>

        <asp:Panel ID="pnlProfile" runat="server">

            <!-- ============ PROFILE HEADER ============ -->
            <div class="mc-card" style="display:flex; gap:24px; flex-wrap:wrap; align-items:center;">
                <asp:Image ID="imgTherapist" runat="server" style="width:120px;height:120px;border-radius:50%;object-fit:cover;border:3px solid var(--mc-mint);" />
                <div style="flex:1; min-width:220px;">
                    <h2 style="margin:0; color:var(--mc-primary-dark);"><asp:Literal ID="litName" runat="server" /></h2>
                    <p style="margin:4px 0; color:var(--mc-muted);"><asp:Literal ID="litQualification" runat="server" /></p>
                    <p style="margin:0;"><asp:Literal ID="litRating" runat="server" /></p>
                </div>
                <div style="text-align:right;">
                    <p style="font-size:26px; font-weight:700; color:var(--mc-primary); margin:0;">
                        ₹<asp:Literal ID="litFees" runat="server" /><span style="font-size:14px; color:var(--mc-muted); font-weight:400;">/session</span>
                    </p>
                    <asp:HyperLink ID="hlBookNow" runat="server" CssClass="mc-btn" style="margin-top:10px; display:inline-block;">Book Appointment</asp:HyperLink>
                </div>
            </div>

            <!-- ============ ABOUT ============ -->
            <div class="mc-card" style="margin-top:20px;">
                <h3 style="color:var(--mc-primary-dark);">About</h3>
                <p><asp:Literal ID="litBio" runat="server" /></p>
                <p>
                    <b>Specialization:</b> <asp:Literal ID="litSpecialization" runat="server" /><br />
                    <b>Language:</b> <asp:Literal ID="litLanguage" runat="server" /><br />
                    <b>Location:</b> <asp:Literal ID="litLocation" runat="server" /><br />
                    <b>Consultation Mode:</b> <asp:Literal ID="litConsultationType" runat="server" />
                </p>
            </div>

            <!-- ============ AVAILABILITY ============ -->
            <div class="mc-card" style="margin-top:20px;">
                <h3 style="color:var(--mc-primary-dark);">Weekly Availability</h3>
                <asp:Repeater ID="rptAvailability" runat="server">
                    <HeaderTemplate><table style="width:100%; border-collapse:collapse;"><tbody></HeaderTemplate>
                    <ItemTemplate>
                        <tr style="border-bottom:1px solid #eee;">
                            <td style="padding:8px 0; font-weight:600;"><%# Eval("DayOfWeek") %></td>
                            <td style="padding:8px 0; text-align:right; color:var(--mc-muted);">
                                <%# Eval("StartTime") %> - <%# Eval("EndTime") %>
                            </td>
                        </tr>
                    </ItemTemplate>
                    <FooterTemplate></tbody></table></FooterTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoAvailability" runat="server" Visible="false" Text="This therapist hasn't set their availability yet." style="color:var(--mc-muted);" />
            </div>

            <!-- ============ FEEDBACK ============ -->
            <div class="mc-card" style="margin-top:20px;">
                <h3 style="color:var(--mc-primary-dark);">Patient Feedback</h3>
                <asp:Repeater ID="rptFeedback" runat="server">
                    <ItemTemplate>
                        <div style="border-bottom:1px solid #eee; padding:10px 0;">
                            <b><%# Eval("Rating") %> / 5 ⭐</b>
                            <p style="margin:4px 0 0 0; color:var(--mc-muted);"><%# Eval("Comments") %></p>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>
                <asp:Label ID="lblNoFeedback" runat="server" Visible="false" Text="No feedback yet." style="color:var(--mc-muted);" />
            </div>

        </asp:Panel>
    </div>
</asp:Content>
