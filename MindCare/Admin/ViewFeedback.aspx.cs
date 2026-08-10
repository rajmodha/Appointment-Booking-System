using System;
using System.Data;

public partial class ViewFeedback : AdminBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminSidebar1.ActivePage = "Feedback";

        if (!IsPostBack)
        {
            LoadFeedback();
        }
    }

    private void LoadFeedback()
    {
        string query = @"SELECT pu.FullName AS PatientName, tu.FullName AS TherapistName,
                                 f.Rating, f.Comments, f.CreatedOn
                          FROM Feedback f
                          INNER JOIN Patients p ON p.PatientId = f.PatientId
                          INNER JOIN Users pu ON pu.UserId = p.UserId
                          INNER JOIN Therapists t ON t.TherapistId = f.TherapistId
                          INNER JOIN Users tu ON tu.UserId = t.UserId
                          ORDER BY f.CreatedOn DESC";

        DataTable dt = DBHelper.ExecuteSelect(query);

        if (dt.Rows.Count == 0)
        {
            pnlNoFeedback.Visible = true;
            rptFeedback.Visible = false;
        }
        else
        {
            rptFeedback.DataSource = dt;
            rptFeedback.DataBind();
        }
    }
}
