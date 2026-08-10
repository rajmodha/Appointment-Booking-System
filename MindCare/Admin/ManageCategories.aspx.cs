using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class Admin_ManageCategories : AdminBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        AdminSidebar1.ActivePage = "Categories";

        if (!IsPostBack)
        {
            LoadCategories();
        }
    }

    private void LoadCategories()
    {
        // LEFT JOIN + COUNT tells us how many therapists currently use each
        // category, which we show in the list AND rely on before allowing a
        // delete (see rptCategories_ItemCommand below).
        string query = @"SELECT c.CategoryId, c.CategoryName, c.Description,
                                 COUNT(t.TherapistId) AS TherapistCount
                          FROM TherapyCategories c
                          LEFT JOIN Therapists t ON t.CategoryId = c.CategoryId
                          GROUP BY c.CategoryId, c.CategoryName, c.Description
                          ORDER BY c.CategoryName";

        DataTable dt = DBHelper.ExecuteSelect(query);

        if (dt.Rows.Count == 0)
        {
            lblNoCategories.Visible = true;
            rptCategories.Visible = false;
        }
        else
        {
            lblNoCategories.Visible = false;
            rptCategories.Visible = true;
            rptCategories.DataSource = dt;
            rptCategories.DataBind();
        }
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        int categoryId = Convert.ToInt32(hdnCategoryId.Value);
        string name = txtCategoryName.Text.Trim();
        string description = txtDescription.Text.Trim();

        if (categoryId == 0)
        {
            // Adding a new category.
            string insertQuery = @"INSERT INTO TherapyCategories (CategoryName, Description)
                                    VALUES (@CategoryName, @Description)";

            DBHelper.ExecuteNonQuery(insertQuery,
                new MySqlParameter("@CategoryName", name),
                new MySqlParameter("@Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description));

            lblMessage.CssClass = "text-success";
            lblMessage.Text = "Category added.";
        }
        else
        {
            // Editing an existing one.
            string updateQuery = @"UPDATE TherapyCategories SET CategoryName = @CategoryName, Description = @Description
                                    WHERE CategoryId = @CategoryId";

            DBHelper.ExecuteNonQuery(updateQuery,
                new MySqlParameter("@CategoryName", name),
                new MySqlParameter("@Description", string.IsNullOrEmpty(description) ? (object)DBNull.Value : description),
                new MySqlParameter("@CategoryId", categoryId));

            lblMessage.CssClass = "text-success";
            lblMessage.Text = "Category updated.";
        }

        ResetForm();
        LoadCategories();
    }

    protected void btnCancelEdit_Click(object sender, EventArgs e)
    {
        ResetForm();
    }

    private void ResetForm()
    {
        hdnCategoryId.Value = "0";
        txtCategoryName.Text = "";
        txtDescription.Text = "";
        litFormTitle.Text = "Add a New Category";
        btnSave.Text = "Add Category";
        btnCancelEdit.Visible = false;
    }

    protected void rptCategories_ItemCommand(object source, System.Web.UI.WebControls.RepeaterCommandEventArgs e)
    {
        int categoryId = Convert.ToInt32(e.CommandArgument);

        if (e.CommandName == "Edit")
        {
            string query = "SELECT CategoryName, Description FROM TherapyCategories WHERE CategoryId = @CategoryId";
            DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@CategoryId", categoryId));

            if (dt.Rows.Count > 0)
            {
                hdnCategoryId.Value = categoryId.ToString();
                txtCategoryName.Text = dt.Rows[0]["CategoryName"].ToString();
                txtDescription.Text = dt.Rows[0]["Description"].ToString();
                litFormTitle.Text = "Edit Category";
                btnSave.Text = "Save Changes";
                btnCancelEdit.Visible = true;
            }
        }
        else if (e.CommandName == "Delete")
        {
            // Check first instead of just try/catching the foreign key error -
            // gives a clearer message and avoids relying on parsing a database
            // error string, which is fragile across MySQL versions.
            string checkQuery = "SELECT COUNT(*) FROM Therapists WHERE CategoryId = @CategoryId";
            object count = DBHelper.ExecuteScalar(checkQuery, new MySqlParameter("@CategoryId", categoryId));

            if (Convert.ToInt32(count) > 0)
            {
                lblMessage.CssClass = "text-danger";
                lblMessage.Text = "Can't delete - one or more therapists are still using this category. Reassign them first.";
            }
            else
            {
                DBHelper.ExecuteNonQuery(
                    "DELETE FROM TherapyCategories WHERE CategoryId = @CategoryId",
                    new MySqlParameter("@CategoryId", categoryId));

                lblMessage.CssClass = "text-success";
                lblMessage.Text = "Category deleted.";
            }
        }

        LoadCategories();
    }
}
