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
    public class Locations
    {
        public Exception exception
        {
            get;
            private set;
        }

        public string connString;
        public Locations(string _connString)
        {
            connString = _connString;
        }

        /// <summary>
        /// Gets all locations in the database.
        /// </summary>
        /// <param name="GetDisabled">True to also get disabled locations. False to get enabled locations only.</param>
        /// <returns></returns>
        public async Task<ObservableCollection<Location>> GetLocationsAsync(bool GetDisabled)
        {
            string Query = "SELECT * FROM Location WHERE LocationName!='new-sys'";

            if (GetDisabled == false)
            {
                Query += " AND IsDisabled=0";
            }
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
                            ObservableCollection<Location> locations = new ObservableCollection<Location>();

                            while (dr.Read())
                            {
                                Location location = new Location();
                                location.locationID = dr.GetInt32(0);
                                location.locationName = dr.GetString(1);
                                location.enableGM = dr.GetBoolean(2);
                                location.isDisabled = dr.GetBoolean(3);
                                location.Shiftless = dr.GetBoolean(4);
                                location.updateLvString();

                                locations.Add(location);
                            }

                            return locations;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[Locations] Exception: " + ex.Message);
                return null;
            }

        }

        /// <summary>
        /// Gets the location by its ID.
        /// </summary>
        /// <param name="LocationID"></param>
        /// <returns></returns>
        public async Task<Location> GetLocationByIdAsync(int LocationID)
        {
            string Query = "SELECT * FROM Location WHERE locationID=@LocationID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@LocationID", LocationID));

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (dr.Read())
                            {
                                Location location = new Location();
                                location.locationID = dr.GetInt32(0);
                                location.locationName = dr.GetString(1);
                                location.enableGM = dr.GetBoolean(2);
                                location.isDisabled = dr.GetBoolean(3);
                                location.Shiftless = dr.GetBoolean(4);
                                location.updateLvString();

                                return location;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastError = ex;
                Debug.WriteLine("[DataAccess] Exception: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Updates the passed location.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> AddLocationAsync(Location location)
        {
            if (!string.IsNullOrEmpty(location.locationName))
            {
                string Query = "UPDATE Location SET LocationName=@LocationName, EnableGM=@EnableGM, IsDisabled=@IsDisabled, Shiftless=@Shiftless WHERE LocationID=@LocationID";
                try
                {
                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        conn.Open();
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = Query;
                            cmd.Parameters.Add(new SqlParameter("@LocationID", location.locationID));
                            cmd.Parameters.Add(new SqlParameter("@LocationName", location.locationName));
                            cmd.Parameters.Add(new SqlParameter("@EnableGM", location.enableGM));
                            cmd.Parameters.Add(new SqlParameter("@IsDisabled", location.isDisabled));
                            cmd.Parameters.Add(new SqlParameter("@Shiftless", location.Shiftless));

                            await cmd.ExecuteNonQueryAsync();

                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    exception = ex;
                    Debug.WriteLine("[Locations] Exception: " + ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
    }
}
