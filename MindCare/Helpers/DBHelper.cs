using System;
using System.Configuration;
using System.Data;
using MySql.Data.MySqlClient;

/// <summary>
/// DBHelper is a single reusable class that every page uses to talk to MySQL.
/// Keeping all database code in one helper avoids repeating connection logic
/// on every page, and makes it easy to change the connection string in one place.
/// </summary>
public class DBHelper
{
    // Reads the connection string named "MindCareDB" from Web.config
    private static string connectionString =
        ConfigurationManager.ConnectionStrings["MindCareDB"].ConnectionString;

    /// <summary>
    /// Use for SELECT queries. Returns the results as a DataTable
    /// so you can bind it directly to a GridView / Repeater / ListView.
    /// </summary>
    public static DataTable ExecuteSelect(string query, params MySqlParameter[] parameters)
    {
        DataTable dt = new DataTable();

        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            using (MySqlDataAdapter adapter = new MySqlDataAdapter(cmd))
            {
                adapter.Fill(dt);
            }
        }
        return dt;
    }

    /// <summary>
    /// Use for INSERT / UPDATE / DELETE queries.
    /// Returns the number of rows affected.
    /// </summary>
    public static int ExecuteNonQuery(string query, params MySqlParameter[] parameters)
    {
        int rowsAffected;

        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            con.Open();
            rowsAffected = cmd.ExecuteNonQuery();
        }
        return rowsAffected;
    }

    /// <summary>
    /// Use when you only need one value back, e.g. COUNT(*) or a newly
    /// generated Id. Returns the value as an object — cast it as needed.
    /// </summary>
    public static object ExecuteScalar(string query, params MySqlParameter[] parameters)
    {
        object result;

        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            con.Open();
            result = cmd.ExecuteScalar();
        }
        return result;
    }

    /// <summary>
    /// Runs an INSERT and returns the auto-generated Id of the new row.
    /// Handy right after registering a user, booking an appointment, etc.
    /// </summary>
    public static long ExecuteInsertAndGetId(string query, params MySqlParameter[] parameters)
    {
        using (MySqlConnection con = new MySqlConnection(connectionString))
        using (MySqlCommand cmd = new MySqlCommand(query, con))
        {
            if (parameters != null)
                cmd.Parameters.AddRange(parameters);

            con.Open();
            cmd.ExecuteNonQuery();
            return cmd.LastInsertedId;
        }
    }
}
