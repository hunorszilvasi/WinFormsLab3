using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;

namespace WinFormsLab3
{
    public class SQLiteHandler
    {
        private SqliteConnection connection;

        public SQLiteHandler()
        {
            connection = null;
        }

        public void ConnectToDb()
        {
            string connectionString = "Data Source=keywords.db";

            connection = new SqliteConnection(connectionString);
            connection.Open();

            string createTableQuery = @"
                CREATE TABLE IF NOT EXISTS Keywords (
                    id INTEGER PRIMARY KEY AUTOINCREMENT,
                    keyword TEXT
                );";

            using (SqliteCommand command =
                   new SqliteCommand(createTableQuery, connection))
            {
                command.ExecuteNonQuery();
            }
        }

        public void DisconnectFromDb()
        {
            if (connection != null)
            {
                connection.Close();
                connection.Dispose();
                connection = null;
            }
        }

        public void InsertKeyword(string k)
        {
            if (connection == null ||
                connection.State != System.Data.ConnectionState.Open)
            {
                throw new InvalidOperationException(
                    "Database is not connected.");
            }

            string query =
                "INSERT INTO Keywords (keyword) VALUES (@keyword);";

            using (SqliteCommand command =
                   new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@keyword", k);
                command.ExecuteNonQuery();
            }
        }

        public List<string> GetAllKeywords()
        {
            List<string> keywords = new List<string>();

            if (connection == null ||
                connection.State != System.Data.ConnectionState.Open)
            {
                throw new InvalidOperationException(
                    "Database is not connected.");
            }

            string query = "SELECT keyword FROM Keywords;";

            using (SqliteCommand command =
                   new SqliteCommand(query, connection))
            using (SqliteDataReader reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    keywords.Add(reader.GetString(0));
                }
            }

            return keywords;
        }

        public void DeleteAllKeywords()
        {
            if (connection == null ||
                connection.State != System.Data.ConnectionState.Open)
            {
                throw new InvalidOperationException(
                    "Database is not connected.");
            }

            string query = "DELETE FROM Keywords;";

            using (SqliteCommand command =
                   new SqliteCommand(query, connection))
            {
                command.ExecuteNonQuery();
            }
        }
        public void DeleteKeyword(string keyword)
        {
            if (connection == null ||
                connection.State != System.Data.ConnectionState.Open)
            {
                throw new InvalidOperationException(
                    "Database is not connected.");
            }

            string query = "DELETE FROM Keywords WHERE keyword = @keyword;";

            using (SqliteCommand command =
                   new SqliteCommand(query, connection))
            {
                command.Parameters.AddWithValue("@keyword", keyword);
                command.ExecuteNonQuery();
            }
        }
    }
}