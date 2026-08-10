using System;

public partial class About : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadStats();
        }
    }

    private void LoadStats()
    {
        litTherapistCount.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM Therapists WHERE ApprovalStatus = 'Approved'").ToString();

        litSessionsCompleted.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM Appointments WHERE Status = 'Completed'").ToString();

        litCategoryCount.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM TherapyCategories").ToString();

        object avgRatingResult = DBHelper.ExecuteScalar("SELECT AVG(Rating) FROM Feedback");
        litAvgRating.Text = (avgRatingResult == null || avgRatingResult == DBNull.Value)
            ? "—"
            : Convert.ToDouble(avgRatingResult).ToString("0.0") + " / 5";
    }
}
