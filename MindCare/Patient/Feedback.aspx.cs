using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class Patient_Feedback : PatientBasePage
{
    private int appointmentId;
    private int therapistId;

    protected void Page_Load(object sender, EventArgs e)
    {
        if (!int.TryParse(Request.QueryString["appointmentId"], out appointmentId))
        {
            ShowInvalid("This appointment could not be found.");
            return;
        }

        if (!IsPostBack)
        {
            LoadAppointment();
        }
    }

    private void ShowInvalid(string reason)
    {
        pnlForm.Visible = false;
        pnlInvalid.Visible = true;
        litInvalidReason.Text = reason;
    }

    /// <summary>
    /// Confirms the appointment (a) belongs to the logged-in patient,
    /// (b) is actually Completed, and (c) doesn't already have feedback -
    /// all three checked server-side so this can't be bypassed by editing
    /// the URL directly.
    /// </summary>
    private void LoadAppointment()
    {
        int patientId = GetPatientId();

        string query = @"SELECT a.TherapistId, a.Status, a.AppointmentDate, u.FullName AS TherapistName,
                                 EXISTS(SELECT 1 FROM Feedback f WHERE f.AppointmentId = a.AppointmentId) AS HasFeedback
                          FROM Appointments a
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users u ON u.UserId = t.UserId
                          WHERE a.AppointmentId = @AppointmentId AND a.PatientId = @PatientId";

        DataTable dt = DBHelper.ExecuteSelect(query,
            new MySqlParameter("@AppointmentId", appointmentId),
            new MySqlParameter("@PatientId", patientId));

        if (dt.Rows.Count == 0)
        {
            ShowInvalid("This appointment could not be found.");
            return;
        }

        DataRow row = dt.Rows[0];

        if (row["Status"].ToString() != "Completed")
        {
            ShowInvalid("You can only leave feedback after the session is marked Completed.");
            return;
        }

        if (Convert.ToBoolean(row["HasFeedback"]))
        {
            ShowInvalid("You've already left feedback for this appointment.");
            return;
        }

        therapistId = Convert.ToInt32(row["TherapistId"]);
        litTherapistName.Text = row["TherapistName"].ToString();
        litSessionDate.Text = Convert.ToDateTime(row["AppointmentDate"]).ToString("dd MMM yyyy");
    }

    private int GetPatientId()
    {
        string query = "SELECT PatientId FROM Patients WHERE UserId = @UserId";
        object result = DBHelper.ExecuteScalar(query, new MySqlParameter("@UserId", CurrentUserId));
        return result == null ? 0 : Convert.ToInt32(result);
    }

    protected void btnSubmit_Click(object sender, EventArgs e)
    {
        int patientId = GetPatientId();

        // Re-check therapistId in case ViewState/postback lost it (defensive - therapistId
        // is a plain field, so re-derive it fresh rather than trusting it survived).
        if (therapistId == 0)
        {
            string query = "SELECT TherapistId FROM Appointments WHERE AppointmentId = @AppointmentId AND PatientId = @PatientId";
            object result = DBHelper.ExecuteScalar(query,
                new MySqlParameter("@AppointmentId", appointmentId),
                new MySqlParameter("@PatientId", patientId));

            if (result == null)
            {
                lblMessage.Text = "Something went wrong - please try again from My Appointments.";
                return;
            }
            therapistId = Convert.ToInt32(result);
        }

        int rating = Convert.ToInt32(rblRating.SelectedValue);
        string comments = txtComments.Text.Trim();

        string insertQuery = @"INSERT INTO Feedback (AppointmentId, PatientId, TherapistId, Rating, Comments)
                                VALUES (@AppointmentId, @PatientId, @TherapistId, @Rating, @Comments)";

        DBHelper.ExecuteNonQuery(insertQuery,
            new MySqlParameter("@AppointmentId", appointmentId),
            new MySqlParameter("@PatientId", patientId),
            new MySqlParameter("@TherapistId", therapistId),
            new MySqlParameter("@Rating", rating),
            new MySqlParameter("@Comments", string.IsNullOrEmpty(comments) ? (object)DBNull.Value : comments));

        pnlForm.Visible = false;
        pnlThankYou.Visible = true;
    }
}
