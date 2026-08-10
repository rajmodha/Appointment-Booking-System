using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class Admin_ContactMessages : AdminBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminSidebar1.ActivePage = "Messages";

        if (!IsPostBack)
        {
            LoadMessages();
        }
    }

    private void LoadMessages()
    {
        // Unread messages first, then newest first within each group - so new
        // submissions surface at the top without older read messages vanishing.
        string query = @"SELECT MessageId, FullName, Email, Subject, Message, SubmittedOn, IsRead
                          FROM ContactMessages
                          ORDER BY IsRead ASC, SubmittedOn DESC";

        DataTable dt = DBHelper.ExecuteSelect(query);

        if (dt.Rows.Count == 0)
        {
            pnlNoMessages.Visible = true;
            rptMessages.Visible = false;
        }
        else
        {
            pnlNoMessages.Visible = false;
            rptMessages.Visible = true;
            rptMessages.DataSource = dt;
            rptMessages.DataBind();
        }
    }

    protected void rptMessages_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        int messageId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "MarkRead")
        {
            DBHelper.ExecuteNonQuery(
                "UPDATE ContactMessages SET IsRead = 1 WHERE MessageId = @MessageId",
                new MySqlParameter("@MessageId", messageId));
            lblMessage.Text = "Marked as read.";
        }
        else if (e.CommandName == "Delete")
        {
            DBHelper.ExecuteNonQuery(
                "DELETE FROM ContactMessages WHERE MessageId = @MessageId",
                new MySqlParameter("@MessageId", messageId));
            lblMessage.Text = "Message deleted.";
        }

        LoadMessages();
    }
}
