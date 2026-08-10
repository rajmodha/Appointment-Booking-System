using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class ManagePatients : AdminBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminSidebar1.ActivePage = "Patients";

        if (!IsPostBack)
        {
            LoadPatients();
        }
    }

    private void LoadPatients()
    {
        string query = @"SELECT p.PatientId, u.FullName, u.Email, u.Phone, u.IsActive,
                                 (SELECT COUNT(*) FROM Appointments a WHERE a.PatientId = p.PatientId) AS TotalAppointments
                          FROM Patients p
                          INNER JOIN Users u ON u.UserId = p.UserId
                          ORDER BY p.PatientId DESC";

        DataTable dt = DBHelper.ExecuteSelect(query);

        if (dt.Rows.Count == 0)
        {
            pnlNoResults.Visible = true;
            rptPatients.Visible = false;
        }
        else
        {
            pnlNoResults.Visible = false;
            rptPatients.Visible = true;
            rptPatients.DataSource = dt;
            rptPatients.DataBind();
        }
    }

    protected void rptPatients_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        if (e.CommandName != "ToggleActive") return;

        int patientId = Convert.ToInt32(e.CommandArgument);

        DBHelper.ExecuteNonQuery(@"
            UPDATE Users u
            INNER JOIN Patients p ON p.UserId = u.UserId
            SET u.IsActive = NOT u.IsActive
            WHERE p.PatientId = @Id",
            new MySqlParameter("@Id", patientId));

        lblMessage.Text = "Account status updated.";
        LoadPatients();
    }
}
