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
    public class GlobalSettings : DataObject
    {
        public GlobalSettings(string _connString)
        {
            connString = _connString;
        }

        public Dictionary<string, string> Settings { get; internal set; }

        public async Task<Dictionary<string, string>> GetSettingsAsync()
        {
            lastEx = null;
            string Query = "SELECT * FROM Global_Settings";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            Dictionary<string, string> globalSettings = new Dictionary<string, string>();

                            while (dr.Read())
                            {
                                globalSettings.Add(dr.GetString(0), dr.GetString(1));
                            }

                            return globalSettings;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Rates] Exception: " + ex.Message);
                return null;
            }
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
                Debug.WriteLine("[Global Settings] Exception: " + ex.Message);
                return false;
            }
        }

    }
}
