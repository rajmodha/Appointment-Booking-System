using System;

/// <summary>
/// The parent page sets ActivePage (in its own Page_Load, before this control
/// renders) so the sidebar knows which link to highlight, e.g.:
///     PatientSidebar1.ActivePage = "Dashboard";
/// </summary>
public partial class PatientSidebar : System.Web.UI.UserControl
{
    public string ActivePage { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        litPatientName.Text = Session["FullName"] != null ? Session["FullName"].ToString() : "Patient";

        if (ActivePage == "Dashboard")
            lnkDashboard.Attributes["class"] = "active";
        else if (ActivePage == "Appointments")
            lnkAppointments.Attributes["class"] = "active";
        else if (ActivePage == "Calendar")
            lnkCalendar.Attributes["class"] = "active";
        else if (ActivePage == "Profile")
            lnkProfile.Attributes["class"] = "active";
    }
}
