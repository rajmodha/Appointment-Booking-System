using System;
using System.Data;

public partial class AdminDashboard : AdminBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminSidebar1.ActivePage = "Dashboard";

        if (!IsPostBack)
        {
            LoadStats();
            LoadPendingApprovals();
        }
    }

    private void LoadStats()
    {
        litTotalPatients.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Patients").ToString();

        litTotalTherapists.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM Therapists WHERE ApprovalStatus = 'Approved'").ToString();

        litPendingApprovals.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM Therapists WHERE ApprovalStatus = 'Pending'").ToString();

        litPendingPayments.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM Payments WHERE PaymentStatus = 'Pending'").ToString();

        // Same criteria as Admin/Refunds.aspx.cs's "Awaiting Refund" list -
        // payments that succeeded but whose appointment was later Rejected
        // or Cancelled by the therapist.
        litRefundsPending.Text = DBHelper.ExecuteScalar(@"
            SELECT COUNT(*) FROM Payments pay
            INNER JOIN Appointments a ON a.AppointmentId = pay.AppointmentId
            WHERE pay.PaymentStatus = 'Success' AND a.Status IN ('Rejected','Cancelled')").ToString();

        litUnreadMessages.Text = DBHelper.ExecuteScalar(
            "SELECT COUNT(*) FROM ContactMessages WHERE IsRead = 0").ToString();

        litTotalAppointments.Text = DBHelper.ExecuteScalar("SELECT COUNT(*) FROM Appointments").ToString();

        object revenue = DBHelper.ExecuteScalar(
            "SELECT COALESCE(SUM(Amount),0) FROM Payments WHERE PaymentStatus = 'Success'");
        litTotalRevenue.Text = Convert.ToDecimal(revenue).ToString("N0");
    }

    private void LoadPendingApprovals()
    {
        string query = @"SELECT u.FullName, u.Email
                          FROM Therapists t
                          INNER JOIN Users u ON u.UserId = t.UserId
                          WHERE t.ApprovalStatus = 'Pending'
                          ORDER BY t.TherapistId DESC
                          LIMIT 5";

        DataTable dt = DBHelper.ExecuteSelect(query);

        if (dt.Rows.Count == 0)
        {
            lblNoPending.Visible = true;
        }
        else
        {
            rptPending.DataSource = dt;
            rptPending.DataBind();
        }
    }
}
