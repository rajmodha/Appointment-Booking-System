using System;

/// <summary>
/// The parent page sets ActivePage before this control renders, e.g.:
///     TherapistSidebar1.ActivePage = "Requests";
/// </summary>
public partial class TherapistSidebar : System.Web.UI.UserControl
{
    public string ActivePage { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        litTherapistName.Text = Session["FullName"] != null ? Session["FullName"].ToString() : "Therapist";

        if (ActivePage == "Dashboard") lnkDashboard.Attributes["class"] = "active";
        else if (ActivePage == "Requests") lnkRequests.Attributes["class"] = "active";
        else if (ActivePage == "Calendar") lnkCalendar.Attributes["class"] = "active";
        else if (ActivePage == "Availability") lnkAvailability.Attributes["class"] = "active";
        else if (ActivePage == "Profile") lnkProfile.Attributes["class"] = "active";
    }
}
