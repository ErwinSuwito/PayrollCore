using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PayrollCore.Entities;

namespace PayrollCore
{
    public class GlobalSettings
    {
        public Exception lastEx
        {
            get;
            private set;
        }

        public string connString;
        public GlobalSettings(string _connString)
        {
            connString = _connString;
        }

        /// <summary>
        /// Gets the value of a global settings
        /// </summary>
        /// <param name="Key"></param>
        /// <returns></returns>
        public async Task<string> GetGlobalSettingsByKeyAsync(string Key)
        {
            lastEx = null;
            string Query = "SELECT SettingValue FROM Global_Settings WHERE SettingKey=@Key";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@Key", Key));

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (dr.Read())
                            {
                                return dr.GetString(0);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[DataAccess] Exception: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Updates the value of a global settings
        /// </summary>
        /// <param name="Key"></param>
        /// <param name="Value"></param>
        /// <returns></returns>
        public async Task<bool> UpdateGlobalSettingsAsync(string Key, string Value)
        {
            lastEx = null;

            string Query = "UPDATE Global_Settings SET SettingValue=@Value WHERE SettingKey=@Key";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@Key", Key));
                        cmd.Parameters.Add(new SqlParameter("@Value", Value));

                        await cmd.ExecuteNonQueryAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[DataAccess] Exception: " + ex.Message);
                return false;
            }
        }

    }
}
