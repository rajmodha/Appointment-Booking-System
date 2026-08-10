using System;
using System.IO;

public partial class SiteMaster : System.Web.UI.MasterPage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        HighlightActiveNavLink();

        // If a UserId is stored in session, the person is logged in.
        if (Session["UserId"] != null)
        {
            phGuestLinks.Visible = false;
            phLoggedInLinks.Visible = true;

            string roleId = Session["RoleId"].ToString();
            litAccountName.Text = Session["FullName"] != null ? Session["FullName"].ToString() : "Account";

            // Send "Dashboard" and "My Profile" to the correct page for each role.
            if (roleId == "1")
            {
                hlDashboard.Attributes["href"] = ResolveUrl("~/Admin/AdminDashboard.aspx");
                hlProfile.Attributes["href"] = ResolveUrl("~/Admin/MyProfile.aspx");
            }
            else if (roleId == "2")
            {
                hlDashboard.Attributes["href"] = ResolveUrl("~/Therapist/TherapistDashboard.aspx");
                hlProfile.Attributes["href"] = ResolveUrl("~/Therapist/Profile.aspx");
            }
            else
            {
                hlDashboard.Attributes["href"] = ResolveUrl("~/Patient/PatientDashboard.aspx");
                hlProfile.Attributes["href"] = ResolveUrl("~/Patient/MyProfile.aspx");
            }
        }
    }

    /// <summary>
    /// Same pattern as the dashboard sidebars (AdminSidebar.ascx.cs etc.),
    /// but instead of every content page having to set an "ActivePage"
    /// property, the master page figures it out itself from the currently
    /// requested page's filename - since Site.Master is shared by every
    /// page site-wide, this one method covers Home/About/Find Therapist/
    /// Contact everywhere automatically with no changes needed to any of
    /// those individual .aspx files.
    /// </summary>
    private void HighlightActiveNavLink()
    {
        // Page.AppRelativeVirtualPath is something like "~/Default.aspx" or
        // "~/About.aspx" - GetFileName pulls out just "Default.aspx" etc.
        string currentPage = Path.GetFileName(Page.AppRelativeVirtualPath).ToLowerInvariant();

        if (currentPage == "default.aspx")
        {
            hlHome.Attributes["class"] = "active";
        }
        else if (currentPage == "about.aspx")
        {
            hlAbout.Attributes["class"] = "active";
        }
        else if (currentPage == "findtherapist.aspx" || currentPage == "therapistdetails.aspx")
        {
            // TherapistDetails.aspx is reached FROM Find Therapist, so it
            // makes sense to keep that link highlighted while browsing a
            // specific therapist's profile too.
            hlFindTherapistNav.Attributes["class"] = "active";
        }
        else if (currentPage == "contact.aspx")
        {
            hlContactNav.Attributes["class"] = "active";
        }
    }

    protected void lnkLogout_Click(object sender, EventArgs e)
    {
        Session.Clear();
        Session.Abandon();
        Response.Redirect("~/Default.aspx");
    }
}
