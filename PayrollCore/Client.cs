﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;

namespace PayrollCore
{
    /// <summary>
    /// Singleton class Client. Use this to 
    /// </summary>
    public class Client
    {
        #region "Constructors"
        private static Client instance;
        static Client()
        {
            instance = new Client();
        }

        public static Client Instance
        {
            get { return instance; }
        }
        #endregion

        #region "Data objects"

        /// <summary>
        /// Requests for activities data
        /// </summary>
        public Activities Activities
        {
            get;
            private set;
        }

        /// <summary>
        /// Requests for claims data
        /// </summary>
        public Claims Claims
        {
            get;
            private set;
        }

        /// <summary>
        /// Requests for Locations data
        /// </summary>
        public Locations Locations
        {
            get;
            private set;
        }
        
        /// <summary>
        /// Requests for Meetings data
        /// </summary>
        public Meetings Meetings
        {
            get;
            private set;
        }

        /// <summary>
        /// Requests for Rates data
        /// </summary>
        public Rates Rates
        {
            get;
            private set;
        }

        /// <summary>
        /// Requests for Shifts data
        /// </summary>
        public Shifts Shifts
        {
            get;
            private set;
        }

        /// <summary>
        /// Requests for Users data
        /// </summary>
        public Users Users
        {
            get;
            private set;
        }
        #endregion

        public Exception lastError
        {
            get; private set;
        }

        private string dbConnString;

        /// <summary>
        /// Initializes Client object. Creates new objects to get database data.
        /// </summary>
        /// <param name="connString"></param>
        public async void Initialize(string connString)
        {
            bool IsConnectable = await TestConnString(connString);
            this.Activities = new Activities(connString);
            this.Claims = new Claims(connString);
            this.Locations = new Locations(connString);
            this.Meetings = new Meetings(connString);
            this.Rates = new Rates(connString);
            this.Shifts = new Shifts(connString);
            this.Users = new Users(connString);
        }

        /// <summary>
        /// Tests the passed connection string. This method does not handle any exception.
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public async Task<bool> TestConnString(string connString)
        {
            using (SqlConnection conn = new SqlConnection(connString))
            {
                await conn.OpenAsync();
                return true;
            }
        }

        /// <summary>
        /// Executes passed scripts to the database. Only use this to initialize new database.
        /// </summary>
        /// <param name="script">The script string to be executed.</param>
        /// <returns></returns>
        public async Task<bool> ExecuteScript(string script)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(dbConnString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = script;
                        int? result = await cmd.ExecuteNonQueryAsync();

                        return (result != null) ? true : false;
                    }
                }
            }
            catch (Exception ex)
            {
                lastError = ex;
                Debug.WriteLine("[Client] Exception: " + ex.Message);
            }

            return false;
        }

        /// <summary>
        /// Checks if important data is on the payroll database
        /// </summary>
        /// <param name="connString"></param>
        /// <returns></returns>
        public async Task<bool> ValidatePayrollDb (string connString)
        {
            try
            {
                return await TestConnString(connString);
            }
            catch (Exception ex)
            {
                lastError = ex;
                Debug.WriteLine("[Client] Exception when testing payroll database. View lastError to check the actual exception");
                return false;
            }
        }
    }
}
