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
        private static SqlConnection connection;

        public static bool Connected { get; set; } = false;

        public static SqlConnection Open()
        {
            if (connection == null)
            {
                connection = new SqlConnection(ConfigurationManager.ConnectionStrings["OnlineLibrarySystemEntities"].ConnectionString);
            }
            if (connection.State != ConnectionState.Open)
            {
                connection.Open();
            }
            Connected = true;
            return connection;
        }

        public static void Shutdown()
        {
            if (connection != null && connection.State == ConnectionState.Open)
            {
                connection.Close();
            }
            Connected = false;
        }

        public static SqlDataReader ExecuteQuery(string sql, params KeyValuePair<string, string>[] pairs)
        {
            if (!Connected) Open();
            SqlCommand cmd = new SqlCommand(sql, connection);
            foreach (var pair in pairs)
            {
                cmd.Parameters.AddWithValue(pair.Key, pair.Value);
            }
            return cmd.ExecuteReader();
        }
    }
}