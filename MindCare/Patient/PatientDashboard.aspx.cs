using System;
using System.Data;
using MySql.Data.MySqlClient;

/// <summary>
/// Inherits PatientBasePage - see Helpers/BasePages.cs. This automatically
/// redirects anyone who isn't logged in as a Patient straight to Login
/// before any code on this page runs.
/// </summary>
public partial class PatientDashboard : PatientBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PatientSidebar1.ActivePage = "Dashboard";

        if (!IsPostBack)
        {
            LoadStats();
            LoadRecentAppointments();
        }
    }

    private int GetPatientId()
    {
        string query = "SELECT PatientId FROM Patients WHERE UserId = @UserId";
        object result = DBHelper.ExecuteScalar(query, new MySqlParameter("@UserId", CurrentUserId));
        return result == null ? 0 : Convert.ToInt32(result);
    }

    private void LoadStats()
    {
        int patientId = GetPatientId();

        string query = @"SELECT
                            COUNT(*) AS Total,
                            SUM(CASE WHEN Status IN ('Pending','Confirmed','Rescheduled') THEN 1 ELSE 0 END) AS Upcoming,
                            SUM(CASE WHEN Status = 'Completed' THEN 1 ELSE 0 END) AS Completed,
                            SUM(CASE WHEN Status IN ('Cancelled','Rejected') THEN 1 ELSE 0 END) AS Cancelled
                          FROM Appointments
                          WHERE PatientId = @PatientId";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@PatientId", patientId));
        DataRow row = dt.Rows[0];

        litTotal.Text = row["Total"] == DBNull.Value ? "0" : row["Total"].ToString();
        litUpcoming.Text = row["Upcoming"] == DBNull.Value ? "0" : row["Upcoming"].ToString();
        litCompleted.Text = row["Completed"] == DBNull.Value ? "0" : row["Completed"].ToString();
        litCancelled.Text = row["Cancelled"] == DBNull.Value ? "0" : row["Cancelled"].ToString();
    }

    private void LoadRecentAppointments()
    {
        int patientId = GetPatientId();

        string query = @"SELECT u.FullName AS TherapistName, a.AppointmentDate, a.AppointmentTime, a.Status,
                                 TIME_FORMAT(a.AppointmentTime, '%h:%i %p') AS DisplayTime
                          FROM Appointments a
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users u ON u.UserId = t.UserId
                          WHERE a.PatientId = @PatientId
                          ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC
                          LIMIT 5";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@PatientId", patientId));

        if (dt.Rows.Count == 0)
        {
            lblNoAppointments.Visible = true;
        }
        else
        {
            rptRecent.DataSource = dt;
            rptRecent.DataBind();
        }
    }
}
