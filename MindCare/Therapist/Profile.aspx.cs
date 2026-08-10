using System;
using System.Data;
using System.IO;
using MySql.Data.MySqlClient;

public partial class Therapist_Profile : TherapistBasePage
{
    protected void Page_Load(object sender, EventArgs e)
    {
        TherapistSidebar1.ActivePage = "Profile";

        if (!IsPostBack)
        {
            LoadCategories();
            LoadProfile();
        }
    }

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

    private void LoadProfile()
    {
        string query = @"SELECT u.Email, u.FullName, u.Phone,
                                 t.CategoryId, t.Specialization, t.Qualification, t.Language, t.Location,
                                 t.Fees, t.ConsultationType, t.Bio, t.ApprovalStatus, t.ProfileImage
                          FROM Users u
                          INNER JOIN Therapists t ON t.UserId = u.UserId
                          WHERE u.UserId = @UserId";

        DataTable dt = DBHelper.ExecuteSelect(query, new MySqlParameter("@UserId", CurrentUserId));
        if (dt.Rows.Count == 0) return;

        DataRow row = dt.Rows[0];

        txtEmail.Text = row["Email"].ToString();
        txtFullName.Text = row["FullName"].ToString();
        txtPhone.Text = row["Phone"].ToString();

        // Show the uploaded photo if one exists; otherwise fall back to the
        // same generated placeholder used everywhere else in the site before
        // a real photo has been uploaded.
        string profileImage = row["ProfileImage"] == DBNull.Value ? null : row["ProfileImage"].ToString();
        imgCurrentPhoto.ImageUrl = string.IsNullOrEmpty(profileImage)
            ? "https://api.dicebear.com/7.x/initials/svg?seed=" + Server.UrlEncode(txtFullName.Text)
            : ResolveUrl(profileImage);

        if (ddlCategory.Items.FindByValue(row["CategoryId"].ToString()) != null)
            ddlCategory.SelectedValue = row["CategoryId"].ToString();

        txtSpecialization.Text = row["Specialization"].ToString();
        txtQualification.Text = row["Qualification"].ToString();
        txtLanguage.Text = row["Language"].ToString();
        txtLocation.Text = row["Location"].ToString();
        txtFees.Text = row["Fees"].ToString();
        txtBio.Text = row["Bio"].ToString();

        if (ddlConsultationType.Items.FindByValue(row["ConsultationType"].ToString()) != null)
            ddlConsultationType.SelectedValue = row["ConsultationType"].ToString();

        string status = row["ApprovalStatus"].ToString();
        litApprovalStatus.Text = "<span class='mc-badge mc-badge-" +
            (status == "Approved" ? "confirmed" : status == "Rejected" ? "rejected" : "pending") +
            "'>" + status + "</span>";
    }

    protected void btnSave_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        decimal fees;
        if (!decimal.TryParse(txtFees.Text, out fees) || fees < 0)
        {
            lblMessage.CssClass = "text-danger";
            lblMessage.Text = "Please enter a valid fee amount.";
            return;
        }

        // Handle the photo upload FIRST, separately from the rest of the form,
        // so a bad photo (wrong type/too large) doesn't also block saving the
        // rest of the profile fields, and vice versa.
        string uploadedImagePath = null;
        if (fileProfilePhoto.HasFile)
        {
            if (!TryUploadPhoto(out uploadedImagePath))
            {
                // TryUploadPhoto already set lblPhotoError.Text - stop here so
                // the rest of the profile isn't half-saved alongside a rejected photo.
                return;
            }
        }

        DBHelper.ExecuteNonQuery(
            "UPDATE Users SET FullName = @FullName, Phone = @Phone WHERE UserId = @UserId",
            new MySqlParameter("@FullName", txtFullName.Text.Trim()),
            new MySqlParameter("@Phone", txtPhone.Text.Trim()),
            new MySqlParameter("@UserId", CurrentUserId));

        // Only touch ProfileImage in the UPDATE if a new photo was actually
        // uploaded this time - otherwise leave whatever's already saved alone.
        if (uploadedImagePath != null)
        {
            DBHelper.ExecuteNonQuery(
                "UPDATE Therapists SET ProfileImage = @ProfileImage WHERE UserId = @UserId",
                new MySqlParameter("@ProfileImage", uploadedImagePath),
                new MySqlParameter("@UserId", CurrentUserId));
        }

        string query = @"UPDATE Therapists SET
                            CategoryId = @CategoryId,
                            Specialization = @Specialization,
                            Qualification = @Qualification,
                            Language = @Language,
                            Location = @Location,
                            Fees = @Fees,
                            ConsultationType = @ConsultationType,
                            Bio = @Bio
                          WHERE UserId = @UserId";

        DBHelper.ExecuteNonQuery(query,
            new MySqlParameter("@CategoryId", Convert.ToInt32(ddlCategory.SelectedValue)),
            new MySqlParameter("@Specialization", txtSpecialization.Text.Trim()),
            new MySqlParameter("@Qualification", txtQualification.Text.Trim()),
            new MySqlParameter("@Language", txtLanguage.Text.Trim()),
            new MySqlParameter("@Location", txtLocation.Text.Trim()),
            new MySqlParameter("@Fees", fees),
            new MySqlParameter("@ConsultationType", ddlConsultationType.SelectedValue),
            new MySqlParameter("@Bio", txtBio.Text.Trim()),
            new MySqlParameter("@UserId", CurrentUserId));

        Session["FullName"] = txtFullName.Text.Trim();

        lblMessage.CssClass = "text-success";
        lblMessage.Text = "Profile saved successfully.";

        LoadProfile(); // refresh the photo preview to show the newly uploaded one
    }

    /// <summary>
    /// Validates the uploaded file (extension + size) and saves it to
    /// ~/Uploads/TherapistPhotos/ with a unique filename so two therapists
    /// can never overwrite each other's photo. Returns the web-relative path
    /// to store in Therapists.ProfileImage, or null (with lblPhotoError set)
    /// if validation failed.
    /// </summary>
    private bool TryUploadPhoto(out string webRelativePath)
    {
        webRelativePath = null;

        string[] allowedExtensions = { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
        string extension = Path.GetExtension(fileProfilePhoto.FileName).ToLowerInvariant();

        if (Array.IndexOf(allowedExtensions, extension) < 0)
        {
            lblPhotoError.Text = "Please upload a JPG, PNG, GIF, or WEBP image.";
            return false;
        }

        const int maxSizeBytes = 2 * 1024 * 1024; // 2 MB
        if (fileProfilePhoto.PostedFile.ContentLength > maxSizeBytes)
        {
            lblPhotoError.Text = "Image is too large - please choose a file under 2 MB.";
            return false;
        }

        // A unique filename per upload (TherapistId + random suffix) means a
        // therapist re-uploading a photo with the same original filename
        // never overwrites or gets confused with someone else's.
        string uniqueFileName = "T" + CurrentUserId + "_" + Guid.NewGuid().ToString("N").Substring(0, 8) + extension;

        string folderPath = Server.MapPath("~/Uploads/TherapistPhotos/");
        if (!Directory.Exists(folderPath))
        {
            Directory.CreateDirectory(folderPath);
        }

        string physicalPath = Path.Combine(folderPath, uniqueFileName);
        fileProfilePhoto.SaveAs(physicalPath);

        webRelativePath = "~/Uploads/TherapistPhotos/" + uniqueFileName;
        return true;
    }
}
