using CommunityToolkit.Mvvm;
using MainApp.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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

        public long AddNewJob(JobPosting newJobPosting)
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DEFAULT_DATABASE_FILENAME}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                INSERT INTO {DEFAULT_TABLE_NAME}
                (Title, CompanyName, PostingURL, Type, Arrangement, Location, Distance, Description, Status)
                VALUES
                    (
                        @title,
                        @companyName,
                        @postingUrl,
                        @type,
                        @arrangement,
                        @location,
                        @distance,
                        @description,
                        @status
                    );
                """;

            command.Parameters.AddWithValue("@title", newJobPosting.JobTitle);
            command.Parameters.AddWithValue("@companyName", newJobPosting.JobCompanyName);
            command.Parameters.AddWithValue("@postingUrl", newJobPosting.JobPostingURL);
            command.Parameters.AddWithValue("@type", (int)newJobPosting.JobType);
            command.Parameters.AddWithValue("@arrangement", (int)newJobPosting.JobArrangement);
            command.Parameters.AddWithValue("@location", newJobPosting.JobLocation);
            command.Parameters.AddWithValue("@distance", (double)newJobPosting.JobDistance);
            command.Parameters.AddWithValue("@description", newJobPosting.JobDescription);
            command.Parameters.AddWithValue("@status", (int)newJobPosting.JobStatus);

            int executeResult = command.ExecuteNonQuery();
            Trace.WriteLine($"AddNewJob Result: (#Rows Modified): {executeResult}");

            // Get the row id from the newly inserted row.
            using SqliteCommand rowIdCommand = connection.CreateCommand();
            rowIdCommand.CommandText = "SELECT last_insert_rowid();";
            return (long)rowIdCommand.ExecuteScalar()!;
        }

        public void UpdateJobPosting(JobPosting posting)
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DEFAULT_DATABASE_FILENAME}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                UPDATE {DEFAULT_TABLE_NAME}
                    SET
                        Title       = @title,
                        CompanyName = @companyName,
                        PostingURL  = @postingUrl,
                        Type        = @type,
                        Arrangement = @arrangement,
                        Location    = @location,
                        Distance    = @distance,
                        Description = @description,
                        Status      = @status
                WHERE rowid = @rowId;
                """;

            command.Parameters.AddWithValue("@title", posting.JobTitle);
            command.Parameters.AddWithValue("@companyName", posting.JobCompanyName);
            command.Parameters.AddWithValue("@postingUrl", posting.JobPostingURL);
            command.Parameters.AddWithValue("@type", (int)posting.JobType);
            command.Parameters.AddWithValue("@arrangement", (int)posting.JobArrangement);
            command.Parameters.AddWithValue("@location", posting.JobLocation);
            command.Parameters.AddWithValue("@distance", (double)posting.JobDistance);
            command.Parameters.AddWithValue("@description", posting.JobDescription);
            command.Parameters.AddWithValue("@status", (int)posting.JobStatus);
            command.Parameters.AddWithValue("@rowId", posting.RowID);

            int rowsAffected = command.ExecuteNonQuery();
            Trace.WriteLine($"DB: Posting Updated ({rowsAffected} rows affected)");
        }

        public void RefreshJobPostings(ObservableCollection<JobPosting> jobPostings)
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DEFAULT_DATABASE_FILENAME}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                SELECT rowid, *
                FROM {DEFAULT_TABLE_NAME};
            """;

            using SqliteDataReader reader = command.ExecuteReader();

            jobPostings.Clear();

            // Get header indices
            Hashtable columnNames = new Hashtable();
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                columnNames[i] = columnName;
                columnNames[columnName] = i;
            }

            // Print each row
            while (reader.Read())
            {
                JobPosting jobPosting = new JobPosting();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = (string)columnNames[i]!;
                    object value = reader.GetValue(i);
                    switch (columnName)
                    {
                        case "rowid":
                            jobPosting.RowID = Convert.ToInt64(value); 
                            break;
                        case "Title":
                            jobPosting.JobTitle = (string)value;
                            break;
                        case "CompanyName":
                            jobPosting.JobCompanyName = (string)value;
                            break;
                        case "PostingURL":
                            jobPosting.JobPostingURL = (string)value;
                            break;
                        case "Type":
                            jobPosting.JobType = (JobType)Convert.ToInt32(value);
                            break;
                        case "Arrangement":
                            jobPosting.JobArrangement = (JobArrangement)Convert.ToInt32(value);
                            break;
                        case "Location":
                            jobPosting.JobLocation = (string)value;
                            break;
                        case "Distance":
                            jobPosting.JobDistance = Convert.ToSingle(value);
                            break;
                        case "Description":
                            jobPosting.JobDescription = (string)value;
                            break;
                        case "Status":
                            jobPosting.JobStatus = (JobStatus)Convert.ToInt32(value);
                            break;
                        default:
                            Trace.WriteLine($"Unknown ColumnName: {columnName}");
                            break;
                    }

                }

                // Posting construction complete
                jobPostings.Add(jobPosting);
            }
        }
    }
}
