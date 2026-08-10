using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class Login : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!IsPostBack)
        {
            ShowRegisteredBannerIfNeeded();
        }
    }

    /// <summary>
    /// Register.aspx.cs redirects here with ?registered=patient or
    /// ?registered=therapist right after a successful sign-up, instead of
    /// showing a message on the Register page itself - this way refreshing
    /// or bookmarking Login.aspx never re-submits the registration form.
    /// </summary>
    private void ShowRegisteredBannerIfNeeded()
    {
        string registered = Request.QueryString["registered"];

        if (registered == "patient")
        {
            lblRegisteredBanner.Visible = true;
            lblRegisteredBanner.Text = "Registration successful! You can now log in.";
        }
        else if (registered == "therapist")
        {
            lblRegisteredBanner.Visible = true;
            lblRegisteredBanner.Text = "Registration submitted! Your account needs Admin approval " +
                                        "before you can log in and accept patients.";
        }
    }

    protected void btnLogin_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        string email = txtEmail.Text.Trim();
        string hashedPassword = SecurityHelper.HashPassword(txtPassword.Text);

        string query = @"SELECT UserId, FullName, RoleId, IsActive
                          FROM Users
                          WHERE Email = @Email AND PasswordHash = @PasswordHash";

        DataTable dt = DBHelper.ExecuteSelect(query,
            new MySqlParameter("@Email", email),
            new MySqlParameter("@PasswordHash", hashedPassword));

        if (dt.Rows.Count == 0)
        {
            lblMessage.Text = "Invalid email or password.";
            return;
        }

        DataRow row = dt.Rows[0];

        if (Convert.ToBoolean(row["IsActive"]) == false)
        {
            lblMessage.Text = "Your account has been disabled. Please contact support.";
            return;
        }

        int roleId = Convert.ToInt32(row["RoleId"]);

        // If this is a therapist, block login until Admin has approved them
        if (roleId == 2)
        {
            string approvalQuery = "SELECT ApprovalStatus FROM Therapists WHERE UserId = @UserId";
            object status = DBHelper.ExecuteScalar(approvalQuery, new MySqlParameter("@UserId", row["UserId"]));

            if (status != null && status.ToString() == "Pending")
            {
                lblMessage.Text = "Your therapist account is awaiting Admin approval.";
                return;
            }
            if (status != null && status.ToString() == "Rejected")
            {
                lblMessage.Text = "Your therapist application was not approved. Please contact support.";
                return;
            }
        }

        // Store the essentials in Session - every protected page checks these
        Session["UserId"] = row["UserId"].ToString();
        Session["FullName"] = row["FullName"].ToString();
        Session["RoleId"] = roleId.ToString();

        // If the person was redirected here from a protected page (e.g. clicking
        // "Book Appointment" while logged out), send them straight back there.
        // We only accept URLs that start with a single "/" so this can't be
        // abused to redirect people off-site (open redirect protection).
        string returnUrl = Request.QueryString["ReturnUrl"];
        if (!string.IsNullOrWhiteSpace(returnUrl) &&
            returnUrl.StartsWith("/") && !returnUrl.StartsWith("//"))
        {
            Response.Redirect(returnUrl);
            return;
        }

        // Otherwise redirect to the correct dashboard based on role
        if (roleId == 1)
            Response.Redirect("~/Admin/AdminDashboard.aspx");
        else if (roleId == 2)
            Response.Redirect("~/Therapist/TherapistDashboard.aspx");
        else
            Response.Redirect("~/Patient/PatientDashboard.aspx");
    }
}
