using System;
using System.Data;

public partial class Reports : AdminBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminSidebar1.ActivePage = "Reports";

        if (!IsPostBack)
        {
            LoadStatusBreakdown();
            LoadCategoryBreakdown();
            LoadTopTherapists();
        }
    }

    private void LoadStatusBreakdown()
    {
        string query = @"SELECT Status, COUNT(*) AS Total
                          FROM Appointments
                          GROUP BY Status
                          ORDER BY Total DESC";

        DataTable dt = DBHelper.ExecuteSelect(query);
        rptStatusBreakdown.DataSource = dt;
        rptStatusBreakdown.DataBind();
    }

    private void LoadCategoryBreakdown()
    {
        string query = @"SELECT tc.CategoryName, COUNT(a.AppointmentId) AS Total
                          FROM TherapyCategories tc
                          LEFT JOIN Therapists t ON t.CategoryId = tc.CategoryId
                          LEFT JOIN Appointments a ON a.TherapistId = t.TherapistId
                          GROUP BY tc.CategoryId, tc.CategoryName
                          ORDER BY Total DESC";

        DataTable dt = DBHelper.ExecuteSelect(query);
        rptCategoryBreakdown.DataSource = dt;
        rptCategoryBreakdown.DataBind();
    }

    private void LoadTopTherapists()
    {
        string query = @"SELECT u.FullName, AVG(f.Rating) AS AvgRating, COUNT(f.FeedbackId) AS TotalReviews
                          FROM Feedback f
                          INNER JOIN Therapists t ON t.TherapistId = f.TherapistId
                          INNER JOIN Users u ON u.UserId = t.UserId
                          GROUP BY t.TherapistId, u.FullName
                          ORDER BY AvgRating DESC, TotalReviews DESC
                          LIMIT 10";

        DataTable dt = DBHelper.ExecuteSelect(query);

        if (dt.Rows.Count == 0)
        {
            lblNoRatings.Visible = true;
        }
        else
        {
            rptTopTherapists.DataSource = dt;
            rptTopTherapists.DataBind();
        }
    }
}
