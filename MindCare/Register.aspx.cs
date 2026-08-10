using System;
using MySql.Data.MySqlClient;

public partial class Register : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
    }

    protected void btnRegister_Click(object sender, EventArgs e)
    {
        if (!Page.IsValid) return;

        string fullName = txtFullName.Text.Trim();
        string email = txtEmail.Text.Trim();
        string phone = txtPhone.Text.Trim();
        string password = txtPassword.Text;
        int roleId = Convert.ToInt32(rblRole.SelectedValue);

        // 1. Make sure this email is not already registered
        string checkQuery = "SELECT COUNT(*) FROM Users WHERE Email = @Email";
        object existing = DBHelper.ExecuteScalar(checkQuery, new MySqlParameter("@Email", email));

        if (Convert.ToInt32(existing) > 0)
        {
            lblMessage.Text = "This email is already registered. Please login instead.";
            return;
        }

        // 2. Insert into the shared Users table
        string hashedPassword = SecurityHelper.HashPassword(password);

        string insertUserQuery = @"INSERT INTO Users (FullName, Email, PasswordHash, Phone, RoleId)
                                    VALUES (@FullName, @Email, @PasswordHash, @Phone, @RoleId)";

        long newUserId = DBHelper.ExecuteInsertAndGetId(insertUserQuery,
            new MySqlParameter("@FullName", fullName),
            new MySqlParameter("@Email", email),
            new MySqlParameter("@PasswordHash", hashedPassword),
            new MySqlParameter("@Phone", phone),
            new MySqlParameter("@RoleId", roleId));

        // 3. Insert the role-specific profile row
        if (roleId == 3) // Patient
        {
            string insertPatient = "INSERT INTO Patients (UserId) VALUES (@UserId)";
            DBHelper.ExecuteNonQuery(insertPatient, new MySqlParameter("@UserId", newUserId));

            // Redirect to Login instead of staying here - Login.aspx reads the
            // "registered" flag and shows the right success banner for the role.
            Response.Redirect("~/Login.aspx?registered=patient");
        }
        else // Therapist - profile stays "Pending" until Admin approves
        {
            string insertTherapist = @"INSERT INTO Therapists (UserId, CategoryId, Fees, ApprovalStatus)
                                        VALUES (@UserId, 1, 0, 'Pending')";
            DBHelper.ExecuteNonQuery(insertTherapist, new MySqlParameter("@UserId", newUserId));

            Response.Redirect("~/Login.aspx?registered=therapist");
        }
    }
}
