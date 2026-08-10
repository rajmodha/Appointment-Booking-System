<%@ Page Title="Find a Therapist" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="FindTherapist.aspx.cs" Inherits="FindTherapist" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">

    <div style="background:linear-gradient(135deg, var(--mc-lavender), var(--mc-mint)); padding:40px 20px; text-align:center;">
        <h1 style="color:var(--mc-primary-dark); margin-bottom:6px;">Find your therapist</h1>
        <p style="color:var(--mc-muted);">Filter by specialization, location, language and budget to find the right fit.</p>
    </div>

    <div style="max-width:1100px; margin:0 auto; padding:30px 20px;">

        <!-- ============ FILTER BAR ============ -->
        <div class="mc-card">
            <div style="display:flex; gap:16px; flex-wrap:wrap; align-items:flex-end;">

                <div style="flex:1; min-width:180px;">
                    <label>Specialization</label>
                    <asp:DropDownList ID="ddlCategory" runat="server" CssClass="mc-form-control">
                        <asp:ListItem Text="All Specializations" Value="0" />
                    </asp:DropDownList>
                </div>

                <div style="flex:1; min-width:150px;">
                    <label>Location</label>
                    <asp:TextBox ID="txtLocation" runat="server" CssClass="mc-form-control" placeholder="e.g. Mumbai" />
                </div>

                <div style="flex:1; min-width:150px;">
                    <label>Language</label>
                    <asp:TextBox ID="txtLanguage" runat="server" CssClass="mc-form-control" placeholder="e.g. English" />
                </div>

                <div style="flex:1; min-width:150px;">
                    <label>Consultation</label>
                    <asp:DropDownList ID="ddlConsultationType" runat="server" CssClass="mc-form-control">
                        <asp:ListItem Text="Any" Value="" />
                        <asp:ListItem Text="Online" Value="Online" />
                        <asp:ListItem Text="Offline" Value="Offline" />
                    </asp:DropDownList>
                </div>

                <div style="flex:1; min-width:150px;">
                    <label>Max Fee (₹)</label>
                    <asp:TextBox ID="txtMaxFee" runat="server" CssClass="mc-form-control" placeholder="e.g. 1500" TextMode="Number" />
                </div>

                <div>
                    <asp:Button ID="btnSearch" runat="server" Text="Search" CssClass="mc-btn" OnClick="btnSearch_Click" />
                </div>
            </div>
        </div>

        <!-- ============ RESULTS ============ -->
        <div style="margin-top:24px;">
            <asp:Label ID="lblResultCount" runat="server" style="color:var(--mc-muted); font-weight:600;" />

            <asp:Repeater ID="rptTherapists" runat="server" OnItemDataBound="rptTherapists_ItemDataBound">
                <ItemTemplate>
                    <div class="mc-card mc-therapist-card" style="margin-top:16px;">
                        <asp:Image ID="imgTherapist" runat="server" AlternateText="" />
                        <div style="flex:1;">
                            <h3 style="margin:0;"><%# Eval("FullName") %></h3>
                            <p style="margin:4px 0; color:var(--mc-muted);"><%# Eval("Specialization") %> &middot; <%# Eval("Qualification") %></p>
                            <p style="margin:0; font-size:14px; color:var(--mc-muted);">
                                📍 <%# Eval("Location") %> &nbsp;|&nbsp;
                                🗣️ <%# Eval("Language") %> &nbsp;|&nbsp;
                                💻 <%# Eval("ConsultationType") %>
                            </p>
                        </div>
                        <div style="text-align:right;">
                            <p style="font-size:20px; font-weight:700; color:var(--mc-primary); margin:0 0 8px 0;">
                                ₹<%# Eval("Fees") %><span style="font-size:13px; color:var(--mc-muted); font-weight:400;">/session</span>
                            </p>
                            <asp:HyperLink ID="hlViewProfile" runat="server" CssClass="mc-btn">View Profile</asp:HyperLink>
                        </div>
                    </div>
                </ItemTemplate>
            </asp:Repeater>

            <asp:Panel ID="pnlNoResults" runat="server" Visible="false" class="mc-card" style="text-align:center; margin-top:16px;">
                <p style="color:var(--mc-muted);">No therapists matched your filters. Try widening your search.</p>
            </asp:Panel>
        </div>

    </div>
</asp:Content>
