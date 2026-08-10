using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class MyAppointments : PatientBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PatientSidebar1.ActivePage = "Appointments";

        if (!IsPostBack)
        {
            LoadAppointments();
        }
    }

    private int GetPatientId()
    {
        string query = "SELECT PatientId FROM Patients WHERE UserId = @UserId";
        object result = DBHelper.ExecuteScalar(query, new MySqlParameter("@UserId", CurrentUserId));
        return result == null ? 0 : Convert.ToInt32(result);
    }

    private void LoadAppointments()
    {
        int patientId = GetPatientId();
        string statusFilter = ddlStatusFilter.SelectedValue;

        string query = @"SELECT a.AppointmentId, u.FullName AS TherapistName, a.AppointmentDate,
                                 TIME_FORMAT(a.AppointmentTime, '%h:%i %p') AS DisplayTime,
                                 a.ConsultationType, a.MeetingLink, a.Status, a.Amount,
                                 pay.PaymentStatus,
                                 EXISTS(SELECT 1 FROM Feedback f WHERE f.AppointmentId = a.AppointmentId) AS HasFeedback
                          FROM Appointments a
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users u ON u.UserId = t.UserId
                          INNER JOIN Payments pay ON pay.AppointmentId = a.AppointmentId
                          WHERE a.PatientId = @PatientId ";

        if (!string.IsNullOrEmpty(statusFilter))
            query += " AND a.Status = @Status ";

        query += " ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC ";

        DataTable dt;
        if (!string.IsNullOrEmpty(statusFilter))
        {
            dt = DBHelper.ExecuteSelect(query,
                new MySqlParameter("@PatientId", patientId),
                new MySqlParameter("@Status", statusFilter));
        }
        else
        {
            dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@PatientId", patientId));
        }

        if (dt.Rows.Count == 0)
        {
            pnlNoAppointments.Visible = true;
            rptAppointments.Visible = false;
        }
        else
        {
            pnlNoAppointments.Visible = false;
            rptAppointments.Visible = true;
            rptAppointments.DataSource = dt;
            rptAppointments.DataBind();
        }
    }

    protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadAppointments();
    }

    /// <summary>
    /// Fires when the "Cancel" LinkButton inside the Repeater is clicked.
    /// Only Pending/Confirmed appointments can reach this (the button is
    /// hidden for every other status in the markup), but we double-check
    /// server-side too since client-side visibility can't be trusted alone.
    /// </summary>
    protected void rptAppointments_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "Cancel") return;

        int appointmentId = Convert.ToInt32(e.CommandArgument);
        int patientId = GetPatientId();

        // Capture whether it was already Confirmed BEFORE cancelling, since we
        // need that for the email wording afterward (and the row disappears
        // into 'Cancelled' the instant the UPDATE below runs).
        string previousStatusQuery = "SELECT Status FROM Appointments WHERE AppointmentId = @AppointmentId AND PatientId = @PatientId";
        object previousStatusResult = DBHelper.ExecuteScalar(previousStatusQuery,
            new MySqlParameter("@AppointmentId", appointmentId),
            new MySqlParameter("@PatientId", patientId));
        bool wasConfirmed = previousStatusResult != null && previousStatusResult.ToString() == "Confirmed";

        // The WHERE clause makes sure a patient can only cancel their OWN
        // appointment, and only while it's still Pending or Confirmed.
        string query = @"UPDATE Appointments
                          SET Status = 'Cancelled'
                          WHERE AppointmentId = @AppointmentId AND PatientId = @PatientId
                            AND Status IN ('Pending','Confirmed')";

        int rowsAffected = DBHelper.ExecuteNonQuery(query,
            new MySqlParameter("@AppointmentId", appointmentId),
            new MySqlParameter("@PatientId", patientId));

        if (rowsAffected > 0)
        {
            SendCancellationEmails(appointmentId, wasConfirmed);
            lblMessage.CssClass = "text-success";
            lblMessage.Text = "Appointment cancelled. The therapist has been notified" +
                               (wasConfirmed ? ", and Admin has been notified about your refund." : ".");
        }
        else
        {
            lblMessage.CssClass = "text-danger";
            lblMessage.Text = "This appointment could not be cancelled (it may already be completed or cancelled).";
        }

        LoadAppointments();
    }

    /// <summary>
    /// Notifies the therapist (informational - their slot just freed up) and
    /// every Admin account (actionable only if payment was already verified,
    /// since a patient can cancel before Admin ever gets to that step).
    /// </summary>
    private void SendCancellationEmails(int appointmentId, bool wasConfirmed)
    {
        string query = @"SELECT pu.FullName AS PatientName, pu.Email AS PatientEmail,
                                 tu.FullName AS TherapistName, tu.Email AS TherapistEmail,
                                 a.AppointmentDate, a.AppointmentTime, a.Amount,
                                 pay.PaymentStatus, pay.UpiTransactionRef
                          FROM Appointments a
                          INNER JOIN Patients p ON p.PatientId = a.PatientId
                          INNER JOIN Users pu ON pu.UserId = p.UserId
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users tu ON tu.UserId = t.UserId
                          INNER JOIN Payments pay ON pay.AppointmentId = a.AppointmentId
                          WHERE a.AppointmentId = @AppointmentId";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@AppointmentId", appointmentId));
        if (dt.Rows.Count == 0) return;

        DataRow row = dt.Rows[0];
        string patientName = row["PatientName"].ToString();
        string patientEmail = row["PatientEmail"].ToString();
        string therapistName = row["TherapistName"].ToString();
        DateTime appointmentDate = Convert.ToDateTime(row["AppointmentDate"]);
        string displayTime = DateTime.Today.Add((TimeSpan)row["AppointmentTime"]).ToString("h:mm tt");
        decimal amount = Convert.ToDecimal(row["Amount"]);
        bool paymentWasVerified = row["PaymentStatus"].ToString() == "Success";
        string upiRef = row["UpiTransactionRef"] == DBNull.Value ? "(not recorded)" : row["UpiTransactionRef"].ToString();

        // Email the therapist - purely informational.
        string therapistBody = EmailHelper.BuildTherapistPatientCancelledBody(
            therapistName, patientName, appointmentDate, displayTime, wasConfirmed);

        EmailHelper.SendEmail(row["TherapistEmail"].ToString(), "MindCare - A Session Was Cancelled", therapistBody);

        // Email every Admin account - wording differs depending on whether
        // payment had actually been verified yet.
        string adminBody = EmailHelper.BuildAdminPatientCancelNoticeBody(
            patientName, patientEmail, therapistName, appointmentDate, displayTime,
            amount, upiRef, paymentWasVerified);

        string adminEmailQuery = "SELECT Email FROM Users WHERE RoleId = 1 AND IsActive = 1";
        DataTable adminEmails = DBHelper.ExecuteSelect(adminEmailQuery);

        foreach (DataRow adminRow in adminEmails.Rows)
        {
            EmailHelper.SendEmail(adminRow["Email"].ToString(),
                paymentWasVerified ? "MindCare - Refund Needed (Patient Cancelled)" : "MindCare - Appointment Cancelled by Patient",
                adminBody);
        }
    }
}
