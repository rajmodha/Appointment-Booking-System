using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class Admin_Refunds : AdminBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminSidebar1.ActivePage = "Refunds";

        if (!IsPostBack)
        {
            LoadRefunds();
        }
    }

    private void LoadRefunds()
    {
        LoadPendingRefunds();
        LoadCompletedRefunds();
    }

    /// <summary>
    /// A payment needs refunding when it succeeded (money was actually collected)
    /// but the appointment it paid for never happened - i.e. the therapist
    /// Rejected or Cancelled it after payment was verified. This is exactly the
    /// set of payments Therapist/Requests.aspx.cs's SendDeclineEmails() already
    /// notified Admin about individually - this page is where that gets tracked
    /// and closed out.
    /// </summary>
    private void LoadPendingRefunds()
    {
        string query = @"SELECT pay.PaymentId, pay.Amount, pay.UpiTransactionRef,
                                 a.AppointmentDate, a.Status AS AppointmentStatus,
                                 TIME_FORMAT(a.AppointmentTime, '%h:%i %p') AS DisplayTime,
                                 pu.FullName AS PatientName, tu.FullName AS TherapistName
                          FROM Payments pay
                          INNER JOIN Appointments a ON a.AppointmentId = pay.AppointmentId
                          INNER JOIN Patients p ON p.PatientId = a.PatientId
                          INNER JOIN Users pu ON pu.UserId = p.UserId
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users tu ON tu.UserId = t.UserId
                          WHERE pay.PaymentStatus = 'Success'
                            AND a.Status IN ('Rejected','Cancelled')
                          ORDER BY a.AppointmentDate DESC";

        DataTable dt = DBHelper.ExecuteSelect(query);

        if (dt.Rows.Count == 0)
        {
            pnlNoPending.Visible = true;
            rptPending.Visible = false;
        }
        else
        {
            pnlNoPending.Visible = false;
            rptPending.Visible = true;
            rptPending.DataSource = dt;
            rptPending.DataBind();
        }
    }

    private void LoadCompletedRefunds()
    {
        string query = @"SELECT pay.PaymentId, pay.Amount, pay.UpiTransactionRef, pay.RefundTransactionRef,
                                 a.AppointmentDate,
                                 TIME_FORMAT(a.AppointmentTime, '%h:%i %p') AS DisplayTime,
                                 pu.FullName AS PatientName, tu.FullName AS TherapistName
                          FROM Payments pay
                          INNER JOIN Appointments a ON a.AppointmentId = pay.AppointmentId
                          INNER JOIN Patients p ON p.PatientId = a.PatientId
                          INNER JOIN Users pu ON pu.UserId = p.UserId
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users tu ON tu.UserId = t.UserId
                          WHERE pay.PaymentStatus = 'Refunded'
                          ORDER BY a.AppointmentDate DESC
                          LIMIT 20";

        DataTable dt = DBHelper.ExecuteSelect(query);

        if (dt.Rows.Count == 0)
        {
            pnlNoRefunded.Visible = true;
            rptRefunded.Visible = false;
        }
        else
        {
            pnlNoRefunded.Visible = false;
            rptRefunded.Visible = true;
            rptRefunded.DataSource = dt;
            rptRefunded.DataBind();
        }
    }

    /// <summary>
    /// Flips the payment to 'Refunded' and records the refund's OWN transaction
    /// reference (separate from the original payment's reference) - this is
    /// what makes it disappear from Admin/AdminDashboard.aspx.cs's revenue
    /// total, since that query only sums PaymentStatus = 'Success'. Nothing
    /// here actually sends money - this only records that Admin has already
    /// done so manually via their UPI app, and emails the patient proof of it.
    /// </summary>
    protected void rptPending_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "MarkRefunded") return;

        int paymentId = Convert.ToInt32(e.CommandArgument);

        System.Web.UI.WebControls.TextBox txtRefundRef =
            (System.Web.UI.WebControls.TextBox)e.Item.FindControl("txtRefundRef");
        string refundRef = txtRefundRef != null ? txtRefundRef.Text.Trim() : "";

        if (string.IsNullOrEmpty(refundRef))
        {
            lblMessage.CssClass = "text-danger";
            lblMessage.Text = "Please enter the refund's transaction ID before marking it as refunded.";
            LoadRefunds();
            return;
        }

        int rowsAffected = DBHelper.ExecuteNonQuery(
            "UPDATE Payments SET PaymentStatus = 'Refunded', RefundTransactionRef = @RefundRef, RefundedOn = NOW() " +
            "WHERE PaymentId = @PaymentId AND PaymentStatus = 'Success'",
            new MySqlParameter("@RefundRef", refundRef),
            new MySqlParameter("@PaymentId", paymentId));

        if (rowsAffected > 0)
        {
            SendRefundProcessedEmail(paymentId, refundRef);
            lblMessage.CssClass = "text-success";
            lblMessage.Text = "Marked as refunded and the patient has been emailed the refund reference. " +
                               "This amount is now excluded from Revenue Collected on the Dashboard.";
        }
        else
        {
            lblMessage.CssClass = "text-danger";
            lblMessage.Text = "This payment could not be updated (it may have already been marked).";
        }

        LoadRefunds();
    }

    private void SendRefundProcessedEmail(int paymentId, string refundRef)
    {
        string query = @"SELECT pu.Email AS PatientEmail, pu.FullName AS PatientName, tu.FullName AS TherapistName,
                                 a.AppointmentDate, a.AppointmentTime, pay.Amount
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
        string body = EmailHelper.BuildRefundProcessedBody(
            row["PatientName"].ToString(),
            row["TherapistName"].ToString(),
            Convert.ToDateTime(row["AppointmentDate"]),
            DateTime.Today.Add((TimeSpan)row["AppointmentTime"]).ToString("h:mm tt"),
            Convert.ToDecimal(row["Amount"]),
            refundRef);

        EmailHelper.SendEmail(row["PatientEmail"].ToString(), "MindCare - Your Refund Has Been Processed", body);
    }
}
