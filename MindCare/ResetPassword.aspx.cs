using System;
using System.Data;
using MySql.Data.MySqlClient;

public partial class ResetPassword : System.Web.UI.Page
{
    private string token;

    protected void Page_Load(object sender, EventArgs e)
    {
        token = Request.QueryString["token"];

        if (string.IsNullOrWhiteSpace(token))
        {
            ShowInvalidToken();
            return;
        }

        if (!IsPostBack)
        {
            if (!IsTokenValid(token))
            {
                ShowInvalidToken();
            }
        }
    }

    private void ShowInvalidToken()
    {
        pnlForm.Visible = false;
        pnlInvalidToken.Visible = true;
    }

    /// <summary>
    /// A token is valid only if it exists, hasn't expired, and hasn't already
    /// been used - each of those is a separate reason a link could be dead,
    /// but we only need a single boolean here since ResetPassword.aspx shows
    /// the same "expired" message either way (no need to tell someone exactly
    /// why a token failed - that's not useful information to leak).
    /// </summary>
    private bool IsTokenValid(string tokenToCheck)
    {
        string query = @"SELECT COUNT(*) FROM PasswordResetTokens
                          WHERE Token = @Token AND IsUsed = 0 AND ExpiryDate > NOW()";

        object result = DBHelper.ExecuteScalar(query, new MySqlParameter("@Token", tokenToCheck));
        return Convert.ToInt32(result) > 0;
    }

    protected void btnResetPassword_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        if (!IsTokenValid(token))
        {
            ShowInvalidToken();
            return;
        }

        string findUserQuery = @"SELECT UserId FROM PasswordResetTokens
                                  WHERE Token = @Token AND IsUsed = 0 AND ExpiryDate > NOW()";

        object userIdResult = DBHelper.ExecuteScalar(findUserQuery, new MySqlParameter("@Token", token));

        if (userIdResult == null)
        {
            ShowInvalidToken();
            return;
        }

        int userId = Convert.ToInt32(userIdResult);
        string newHashedPassword = SecurityHelper.HashPassword(txtNewPassword.Text);

        // Update the password...
        DBHelper.ExecuteNonQuery(
            "UPDATE Users SET PasswordHash = @PasswordHash WHERE UserId = @UserId",
            new MySqlParameter("@PasswordHash", newHashedPassword),
            new MySqlParameter("@UserId", userId));

        // ...then immediately burn the token so it can't be reused (e.g. if the
        // reset email link was somehow intercepted after the real owner used it).
        DBHelper.ExecuteNonQuery(
            "UPDATE PasswordResetTokens SET IsUsed = 1 WHERE Token = @Token",
            new MySqlParameter("@Token", token));

        pnlForm.Visible = false;
        pnlSuccess.Visible = true;
    }
}
