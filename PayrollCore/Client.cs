using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Threading.Tasks;
using PayrollCore.Entities;

namespace PayrollCore
{
    /// <summary>
    /// Singleton class Client. Use this to interact with Payroll Database.
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

        public UserState UserState
        {
            get;
            private set;
        }

        public GlobalSettings GlobalSettings
        {
            get;
            private set;
        }

        public Cards Cards
        {
            get;
            private set;
        }
        #endregion

        public enum InitStages
        {
            NotStarted,
            InProgress,
            Success,
            Failed,
            FailedDbNotInitialized
        }

        public InitStages InitStatus = InitStages.NotStarted;

        public Exception lastError
        {
            get; private set;
        }

        private string dbConnString;

        public int LocationId { get; private set; }

        /// <summary>
        /// Initializes Client object. Creates new objects to get database data.
        /// </summary>
        /// <param name="dbConnString">The connection string for the apSHA database</param>
        /// <param name="cardConnString">The connection string for the cards database</param>
        /// <param name="locationId">The location ID for the location where the client is located at</param>
        /// /// <exception cref="ArgumentException">Thrown when the connection string is invalid or when can't connect to the database.</exception>
        public async void Initialize(string dbConnString, string cardConnString, int locationId)
        {
            InitStatus = InitStages.InProgress;

            UserState = new UserState();
            LocationId = locationId;
            this.Activities = new Activities(dbConnString);
            this.Claims = new Claims(dbConnString);
            this.Locations = new Locations(dbConnString);
            this.Meetings = new Meetings(dbConnString);
            this.Rates = new Rates(dbConnString);
            this.Shifts = new Shifts(dbConnString);
            this.Users = new Users(dbConnString);
            this.GlobalSettings = new GlobalSettings(dbConnString);
            this.Cards = new Cards(dbConnString);

            try
            {
                bool IsConnectable = await ValidatePayrollDb(dbConnString) && await TestConnString(cardConnString);
                if (IsConnectable == false)
                {
                    // Checks the value of InitStatus as we don't want to change the value of it
                    // if is changed by ValidatePayrollDb() method
                    if (InitStatus == InitStages.InProgress)
                    {
                        InitStatus = InitStages.Failed;
                    }
                }


            }
            catch (Exception ex)
            {
                InitStatus = InitStages.Failed;
                throw new ArgumentException("Unable to connect to the database.", nameof(dbConnString), ex);
            }
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
                bool IsSuccess = await TestConnString(connString);
                if (!IsSuccess)
                {
                    return false;
                }

                var locations = await Locations.GetLocationsAsync(false);
                var rates = await Rates.GetRatesAsync(false);
                GlobalSettings.Settings = await GlobalSettings.GetSettingsAsync();

                int locationsCount = locations.Count;
                int ratesCount = rates.Count;
                int settingsCount = GlobalSettings.Settings.Count;

                if (locationsCount > 1 && ratesCount > 1 && settingsCount > 6)
                {
                    return true;
                }
                else
                {
                    return false;
                }
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
