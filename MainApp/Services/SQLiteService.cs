using CommunityToolkit.Mvvm;
using MainApp.Models;
using Microsoft.Data.Sqlite;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Device.Location;
using System.Diagnostics;
using System.Text;

namespace MainApp.Services
{
    public class SQLiteService : IDatabaseService
    {
        private const int DB_JOBS_VERSION = 1;

        private const string DBNAME_CACHE = "cache";
        private const string TABLENAME_JOBS = "Jobs";
        private const string TABLENAME_JOBS_VERSION = "Version";
        private const string TABLENAME_CACHE_LOCATIONCOORDS = "Locations";
        private const string TABLENAME_CACHE_LOCATIONNAMES = "LocationNames";

        public SQLiteService()
        {
            EnsureTablesExist();
        }

        public void EnsureTablesExist()
        {
            EnsureJobsTableExists();
            EnsureCoordsCacheTableExists();
            EnsureLocationNameCacheTableExists();
            EnsureVersionTableExists();
        }

        private void EnsureJobsTableExists()
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={Settings.Default.SaveLocation}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                CREATE TABLE IF NOT EXISTS {TABLENAME_JOBS}
                (
                    Title TEXT,
                    CompanyName TEXT,
                    PostingURL TEXT,
                    Type INTEGER,
                    Arrangement INTEGER,
                    Location TEXT,
                    Distance INTEGER,
                    Description TEXT,
                    Status INTEGER
                );
                """;

            int executeResult = command.ExecuteNonQuery();
            Trace.WriteLine($"EnsureTableExists Result: (#Rows Modified): {executeResult}");
        }

        private void EnsureCoordsCacheTableExists()
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DBNAME_CACHE}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                CREATE TABLE IF NOT EXISTS {TABLENAME_CACHE_LOCATIONCOORDS}
                (
                    FullName TEXT,
                    Latitude REAL,
                    Longitude REAL
                );
                """;

            int executeResult = command.ExecuteNonQuery();
        }

        private void EnsureLocationNameCacheTableExists()
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DBNAME_CACHE}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                CREATE TABLE IF NOT EXISTS {TABLENAME_CACHE_LOCATIONNAMES}
                (
                    InputName TEXT,
                    FullName TEXT
                );
                """;

            int executeResult = command.ExecuteNonQuery();
        }

        private void EnsureVersionTableExists()
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={Settings.Default.SaveLocation}");
            connection.Open();

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"""
                CREATE TABLE IF NOT EXISTS {TABLENAME_JOBS_VERSION}
                (
                    Version INT
                );
                """;

                int executeResult = command.ExecuteNonQuery();
            }

            using (SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"""
                SELECT *
                FROM {TABLENAME_JOBS_VERSION}
                LIMIT 1;
                """;

                SqliteDataReader reader = command.ExecuteReader();
                if(!reader.HasRows)
                {
                    using (SqliteCommand versionCommand = connection.CreateCommand())
                    {
                        versionCommand.CommandText = $"""
                            INSERT INTO {TABLENAME_JOBS_VERSION}
                            (Version)
                            Values
                                (
                                    @version
                                );
                            """;

                        versionCommand.Parameters.AddWithValue("@version", DB_JOBS_VERSION);
                        versionCommand.ExecuteNonQuery();
                    }
                }
            }

        }

        public int? GetJobsDBVersion()
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={Settings.Default.SaveLocation}");
            connection.Open();

            using(SqliteCommand command = connection.CreateCommand())
            {
                command.CommandText = $"""
                    SELECT Version
                    FROM {TABLENAME_JOBS_VERSION}
                    LIMIT 1;
                    """;

                return command.ExecuteScalar() as int?;
            }
        }

        /// <summary>
        /// Get the full location name from cache if it exists
        /// </summary>
        /// <param name="inputLocation">The short or query string for a location.</param>
        /// <returns>The full location name as returned by Nominatim, otherwise null</returns>
        public string? GetLocationMappingFromCache(string inputLocation)
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DBNAME_CACHE}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                SELECT FullName 
                FROM {TABLENAME_CACHE_LOCATIONNAMES}
                WHERE InputName = @inputLocationName
                LIMIT 1;
            """;

            command.Parameters.AddWithValue("@inputLocationName", inputLocation);

            using SqliteDataReader reader = command.ExecuteReader();
            if(!reader.HasRows || !reader.Read())
            {
                return null;
            }
            else
            {
                return reader["FullName"] as string;
            }
        }

        public void EnsureLocationMappingExists(string inputLocation, string fullLocation)
        {
            string? existingFullLocation = GetLocationMappingFromCache(inputLocation);
            if (existingFullLocation != null)
            {
                if(fullLocation != existingFullLocation)
                {
                    // TODO(Salads): DB - Trying to input inputLocation -> full but different full location exists for same input.
                }

                return;
            }

            using SqliteConnection connection = new SqliteConnection($"Data Source={DBNAME_CACHE}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                INSERT INTO {TABLENAME_CACHE_LOCATIONNAMES}
                (InputName, FullName)
                VALUES
                    (
                        @inputName,
                        @fullName
                    );
            """;

            command.Parameters.AddWithValue("@inputName", inputLocation);
            command.Parameters.AddWithValue("@fullName", fullLocation);
            command.ExecuteNonQuery();
        }

        public GeoCoordinate? GetLocationCoordsFromCache(string fullName)
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={DBNAME_CACHE}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                SELECT Latitude, Longitude
                FROM {TABLENAME_CACHE_LOCATIONCOORDS}
                WHERE FullName = @fullName
                LIMIT 1;
            """;

            command.Parameters.AddWithValue("@fullName", fullName);

            using SqliteDataReader reader = command.ExecuteReader();
            if(!reader.HasRows || !reader.Read())
            {
                return null;
            }

            
            double? lat = reader["Latitude"] as double?;
            double? lon = reader["Longitude"] as double?;
            if (lat == null || lon == null)
            {
                return null;
            }

            GeoCoordinate? result = new GeoCoordinate((double)lat, (double)lon);
            return result;
        }

        public void EnsureLocationCoordsExists(string fullLocation, GeoCoordinate coords)
        {
            GeoCoordinate? result = GetLocationCoordsFromCache(fullLocation);
            if(result != null)
            {
                return;
            }

            using SqliteConnection connection = new SqliteConnection($"Data Source={DBNAME_CACHE}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                INSERT INTO {TABLENAME_CACHE_LOCATIONCOORDS}
                (FullName, Latitude, Longitude)
                VALUES
                    (
                        @fullName,
                        @latitude,
                        @longitude
                    );
            """;

            command.Parameters.AddWithValue("@fullName", fullLocation);
            command.Parameters.AddWithValue("@latitude", coords.Latitude);
            command.Parameters.AddWithValue("@longitude", coords.Longitude);
            command.ExecuteNonQuery();
        }

        public long AddNewJob(JobPosting newJobPosting)
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={Settings.Default.SaveLocation}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                INSERT INTO {TABLENAME_JOBS}
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
            command.Parameters.AddWithValue("@distance", (int)newJobPosting.JobDistance);
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
            using SqliteConnection connection = new SqliteConnection($"Data Source={Settings.Default.SaveLocation}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                UPDATE {TABLENAME_JOBS}
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
            command.Parameters.AddWithValue("@distance", (int)posting.JobDistance);
            command.Parameters.AddWithValue("@description", posting.JobDescription);
            command.Parameters.AddWithValue("@status", (int)posting.JobStatus);
            command.Parameters.AddWithValue("@rowId", posting.RowID);

            int rowsAffected = command.ExecuteNonQuery();
            Trace.WriteLine($"DB: Posting Updated ({rowsAffected} rows affected)");
        }

        public void RefreshJobPostings(ObservableCollection<JobPosting> jobPostings)
        {
            using SqliteConnection connection = new SqliteConnection($"Data Source={Settings.Default.SaveLocation}");
            connection.Open();

            using SqliteCommand command = connection.CreateCommand();
            command.CommandText = $"""
                SELECT rowid, *
                FROM {TABLENAME_JOBS};
            """;

            using SqliteDataReader reader = command.ExecuteReader();

            jobPostings.Clear();

            // Get column names
            string[] columnNames = new string[reader.FieldCount];
            for (int i = 0; i < reader.FieldCount; i++)
            {
                string columnName = reader.GetName(i);
                columnNames[i] = columnName;
            }

            // Print each row
            while (reader.Read())
            {
                JobPosting jobPosting = new JobPosting();
                for (int i = 0; i < reader.FieldCount; i++)
                {
                    string columnName = columnNames[i];
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
                            jobPosting.JobDistance = Convert.ToInt32(value);
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
