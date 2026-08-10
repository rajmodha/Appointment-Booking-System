using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class ForgotPassword : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnSendLink_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        string email = txtEmail.Text.Trim();

        string userQuery = "SELECT UserId, FullName FROM Users WHERE Email = @Email AND IsActive = 1";
        DataTable dt = DBHelper.ExecuteSelect(userQuery, new MySqlParameter("@Email", email));

        // IMPORTANT: we show the same "check your email" message whether or not
        // the account actually exists. This stops someone from using this form
        // to find out which emails are registered on the site.
        if (dt.Rows.Count > 0)
        {
            int userId = Convert.ToInt32(dt.Rows[0]["UserId"]);
            string fullName = dt.Rows[0]["FullName"].ToString();

            string token = SecurityHelper.GenerateResetToken();

            string insertTokenQuery = @"INSERT INTO PasswordResetTokens (UserId, Token, ExpiryDate)
                                         VALUES (@UserId, @Token, @ExpiryDate)";

            DBHelper.ExecuteNonQuery(insertTokenQuery,
                new MySqlParameter("@UserId", userId),
                new MySqlParameter("@Token", token),
                new MySqlParameter("@ExpiryDate", DateTime.Now.AddHours(1)));

            string resetLink = ResolveUrl("~/ResetPassword.aspx") + "?token=" + Server.UrlEncode(token);
            string fullResetUrl = "https://" + Request.Url.Authority + resetLink;

            string emailBody = "<h2>Reset your MindCare password</h2>" +
                                "<p>Hi " + fullName + ",</p>" +
                                "<p>Click the link below to reset your password. This link expires in 1 hour.</p>" +
                                "<p><a href='" + fullResetUrl + "'>Reset My Password</a></p>" +
                                "<p>If you didn't request this, you can safely ignore this email.</p>";

            EmailHelper.SendEmail(email, "Reset your MindCare password", emailBody);
        }

        pnlForm.Visible = false;
        pnlConfirmation.Visible = true;
    }
}
