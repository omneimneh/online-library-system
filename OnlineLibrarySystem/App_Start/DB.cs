using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace OnlineLibrarySystem.Controllers
{
    public class DB
    {
        public static readonly string ConnectionString =
            ConfigurationManager.ConnectionStrings["OnlineLibrarySystemEntities"].ConnectionString;

        public static SqlDataReader ExecuteQuery(SqlConnection Connection, string sql, params KeyValuePair<string, object>[] pairs)
        {
            SqlCommand cmd = new SqlCommand(sql, Connection);
            foreach (var pair in pairs)
            {
                cmd.Parameters.AddWithValue(pair.Key, pair.Value);
            }
            return cmd.ExecuteReader();
        }

        public static void ExecuteNonQuery(SqlConnection Connection, string sql, params KeyValuePair<string, object>[] pairs)
        {
            SqlCommand cmd = new SqlCommand(sql, Connection);
            foreach (var pair in pairs)
            {
                cmd.Parameters.AddWithValue(pair.Key, pair.Value);
            }
            cmd.ExecuteNonQuery();
        }

        public static int ExecuteInsertQuery(SqlConnection Connection, string sql, params KeyValuePair<string, object>[] pairs)
        {
            SqlCommand cmd = new SqlCommand(sql + " SELECT SCOPE_IDENTITY()", Connection);
            foreach (var pair in pairs)
            {
                cmd.Parameters.AddWithValue(pair.Key, pair.Value);
            }
            return Convert.ToInt32(cmd.ExecuteScalar());
        }

        public static int ExecuteScalar(SqlConnection Connection, string sql, params KeyValuePair<string, object>[] pairs)
        {
            SqlCommand cmd = new SqlCommand(sql, Connection);
            foreach (var pair in pairs)
            {
                cmd.Parameters.AddWithValue(pair.Key, pair.Value);
            }
            return Convert.ToInt32(cmd.ExecuteScalar());
        }
    }
}