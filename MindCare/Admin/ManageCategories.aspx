<%@ Page Title="Therapy Categories" Language="C#" MasterPageFile="~/Site.Master" AutoEventWireup="true" CodeBehind="ManageCategories.aspx.cs" Inherits="Admin_ManageCategories" %>
<%@ Register Src="~/Admin/AdminSidebar.ascx" TagPrefix="uc" TagName="AdminSidebar" %>

<asp:Content ID="Content1" ContentPlaceHolderID="MainContent" runat="server">
    <div class="mc-dashboard">

        <uc:AdminSidebar ID="AdminSidebar1" runat="server" />

        <div class="mc-main">
            <h2 style="color:var(--mc-primary-dark); margin-top:0;">Therapy Categories</h2>

            <!-- ============ ADD / EDIT FORM ============ -->
            <!-- ValidationGroup="CategoryForm" is a second, independent safeguard on
                 top of CausesValidation="False" below - even if one of the two is ever
                 lost again in a copy-paste, the other still stops the Edit/Delete/Cancel
                 buttons from triggering this form's validator. -->
            <div class="mc-card">
                <h3 style="margin-top:0; color:var(--mc-primary-dark);">
                    <asp:Literal ID="litFormTitle" runat="server" Text="Add a New Category" />
                </h3>

                <!-- Hidden field holds the CategoryId being edited, 0 means "adding new" -->
                <asp:HiddenField ID="hdnCategoryId" runat="server" Value="0" />

                <label>Category Name</label>
                <asp:TextBox ID="txtCategoryName" runat="server" CssClass="mc-form-control" placeholder="e.g. Anxiety & Stress" />
                <asp:RequiredFieldValidator ID="rfvCategoryName" ControlToValidate="txtCategoryName" runat="server"
                    Text="Category name is required" CssClass="text-danger" Display="Dynamic" ValidationGroup="CategoryForm" />

                <label>Description</label>
                <asp:TextBox ID="txtDescription" runat="server" CssClass="mc-form-control" TextMode="MultiLine" Rows="2" placeholder="Short description shown to patients" />

                <asp:Label ID="lblMessage" runat="server" CssClass="text-danger" Display="Dynamic" style="display:block; margin-bottom:10px;" />

                <asp:Button ID="btnSave" runat="server" Text="Add Category" CssClass="mc-btn" OnClick="btnSave_Click" ValidationGroup="CategoryForm" />
                <asp:Button ID="btnCancelEdit" runat="server" Text="Cancel" CssClass="mc-btn mc-btn-outline" OnClick="btnCancelEdit_Click" CausesValidation="False" Visible="false" style="margin-left:8px;" />
            </div>

            <!-- ============ EXISTING CATEGORIES ============ -->
            <div class="mc-card">
                <h3 style="margin-top:0; color:var(--mc-primary-dark);">All Categories</h3>

                <asp:Repeater ID="rptCategories" runat="server" OnItemCommand="rptCategories_ItemCommand">
                    <ItemTemplate>
                        <div style="display:flex; justify-content:space-between; align-items:center; padding:10px 0; border-bottom:1px solid #eee;">
                            <div>
                                <b><%# Eval("CategoryName") %></b>
                                <p style="margin:2px 0 0 0; color:var(--mc-muted); font-size:14px;"><%# Eval("Description") %></p>
                                <p style="margin:2px 0 0 0; color:var(--mc-muted); font-size:13px;"><%# Eval("TherapistCount") %> therapist(s) using this category</p>
                            </div>
                            <div>
                                <asp:LinkButton ID="btnEdit" runat="server" CommandName="Edit" CommandArgument='<%# Eval("CategoryId") %>'
                                    CssClass="mc-btn mc-btn-outline" style="font-size:13px; padding:6px 16px;" CausesValidation="False">Edit</asp:LinkButton>

                                <asp:LinkButton ID="btnDelete" runat="server" CommandName="Delete" CommandArgument='<%# Eval("CategoryId") %>'
                                    CssClass="mc-btn mc-btn-outline" style="font-size:13px; padding:6px 16px; margin-left:6px;" CausesValidation="False"
                                    OnClientClick="return confirm('Delete this category? This only works if no therapist is using it.');">Delete</asp:LinkButton>
                            </div>
                        </div>
                    </ItemTemplate>
                </asp:Repeater>

                <asp:Label ID="lblNoCategories" runat="server" Visible="false" Text="No categories yet - add one above." style="color:var(--mc-muted);" />
            </div>
        </div>
    </div>
</asp:Content>
