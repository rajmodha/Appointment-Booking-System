using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class VerifyPayments : AdminBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminSidebar1.ActivePage = "Payments";

        if (!IsPostBack)
        {
            LoadPendingPayments();
        }
    }

    private void LoadPendingPayments()
    {
        string query = @"SELECT pay.PaymentId, pay.Amount, pay.UpiTransactionRef,
                                 a.AppointmentId, a.AppointmentDate, a.ConsultationType,
                                 TIME_FORMAT(a.AppointmentTime, '%h:%i %p') AS DisplayTime,
                                 pu.FullName AS PatientName, tu.FullName AS TherapistName
                          FROM Payments pay
                          INNER JOIN Appointments a ON a.AppointmentId = pay.AppointmentId
                          INNER JOIN Patients p ON p.PatientId = a.PatientId
                          INNER JOIN Users pu ON pu.UserId = p.UserId
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users tu ON tu.UserId = t.UserId
                          WHERE pay.PaymentStatus = 'Pending'
                          ORDER BY pay.PaymentDate ASC";

        DataTable dt = DBHelper.ExecuteSelect(query);

        if (dt.Rows.Count == 0)
        {
            pnlNoPayments.Visible = true;
            rptPayments.Visible = false;
        }
        else
        {
            pnlNoPayments.Visible = false;
            rptPayments.Visible = true;
            rptPayments.DataSource = dt;
            rptPayments.DataBind();
        }
    }

    protected void rptPayments_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        int paymentId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "Approve")
        {
            ApprovePayment(paymentId);
            lblMessage.Text = "Payment verified. The therapist can now see this request, and both the patient and therapist have been emailed.";
        }
        else if (e.CommandName == "Reject")
        {
            RejectPayment(paymentId);
            lblMessage.Text = "Payment rejected, the appointment has been cancelled, and the patient has been emailed.";
        }

        LoadPendingPayments();
    }

    /// <summary>
    /// Marks the payment Success, which is what Therapist/Requests.aspx.cs's query
    /// filters on to decide whether a therapist can see this appointment yet - so
    /// this one UPDATE is what actually "unlocks" the request for the therapist.
    /// Only after that do we send the patient their confirmation email.
    /// </summary>
    private void ApprovePayment(int paymentId)
    {
        DBHelper.ExecuteNonQuery(
            "UPDATE Payments SET PaymentStatus = 'Success' WHERE PaymentId = @PaymentId",
            new MySqlParameter("@PaymentId", paymentId));

        SendConfirmationEmail(paymentId);
    }

    /// <summary>
    /// A rejected payment means there's no valid booking behind it, so the
    /// appointment itself is cancelled too - it should not sit around as
    /// "Pending" forever with no real payment backing it.
    /// </summary>
    private void RejectPayment(int paymentId)
    {
        DBHelper.ExecuteNonQuery(
            "UPDATE Payments SET PaymentStatus = 'Failed' WHERE PaymentId = @PaymentId",
            new MySqlParameter("@PaymentId", paymentId));

        DBHelper.ExecuteNonQuery(@"
            UPDATE Appointments a
            INNER JOIN Payments pay ON pay.AppointmentId = a.AppointmentId
            SET a.Status = 'Cancelled'
            WHERE pay.PaymentId = @PaymentId",
            new MySqlParameter("@PaymentId", paymentId));

        SendRejectionEmail(paymentId);
    }

    /// <summary>
    /// Lets the patient know why their appointment vanished, instead of them
    /// finding out only by checking "My Appointments" and seeing Cancelled
    /// with no explanation.
    /// </summary>
    private void SendRejectionEmail(int paymentId)
    {
        string query = @"SELECT pu.Email AS PatientEmail, pu.FullName AS PatientName, tu.FullName AS TherapistName,
                                 a.AppointmentDate, a.AppointmentTime
                          FROM Payments pay
                          INNER JOIN Appointments a ON a.AppointmentId = pay.AppointmentId
                          INNER JOIN Patients p ON p.PatientId = a.PatientId
                          INNER JOIN Users pu ON pu.UserId = p.UserId
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users tu ON tu.UserId = t.UserId
                          WHERE pay.PaymentId = @PaymentId";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@PaymentId", paymentId));
        if (dt.Rows.Count == 0) return;

        DataRow row = dt.Rows[0];
        string body = EmailHelper.BuildPaymentRejectedBody(
            row["PatientName"].ToString(),
            row["TherapistName"].ToString(),
            Convert.ToDateTime(row["AppointmentDate"]),
            DateTime.Today.Add((TimeSpan)row["AppointmentTime"]).ToString("h:mm tt"));

        EmailHelper.SendEmail(row["PatientEmail"].ToString(), "MindCare - Payment Could Not Be Verified", body);
    }

    private void SendConfirmationEmail(int paymentId)
    {
        string query = @"SELECT pu.Email AS PatientEmail, pu.FullName AS PatientName,
                                 tu.Email AS TherapistEmail, tu.FullName AS TherapistName,
                                 a.AppointmentDate, a.AppointmentTime, a.ConsultationType
                          FROM Payments pay
                          INNER JOIN Appointments a ON a.AppointmentId = pay.AppointmentId
                          INNER JOIN Patients p ON p.PatientId = a.PatientId
                          INNER JOIN Users pu ON pu.UserId = p.UserId
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users tu ON tu.UserId = t.UserId
                          WHERE pay.PaymentId = @PaymentId";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@PaymentId", paymentId));
        if (dt.Rows.Count == 0) return;

        DataRow row = dt.Rows[0];
        DateTime appointmentDate = Convert.ToDateTime(row["AppointmentDate"]);
        string displayTime = DateTime.Today.Add((TimeSpan)row["AppointmentTime"]).ToString("h:mm tt");
        string consultationType = row["ConsultationType"].ToString();

        // Email #1 - to the patient, confirming their payment went through.
        string patientBody = EmailHelper.BuildPaymentVerifiedBody(
            row["PatientName"].ToString(),
            row["TherapistName"].ToString(),
            appointmentDate, displayTime, consultationType);

        EmailHelper.SendEmail(row["PatientEmail"].ToString(), "MindCare - Payment Verified", patientBody);

        // Email #2 - to the therapist, since this is the exact moment their
        // Requests.aspx page actually starts showing this appointment (it's
        // filtered on PaymentStatus = 'Success' - see Requests.aspx.cs). Without
        // this email they'd have no way to know a new request appeared short
        // of manually checking the page.
        string therapistBody = EmailHelper.BuildTherapistNewRequestBody(
            row["TherapistName"].ToString(),
            row["PatientName"].ToString(),
            appointmentDate, displayTime, consultationType);

        EmailHelper.SendEmail(row["TherapistEmail"].ToString(), "MindCare - New Appointment Request", therapistBody);
    }
}
