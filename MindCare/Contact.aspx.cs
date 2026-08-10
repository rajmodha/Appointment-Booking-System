using System;
using MySql.Data.MySqlClient;

public partial class Contact : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnSend_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        string query = @"INSERT INTO ContactMessages (FullName, Email, Subject, Message)
                          VALUES (@FullName, @Email, @Subject, @Message)";

        DBHelper.ExecuteNonQuery(query,
            new MySqlParameter("@FullName", txtFullName.Text.Trim()),
            new MySqlParameter("@Email", txtEmail.Text.Trim()),
            new MySqlParameter("@Subject", string.IsNullOrWhiteSpace(txtSubject.Text) ? "(no subject)" : txtSubject.Text.Trim()),
            new MySqlParameter("@Message", txtMessage.Text.Trim()));

        pnlForm.Visible = false;
        pnlThankYou.Visible = true;
    }
}
