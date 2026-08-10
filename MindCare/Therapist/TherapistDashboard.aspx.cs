using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class TherapistDashboard : TherapistBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TherapistSidebar1.ActivePage = "Dashboard";

        if (!IsPostBack)
        {
            LoadStats();
            LoadRecentRequests();
        }
    }

    /// <summary>
    /// Every Therapist page needs the TherapistId (not the same as UserId - the
    /// Therapists table has its own primary key). Re-fetched fresh each request
    /// rather than cached in a field, same lesson learned from the BookAppointment
    /// fee bug earlier.
    /// </summary>
    private int GetTherapistId()
    {
        string query = "SELECT TherapistId FROM Therapists WHERE UserId = @UserId";
        object result = DBHelper.ExecuteScalar(query, new MySqlParameter("@UserId", CurrentUserId));
        return result == null ? 0 : Convert.ToInt32(result);
    }

    private void LoadStats()
    {
        int therapistId = GetTherapistId();

        // Same rule as Requests.aspx: an appointment only "counts" here once its
        // payment has been verified by Admin - otherwise this number wouldn't
        // match what the therapist can actually see and act on.
        string query = @"SELECT
                            SUM(CASE WHEN a.Status = 'Pending' THEN 1 ELSE 0 END) AS Pending,
                            SUM(CASE WHEN a.Status = 'Confirmed' THEN 1 ELSE 0 END) AS Upcoming,
                            SUM(CASE WHEN a.Status = 'Completed' THEN 1 ELSE 0 END) AS Completed
                          FROM Appointments a
                          INNER JOIN Payments pay ON pay.AppointmentId = a.AppointmentId
                          WHERE a.TherapistId = @TherapistId AND pay.PaymentStatus = 'Success'";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@TherapistId", therapistId));
        DataRow row = dt.Rows[0];

        litPending.Text = row["Pending"] == DBNull.Value ? "0" : row["Pending"].ToString();
        litUpcoming.Text = row["Upcoming"] == DBNull.Value ? "0" : row["Upcoming"].ToString();
        litCompleted.Text = row["Completed"] == DBNull.Value ? "0" : row["Completed"].ToString();

        string ratingQuery = "SELECT AVG(Rating) FROM Feedback WHERE TherapistId = @TherapistId";
        object avgResult = DBHelper.ExecuteScalar(ratingQuery, new MySqlParameter("@TherapistId", therapistId));
        litRating.Text = avgResult == DBNull.Value || avgResult == null ? "—" : Convert.ToDouble(avgResult).ToString("0.0");
    }

    private void LoadRecentRequests()
    {
        int therapistId = GetTherapistId();

        string query = @"SELECT u.FullName AS PatientName, a.AppointmentDate,
                                 TIME_FORMAT(a.AppointmentTime, '%h:%i %p') AS DisplayTime,
                                 a.ConsultationType, a.Status
                          FROM Appointments a
                          INNER JOIN Patients p ON p.PatientId = a.PatientId
                          INNER JOIN Users u ON u.UserId = p.UserId
                          INNER JOIN Payments pay ON pay.AppointmentId = a.AppointmentId
                          WHERE a.TherapistId = @TherapistId AND pay.PaymentStatus = 'Success'
                          ORDER BY a.CreatedOn DESC
                          LIMIT 5";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@TherapistId", therapistId));

        if (dt.Rows.Count == 0)
        {
            lblNoRequests.Visible = true;
        }
        else
        {
            rptRecent.DataSource = dt;
            rptRecent.DataBind();
        }
    }
}
