using System;
using System.Data;

public partial class _Default : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            LoadFeaturedTherapists();
        }
    }

    private void LoadFeaturedTherapists()
    {
        string query = @"SELECT u.FullName, t.Specialization, t.Fees, t.ProfileImage
                          FROM Therapists t
                          INNER JOIN Users u ON u.UserId = t.UserId
                          WHERE t.ApprovalStatus = 'Approved'
                          LIMIT 3";

        DataTable dt = DBHelper.ExecuteSelect(query);
        rptFeaturedTherapists.DataSource = dt;
        rptFeaturedTherapists.DataBind();
    }

    /// <summary>
    /// Uses the therapist's real uploaded photo if they have one (see
    /// Therapist/Profile.aspx.cs), otherwise falls back to the same
    /// generated placeholder used everywhere else on the site.
    /// </summary>
    protected void rptFeaturedTherapists_ItemDataBound(object sender, System.Web.UI.WebControls.RepeaterItemEventArgs e)
    {
        if (e.Item.ItemType != System.Web.UI.WebControls.ListItemType.Item &&
            e.Item.ItemType != System.Web.UI.WebControls.ListItemType.AlternatingItem)
            return;

        DataRowView rowView = (DataRowView)e.Item.DataItem;
        string fullName = rowView["FullName"].ToString();

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
