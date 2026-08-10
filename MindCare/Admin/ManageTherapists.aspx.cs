using System;
using System.Data;
using MySql.Data.MySqlClient;
using System.Web.UI.WebControls;

public partial class ManageTherapists : AdminBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminSidebar1.ActivePage = "Therapists";

        if (!IsPostBack)
        {
            LoadTherapists();
        }
    }

    private void LoadTherapists()
    {
        string statusFilter = ddlStatusFilter.SelectedValue;

        string query = @"SELECT t.TherapistId, u.FullName, u.Email, u.Phone, u.IsActive,
                                 t.Specialization, t.Fees, t.ApprovalStatus
                          FROM Therapists t
                          INNER JOIN Users u ON u.UserId = t.UserId
                          WHERE 1=1 ";

        if (!string.IsNullOrEmpty(statusFilter))
            query += " AND t.ApprovalStatus = @Status ";

        query += " ORDER BY t.TherapistId DESC ";

        DataTable dt = !string.IsNullOrEmpty(statusFilter)
            ? DBHelper.ExecuteSelect(query, new MySqlParameter("@Status", statusFilter))
            : DBHelper.ExecuteSelect(query);

        if (dt.Rows.Count == 0)
        {
            pnlNoResults.Visible = true;
            rptTherapists.Visible = false;
        }
        else
        {
            pnlNoResults.Visible = false;
            rptTherapists.Visible = true;
            rptTherapists.DataSource = dt;
            rptTherapists.DataBind();
        }
    }

    protected void ddlStatusFilter_SelectedIndexChanged(object sender, EventArgs e)
    {
        LoadTherapists();
    }

    /// <summary>
    /// Approve/Reject only make sense while a therapist is still Pending, so
    /// we hide those two buttons once a decision has already been made -
    /// "Disable/Enable Account" (via IsActive) remains available regardless,
    /// since that's a separate, reversible switch.
    /// </summary>
    protected void rptTherapists_ItemDataBound(object sender, RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != ListItemType.Item && e.Item.ItemType != ListItemType.AlternatingItem) return;

        DataRowView row = (DataRowView)e.Item.DataItem;
        string approvalStatus = row["ApprovalStatus"].ToString();

        LinkButton btnApprove = (LinkButton)e.Item.FindControl("btnApprove");
        LinkButton btnReject = (LinkButton)e.Item.FindControl("btnReject");

        btnApprove.Visible = approvalStatus == "Pending";
        btnReject.Visible = approvalStatus == "Pending";
    }

    protected void rptTherapists_ItemCommand(object source, RepeaterCommandEventArgs e)
    {
        int therapistId = Convert.ToInt32(e.CommandArgument);

        switch (e.CommandName)
        {
            case "Approve":
                DBHelper.ExecuteNonQuery(
                    "UPDATE Therapists SET ApprovalStatus = 'Approved' WHERE TherapistId = @Id",
                    new MySqlParameter("@Id", therapistId));
                lblMessage.Text = "Therapist approved.";
                break;

            case "Reject":
                DBHelper.ExecuteNonQuery(
                    "UPDATE Therapists SET ApprovalStatus = 'Rejected' WHERE TherapistId = @Id",
                    new MySqlParameter("@Id", therapistId));
                lblMessage.Text = "Therapist application rejected.";
                break;

            case "ToggleActive":
                // Flip Users.IsActive for the account linked to this Therapist row.
                // Disabling blocks login (see Login.aspx.cs's IsActive check) without
                // deleting any of their data.
                DBHelper.ExecuteNonQuery(@"
                    UPDATE Users u
                    INNER JOIN Therapists t ON t.UserId = u.UserId
                    SET u.IsActive = NOT u.IsActive
                    WHERE t.TherapistId = @Id",
                    new MySqlParameter("@Id", therapistId));
                lblMessage.Text = "Account status updated.";
                break;
        }

        LoadTherapists();
    }
}
