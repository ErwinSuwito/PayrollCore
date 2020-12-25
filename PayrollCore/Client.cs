using System;
using System.Collections.Generic;
using System.Data.SqlClient;
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

        private string dbConnString;
        public Activities Activities
        {
            get;
            private set;
        }
        public Claims Claims
        {
            get;
            private set;
        }
        public Locations Locations
        {
            get;
            private set;
        }
        public Meetings Meetings
        {
            get;
            private set;
        }
        public Rates Rates
        {
            get;
            private set;
        }
        public Shifts Shifts
        {
            get;
            private set;
        }
        public Users Users
        {
            get;
            private set;
        }

        /// <summary>
        /// Initializes Client object. Creates new objects to get database data.
        /// </summary>
        /// <param name="connString"></param>
        public async void Initialize(string connString)
        {
            bool IsConnectable = await TestConnString(connString);
            if (IsConnectable)
            {
                Locations = new Locations(connString);
            }
        }

        public async Task<bool> TestConnString(string connString)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    await conn.OpenAsync();
                    return true;
                }
            }
            catch (Exception ex)
            {
                return false;
            }
        }
    }
}
