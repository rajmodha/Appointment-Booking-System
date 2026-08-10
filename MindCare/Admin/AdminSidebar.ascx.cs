using System;

public partial class AdminSidebar : System.Web.UI.UserControl
{
    public string ActivePage { get; set; }

    protected void Page_Load(object sender, EventArgs e)
    {
        if (ActivePage == "Dashboard") lnkDashboard.Attributes["class"] = "active";
        else if (ActivePage == "Therapists") lnkTherapists.Attributes["class"] = "active";
        else if (ActivePage == "Patients") lnkPatients.Attributes["class"] = "active";
        else if (ActivePage == "Payments") lnkPayments.Attributes["class"] = "active";
        else if (ActivePage == "Refunds") lnkRefunds.Attributes["class"] = "active";
        else if (ActivePage == "Appointments") lnkAppointments.Attributes["class"] = "active";
        else if (ActivePage == "Categories") lnkCategories.Attributes["class"] = "active";
        else if (ActivePage == "Messages") lnkMessages.Attributes["class"] = "active";
        else if (ActivePage == "Feedback") lnkFeedback.Attributes["class"] = "active";
        else if (ActivePage == "Reports") lnkReports.Attributes["class"] = "active";
        else if (ActivePage == "Profile") lnkProfile.Attributes["class"] = "active";
    }
}
