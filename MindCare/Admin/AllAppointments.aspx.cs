using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class AllAppointments : AdminBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminSidebar1.ActivePage = "Appointments";

        if (!IsPostBack)
        {
            LoadAppointments();
        }
    }

    private void LoadAppointments()
    {
        string statusFilter = ddlStatusFilter.SelectedValue;

        string query = @"SELECT pu.FullName AS PatientName, tu.FullName AS TherapistName,
                                 a.AppointmentDate, TIME_FORMAT(a.AppointmentTime, '%h:%i %p') AS DisplayTime,
                                 a.ConsultationType, a.Amount, a.Status
                          FROM Appointments a
                          INNER JOIN Patients p ON p.PatientId = a.PatientId
                          INNER JOIN Users pu ON pu.UserId = p.UserId
                          INNER JOIN Therapists t ON t.TherapistId = a.TherapistId
                          INNER JOIN Users tu ON tu.UserId = t.UserId
                          WHERE 1=1 ";

        if (!string.IsNullOrEmpty(statusFilter))
            query += " AND a.Status = @Status ";

        query += " ORDER BY a.AppointmentDate DESC, a.AppointmentTime DESC ";

        DataTable dt = !string.IsNullOrEmpty(statusFilter)
            ? DBHelper.ExecuteSelect(query, new MySqlParameter("@Status", statusFilter))
            : DBHelper.ExecuteSelect(query);

        if (dt.Rows.Count == 0)
        {
            pnlNoResults.Visible = true;
            rptAppointments.Visible = false;
        }
        else
        {
            pnlNoResults.Visible = false;
            rptAppointments.Visible = true;
            rptAppointments.DataSource = dt;
            rptAppointments.DataBind();
        }
    }

    protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadAppointments();
    }
}
