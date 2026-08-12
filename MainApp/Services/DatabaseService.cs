using CommunityToolkit.Mvvm;
using MainApp.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace MainApp.Services
{
    public class DatabaseService
    {
        private const string DEFAULT_DATABASE_FILENAME = "save.db";
        private const string DEFAULT_TABLE_NAME = "Jobs";

        public DatabaseService()
        {
            EnsureTableExists();
        }

        private bool DoesTableExist(string tableName = DEFAULT_TABLE_NAME)
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DEFAULT_DATABASE_FILENAME}");

            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                SELECT name 
                FROM sqlite_master 
                WHERE type='table' AND name='{tableName}';
            """;

            SqliteDataReader reader = command.ExecuteReader();
            return reader.HasRows;
        }

        public void EnsureTableExists()
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DEFAULT_DATABASE_FILENAME}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                CREATE TABLE IF NOT EXISTS {DEFAULT_TABLE_NAME}
                (
                    Title TEXT,
                    CompanyName TEXT,
                    PostingURL TEXT,
                    Type INTEGER,
                    Arrangement INTEGER,
                    Location TEXT,
                    Distance REAL,
                    Description TEXT,
                    Status INTEGER
                );
                """;

            int executeResult = command.ExecuteNonQuery();
            Trace.WriteLine($"EnsureTableExists Result: (#Rows Modified): {executeResult}");
        }

        public void AddNewJob(JobPosting newJobPosting)
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DEFAULT_DATABASE_FILENAME}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                INSERT INTO {DEFAULT_TABLE_NAME}
                VALUES
                    (
                        '{newJobPosting.JobTitle}', 
                        '{newJobPosting.JobCompanyName}', 
                        '{newJobPosting.JobPostingURL}',
                        {(int)newJobPosting.JobType}, 
                        {(int)newJobPosting.JobArrangement},
                        '{newJobPosting.JobLocation}',
                        {newJobPosting.JobDistance},
                        '{newJobPosting.JobDescription}',
                        {(int)newJobPosting.JobStatus}
                    );
                """;

            int executeResult = command.ExecuteNonQuery();
            Trace.WriteLine($"AddNewJob Result: (#Rows Modified): {executeResult}");
        }

        public void GetAllJobs()
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DEFAULT_DATABASE_FILENAME}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                SELECT *
                FROM {DEFAULT_TABLE_NAME};
            """;

            using SqliteDataReader reader = command.ExecuteReader();

            // TODO(Salads): Get all o' this and slam it in an ObservableCollection<JobPosting> for a Model.

            // Print column headers
            for (int i = 0; i < reader.FieldCount; i++)
            {
                Trace.Write($"{reader.GetName(i)}\t");
            }
            Trace.WriteLine("");

            // Print each row
            while (reader.Read())
            {
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    // Use type-specific getters (e.g., GetString, GetInt32) for better performance
                    Trace.Write($"{reader.GetValue(i)}\t");
                }
                Trace.WriteLine("");
            }
        }
    }
}
