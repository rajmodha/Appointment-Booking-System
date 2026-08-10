using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class Admin_MyProfile : AdminBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminSidebar1.ActivePage = "Profile";

        if (!IsPostBack)
        {
            LoadProfile();
        }
    }

    private void LoadProfile()
    {
        string query = "SELECT Email, FullName, Phone FROM Users WHERE UserId = @UserId";
        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@UserId", CurrentUserId));
        if (dt.Rows.Count == 0) return;

        txtEmail.Text = dt.Rows[0]["Email"].ToString();
        txtFullName.Text = dt.Rows[0]["FullName"].ToString();
        txtPhone.Text = dt.Rows[0]["Phone"].ToString();
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        DBHelper.ExecuteNonQuery(
            "UPDATE Users SET FullName = @FullName, Phone = @Phone WHERE UserId = @UserId",
            new MySqlParameter("@FullName", txtFullName.Text.Trim()),
            new MySqlParameter("@Phone", txtPhone.Text.Trim()),
            new MySqlParameter("@UserId", CurrentUserId));

        Session["FullName"] = txtFullName.Text.Trim();
        lblMessage.Text = "Profile saved successfully.";
    }
}
