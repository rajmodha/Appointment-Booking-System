using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.UI.WebControls;

public partial class Therapist_Requests : TherapistBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TherapistSidebar1.ActivePage = "Requests";

        if (!IsPostBack)
        {
            LoadRequests();
        }
    }

    private int GetTherapistId()
    {
        string query = "SELECT TherapistId FROM Therapists WHERE UserId = @UserId";
        object result = DBHelper.ExecuteScalar(query, new MySqlParameter("@UserId", CurrentUserId));
        return result == null ? 0 : Convert.ToInt32(result);
    }

    private void LoadRequests()
    {
        int therapistId = GetTherapistId();
        string statusFilter = ddlStatusFilter.SelectedValue;

        // A therapist should never see a request until Admin has verified the
        // patient's UPI transaction reference (see Admin/VerifyPayments.aspx).
        // Until then, the INNER JOIN + WHERE below simply excludes it entirely.
        string query = @"SELECT a.AppointmentId, u.FullName AS PatientName, u.Phone AS PatientPhone,
                                 u.Email AS PatientEmail, a.AppointmentDate,
                                 TIME_FORMAT(a.AppointmentTime, '%h:%i %p') AS DisplayTime,
                                 a.ConsultationType, a.MeetingLink, a.Status, a.Amount, a.Notes
                          FROM Appointments a
                          INNER JOIN Patients p ON p.PatientId = a.PatientId
                          INNER JOIN Users u ON u.UserId = p.UserId
                          INNER JOIN Payments pay ON pay.AppointmentId = a.AppointmentId
                          WHERE a.TherapistId = @TherapistId
                            AND pay.PaymentStatus = 'Success' ";

        if (!string.IsNullOrEmpty(statusFilter))
            query += " AND a.Status = @Status ";

        query += " ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC ";

        DataTable dt;
        if (!string.IsNullOrEmpty(statusFilter))
        {
            dt = DBHelper.ExecuteSelect(query,
                new MySqlParameter("@TherapistId", therapistId),
                new MySqlParameter("@Status", statusFilter));
        }
        else
        {
            dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@TherapistId", therapistId));
        }

        if (dt.Rows.Count == 0)
        {
            pnlNoRequests.Visible = true;
            rptRequests.Visible = false;
        }
        else
        {
            pnlNoRequests.Visible = false;
            rptRequests.Visible = true;
            rptRequests.DataSource = dt;
            rptRequests.DataBind();
        }
    }

    protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadRequests();
    }

    /// <summary>
    /// Shows/hides each action button per row depending on the appointment's
    /// current status - e.g. "Accept"/"Reject" only make sense while Pending,
    /// "Mark Completed" only while Confirmed, and the meeting-link box only
    /// for Online appointments that are Pending or Confirmed.
    /// </summary>
    protected void rptRequests_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

        DataRowView row = (DataRowView)e.Item.DataItem;
        string status = row["Status"].ToString();
        string consultationType = row["ConsultationType"].ToString();

        LinkButton btnAccept = (LinkButton)e.Item.FindControl("btnAccept");
        LinkButton btnReject = (LinkButton)e.Item.FindControl("btnReject");
        LinkButton btnSaveLink = (LinkButton)e.Item.FindControl("btnSaveLink");
        LinkButton btnComplete = (LinkButton)e.Item.FindControl("btnComplete");
        LinkButton btnCancel = (LinkButton)e.Item.FindControl("btnCancel");
        Panel pnlMeetingLink = (Panel)e.Item.FindControl("pnlMeetingLink");

        bool isOnline = consultationType == "Online";

        btnAccept.Visible = status == "Pending";
        btnReject.Visible = status == "Pending";
        btnComplete.Visible = status == "Confirmed";
        btnCancel.Visible = status == "Confirmed";

        // Meeting link box + its own save button only matter for online sessions
        // that are still Pending (about to be accepted) or already Confirmed.
        pnlMeetingLink.Visible = isOnline && (status == "Pending" || status == "Confirmed");
        btnSaveLink.Visible = isOnline && status == "Confirmed";
    }

    protected void rptRequests_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int appointmentId = Convert.ToInt32(e.CommandArgument);
        int therapistId = GetTherapistId();

        // The meeting link textbox lives inside the same row, so we read it
        // here regardless of which button was clicked (Accept also saves it
        // if the therapist filled it in before accepting).
        TextBox txtMeetingLink = (TextBox)e.Item.FindControl("txtMeetingLink");
        string meetingLink = txtMeetingLink != null ? txtMeetingLink.Text.Trim() : "";

        switch (e.CommandName)
        {
            case "Accept":
                int rowsAffected = UpdateAppointment(appointmentId, therapistId,
                    "UPDATE Appointments SET Status = 'Confirmed', MeetingLink = @MeetingLink " +
                    "WHERE AppointmentId = @AppointmentId AND TherapistId = @TherapistId AND Status = 'Pending'",
                    meetingLink);

                if (rowsAffected > 0)
                {
                    SendConfirmationEmail(appointmentId);
                    lblMessage.CssClass = "text-success";
                    lblMessage.Text = "Appointment confirmed.";
                }
                else
                {
                    lblMessage.CssClass = "text-danger";
                    lblMessage.Text = "This request could not be accepted (it may have already been updated).";
                }
                break;

            case "Reject":
                int rejectRows = UpdateAppointment(appointmentId, therapistId,
                    "UPDATE Appointments SET Status = 'Rejected' " +
                    "WHERE AppointmentId = @AppointmentId AND TherapistId = @TherapistId AND Status = 'Pending'",
                    null);

                if (rejectRows > 0)
                {
                    SendDeclineEmails(appointmentId, wasAlreadyConfirmed: false);
                    lblMessage.CssClass = "text-success";
                    lblMessage.Text = "Appointment rejected. The patient and Admin have been emailed about the refund.";
                }
                else
                {
                    lblMessage.CssClass = "text-danger";
                    lblMessage.Text = "This request could not be rejected (it may have already been updated).";
                }
                break;

            case "Cancel":
                int cancelRows = UpdateAppointment(appointmentId, therapistId,
                    "UPDATE Appointments SET Status = 'Cancelled' " +
                    "WHERE AppointmentId = @AppointmentId AND TherapistId = @TherapistId AND Status = 'Confirmed'",
                    null);

                if (cancelRows > 0)
                {
                    SendDeclineEmails(appointmentId, wasAlreadyConfirmed: true);
                    lblMessage.CssClass = "text-success";
                    lblMessage.Text = "Session cancelled. The patient and Admin have been emailed about the refund.";
                }
                else
                {
                    lblMessage.CssClass = "text-danger";
                    lblMessage.Text = "This session could not be cancelled (it may have already been updated).";
                }
                break;

            case "SaveLink":
                UpdateAppointment(appointmentId, therapistId,
                    "UPDATE Appointments SET MeetingLink = @MeetingLink " +
                    "WHERE AppointmentId = @AppointmentId AND TherapistId = @TherapistId AND Status = 'Confirmed'",
                    meetingLink);
                lblMessage.Text = "Meeting link saved.";
                break;

            case "Complete":
                UpdateAppointment(appointmentId, therapistId,
                    "UPDATE Appointments SET Status = 'Completed' " +
                    "WHERE AppointmentId = @AppointmentId AND TherapistId = @TherapistId AND Status = 'Confirmed'",
                    null);
                lblMessage.Text = "Session marked as completed.";
                break;
        }

        LoadRequests();
    }

    /// <summary>
    /// This is the FINAL confirmation email - sent only once the therapist actually
    /// accepts the request (not when payment is verified - that's a separate, earlier
    /// email from Admin/VerifyPayments.aspx.cs, worded to make clear it's not final yet).
    /// </summary>
    private void SendConfirmationEmail(int appointmentId)
    {
        string query = @"SELECT u.Email, u.FullName AS PatientName, tu.FullName AS TherapistName,
                                 a.AppointmentDate, a.AppointmentTime, a.ConsultationType
                          FROM Appointments a
                          INNER JOIN Patients p ON p.PatientId = a.PatientId
                          INNER JOIN Users u ON u.UserId = p.UserId
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users tu ON tu.UserId = t.UserId
                          WHERE a.AppointmentId = @AppointmentId";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@AppointmentId", appointmentId));
        if (dt.Rows.Count == 0) return;

        DataRow row = dt.Rows[0];
        string body = EmailHelper.BuildAppointmentConfirmationBody(
            row["PatientName"].ToString(),
            row["TherapistName"].ToString(),
            Convert.ToDateTime(row["AppointmentDate"]),
            DateTime.Today.Add((TimeSpan)row["AppointmentTime"]).ToString("h:mm tt"),
            row["ConsultationType"].ToString());

        EmailHelper.SendEmail(row["Email"].ToString(), "MindCare - Appointment Confirmed", body);
    }

    /// <summary>
    /// Fired when a therapist Rejects a Pending request or Cancels an already-
    /// Confirmed one. Since a therapist can only ever see a payment-verified
    /// appointment in the first place, real money was collected - so this sends
    /// two emails: one to the patient explaining a refund is coming, and one to
    /// EVERY Admin account with the exact UPI transaction reference they need
    /// to actually locate and process that refund manually.
    /// </summary>
    private void SendDeclineEmails(int appointmentId, bool wasAlreadyConfirmed)
    {
        string query = @"SELECT pu.Email AS PatientEmail, pu.FullName AS PatientName, tu.FullName AS TherapistName,
                                 a.AppointmentDate, a.AppointmentTime, a.Amount, pay.UpiTransactionRef
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
        string patientEmail = row["PatientEmail"].ToString();
        string patientName = row["PatientName"].ToString();
        string therapistName = row["TherapistName"].ToString();
        DateTime appointmentDate = Convert.ToDateTime(row["AppointmentDate"]);
        string displayTime = DateTime.Today.Add((TimeSpan)row["AppointmentTime"]).ToString("h:mm tt");
        decimal amount = Convert.ToDecimal(row["Amount"]);
        string upiRef = row["UpiTransactionRef"] == DBNull.Value ? "(not recorded)" : row["UpiTransactionRef"].ToString();

        // Email the patient.
        string patientBody = EmailHelper.BuildTherapistDeclinedBody(
            patientName, therapistName, appointmentDate, displayTime, wasAlreadyConfirmed);

        EmailHelper.SendEmail(patientEmail,
            wasAlreadyConfirmed ? "MindCare - Your Session Was Cancelled" : "MindCare - Your Session Request Was Declined",
            patientBody);

        // Email every Admin account - there could be more than one, so send to all.
        string adminBody = EmailHelper.BuildAdminRefundNoticeBody(
            patientName, patientEmail, therapistName, appointmentDate, displayTime, amount, upiRef, wasAlreadyConfirmed);

        string adminEmailQuery = "SELECT Email FROM Users WHERE RoleId = 1 AND IsActive = 1";
        DataTable adminEmails = DBHelper.ExecuteSelect(adminEmailQuery);

        foreach (DataRow adminRow in adminEmails.Rows)
        {
            EmailHelper.SendEmail(adminRow["Email"].ToString(), "MindCare - Refund Needed (Action Required)", adminBody);
        }
    }

    /// <summary>
    /// One shared helper for all four actions above. The WHERE clause always
    /// includes "AND TherapistId = @TherapistId" so a therapist can never
    /// modify another therapist's appointment by tampering with the posted
    /// CommandArgument, and the current-status check (e.g. "Status = 'Pending'")
    /// stops the same action firing twice from a stale page.
    /// </summary>
    private int UpdateAppointment(int appointmentId, int therapistId, string query, string meetingLink)
    {
        if (query.Contains("@MeetingLink"))
        {
            return DBHelper.ExecuteNonQuery(query,
                new MySqlParameter("@MeetingLink", string.IsNullOrEmpty(meetingLink) ? (object)DBNull.Value : meetingLink),
                new MySqlParameter("@AppointmentId", appointmentId),
                new MySqlParameter("@TherapistId", therapistId));
        }
        else
        {
            return DBHelper.ExecuteNonQuery(query,
                new MySqlParameter("@AppointmentId", appointmentId),
                new MySqlParameter("@TherapistId", therapistId));
        }
    }
}
