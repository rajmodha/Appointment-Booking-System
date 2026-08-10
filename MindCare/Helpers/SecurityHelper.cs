using System;
using System.Security.Cryptography;
using System.Text;

/// <summary>
/// SecurityHelper handles password hashing so we never store plain-text
/// passwords in the database. It also has small helpers for the
/// currently logged-in user's session values.
/// </summary>
public class SecurityHelper
{
    /// <summary>
    /// Converts a plain password into a SHA256 hash string.
    /// Call this BEFORE saving a password, and again when checking login.
    /// </summary>
    public static string HashPassword(string plainPassword)
    {
        using (SHA256 sha256 = SHA256.Create())
        {
            byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(plainPassword));
            StringBuilder builder = new StringBuilder();
            foreach (byte b in bytes)
                builder.Append(b.ToString("X2"));
            return builder.ToString();
        }
    }

    /// <summary>
    /// Generates a random token string, used for the "Forgot Password" email link.
    /// </summary>
    public static string GenerateResetToken()
    {
        return Guid.NewGuid().ToString("N") + DateTime.Now.Ticks;
    }
}
