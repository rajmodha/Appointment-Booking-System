using System;
using System.Data;
using System.Text;
using System.Collections.Generic;
using MySql.Data.MySqlClient;

public partial class FindTherapist : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadCategories();
            SearchTherapists(); // show all approved therapists by default
        }
    }

    /// <summary>
    /// Fills the "Specialization" dropdown from the TherapyCategories table.
    /// </summary>
    private void LoadCategories()
    {
        string query = "SELECT CategoryId, CategoryName FROM TherapyCategories ORDER BY CategoryName";
        DataTable dt = DBHelper.ExecuteSelect(query);

        foreach (DataRow row in dt.Rows)
        {
            ddlCategory.Items.Add(new System.Web.UI.WebControls.ListItem(
                row["CategoryName"].ToString(), row["CategoryId"].ToString()));
        }
    }

    protected void btnSearch_Click(object sender, EventArgs e)
    {
        SearchTherapists();
    }

    /// <summary>
    /// Builds a filtered SELECT using only the filters the user actually filled in.
    /// Everything is passed as a MySqlParameter, never string-concatenated, so this
    /// is safe from SQL injection even though the WHERE clause is built dynamically.
    /// </summary>
    private void SearchTherapists()
    {
        StringBuilder query = new StringBuilder(@"
            SELECT t.TherapistId, u.FullName, t.Specialization, t.Qualification,
                   t.Location, t.Language, t.ConsultationType, t.Fees, t.ProfileImage
            FROM Therapists t
            INNER JOIN Users u ON u.UserId = t.UserId
            WHERE t.ApprovalStatus = 'Approved' ");

        List<MySqlParameter> parameters = new List<MySqlParameter>();

        // Specialization / category filter
        int categoryId = Convert.ToInt32(ddlCategory.SelectedValue);
        if (categoryId > 0)
        {
            query.Append(" AND t.CategoryId = @CategoryId ");
            parameters.Add(new MySqlParameter("@CategoryId", categoryId));
        }

        // Location filter (partial match)
        if (!string.IsNullOrWhiteSpace(txtLocation.Text))
        {
            query.Append(" AND t.Location LIKE @Location ");
            parameters.Add(new MySqlParameter("@Location", "%" + txtLocation.Text.Trim() + "%"));
        }

        // Language filter (partial match)
        if (!string.IsNullOrWhiteSpace(txtLanguage.Text))
        {
            query.Append(" AND t.Language LIKE @Language ");
            parameters.Add(new MySqlParameter("@Language", "%" + txtLanguage.Text.Trim() + "%"));
        }

        // Consultation type: therapist offers 'Both' OR exactly what was requested
        if (!string.IsNullOrWhiteSpace(ddlConsultationType.SelectedValue))
        {
            query.Append(" AND (t.ConsultationType = @ConsultationType OR t.ConsultationType = 'Both') ");
            parameters.Add(new MySqlParameter("@ConsultationType", ddlConsultationType.SelectedValue));
        }

        // Max fee filter
        if (!string.IsNullOrWhiteSpace(txtMaxFee.Text))
        {
            decimal maxFee;
            if (decimal.TryParse(txtMaxFee.Text, out maxFee))
            {
                query.Append(" AND t.Fees <= @MaxFee ");
                parameters.Add(new MySqlParameter("@MaxFee", maxFee));
            }
        }

        query.Append(" ORDER BY t.Fees ASC ");

        DataTable dt = DBHelper.ExecuteSelect(query.ToString(), parameters.ToArray());

        rptTherapists.DataSource = dt;
        rptTherapists.DataBind();

        if (dt.Rows.Count == 0)
        {
            pnlNoResults.Visible = true;
            lblResultCount.Text = "";
        }
        else
        {
            pnlNoResults.Visible = false;
            lblResultCount.Text = dt.Rows.Count + " therapist(s) found";
        }
    }

    /// <summary>
    /// Sets the "View Profile" link's URL and the photo per row (both need
    /// per-row data, so easier in code-behind than inline markup expressions).
    /// </summary>
    protected void rptTherapists_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
            e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
            return;

        DataRowView rowView = (DataRowView)e.Item.DataItem;
        int therapistId = Convert.ToInt32(rowView["TherapistId"]);
        string fullName = rowView["FullName"].ToString();

        System.Web.UI.WebControls.HyperLink hlViewProfile =
            (System.Web.UI.WebControls.HyperLink)e.Item.FindControl("hlViewProfile");
        hlViewProfile.NavigateUrl = ResolveUrl("~/TherapistDetails.aspx?id=" + therapistId);

        System.Web.UI.WebControls.Image imgTherapist =
            (System.Web.UI.WebControls.Image)e.Item.FindControl("imgTherapist");

        object profileImageValue = rowView["ProfileImage"];
        string profileImage = (profileImageValue == null || profileImageValue == DBNull.Value)
            ? null : profileImageValue.ToString();

        imgTherapist.ImageUrl = string.IsNullOrEmpty(profileImage)
            ? "https://api.dicebear.com/7.x/initials/svg?seed=" + Server.UrlEncode(fullName)
            : ResolveUrl(profileImage);
    }
}
