using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class Patient_MyProfile : PatientBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        PatientSidebar1.ActivePage = "Profile";

        if (!IsPostBack)
        {
            LoadProfile();
        }
    }

    private void LoadProfile()
    {
        string query = @"SELECT u.Email, u.FullName, u.Phone, p.DateOfBirth, p.Gender, p.Address
                          FROM Users u
                          INNER JOIN Patients p ON p.UserId = u.UserId
                          WHERE u.UserId = @UserId";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@UserId", CurrentUserId));
        if (dt.Rows.Count == 0) return;

        DataRow row = dt.Rows[0];
        txtEmail.Text = row["Email"].ToString();
        txtFullName.Text = row["FullName"].ToString();
        txtPhone.Text = row["Phone"].ToString();

        if (row["DateOfBirth"] != DBNull.Value)
            txtDateOfBirth.Text = Convert.ToDateTime(row["DateOfBirth"]).ToString("yyyy-MM-dd");

        if (row["Gender"] != DBNull.Value && ddlGender.Items.FindByValue(row["Gender"].ToString()) != null)
            ddlGender.SelectedValue = row["Gender"].ToString();

        txtAddress.Text = row["Address"].ToString();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        DBHelper.ExecuteNonQuery(
            "UPDATE Users SET FullName = @FullName, Phone = @Phone WHERE UserId = @UserId",
            new MySqlParameter("@FullName", txtFullName.Text.Trim()),
            new MySqlParameter("@Phone", txtPhone.Text.Trim()),
            new MySqlParameter("@UserId", CurrentUserId));

        object dob = string.IsNullOrWhiteSpace(txtDateOfBirth.Text) ? (object)DBNull.Value : txtDateOfBirth.Text;
        object gender = string.IsNullOrWhiteSpace(ddlGender.SelectedValue) ? (object)DBNull.Value : ddlGender.SelectedValue;

        DBHelper.ExecuteNonQuery(
            "UPDATE Patients SET DateOfBirth = @DateOfBirth, Gender = @Gender, Address = @Address WHERE UserId = @UserId",
            new MySqlParameter("@DateOfBirth", dob),
            new MySqlParameter("@Gender", gender),
            new MySqlParameter("@Address", txtAddress.Text.Trim()),
            new MySqlParameter("@UserId", CurrentUserId));

        // Update the session's display name immediately, so the navbar greeting
        // and sidebar "Hi, ___" reflect a name change without needing to re-login.
        Session["FullName"] = txtFullName.Text.Trim();

        lblMessage.Text = "Profile saved successfully.";
    }
}
