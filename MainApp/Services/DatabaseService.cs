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

        public void RefreshJobPostings(ObservableCollection<JobPosting> jobPostings)
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DEFAULT_DATABASE_FILENAME}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                SELECT *
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
