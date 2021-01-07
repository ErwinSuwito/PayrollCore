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
    public class Locations : DataObject
    {
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
            lastEx = null;
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
                                location.LocationID = dr.GetInt32(0);
                                location.LocationName = dr.GetString(1);
                                location.EnableGM = dr.GetBoolean(2);
                                location.IsDisabled = dr.GetBoolean(3);
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
                lastEx = ex;
                Debug.WriteLine("[Locations] Exception: " + ex.Message);
                return null;
            }

        }

        /// <summary>
        /// Gets the location by its ID.
        /// </summary>
        /// <param name="LocationID"></param>
        /// <returns></returns>
        public async Task<Location> GetLocationAsync(int LocationID)
        {
            lastEx = null;
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
                                location.LocationID = dr.GetInt32(0);
                                location.LocationName = dr.GetString(1);
                                location.EnableGM = dr.GetBoolean(2);
                                location.IsDisabled = dr.GetBoolean(3);
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
                lastEx = ex;
                Debug.WriteLine("[Locations] Exception: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Adds a new location
        /// </summary>
        /// <param name="location"></param>
        /// <returns>The LocationID of the new location</returns>
        public async Task<int> AddLocationAsync(Location location)
        {
            lastEx = null;

            string Query = "INSERT INTO Location(LocationName, EnableGM, IsDisabled, Shiftless) VALUES(@LocationName, @EnableGM, @IsDisabled, @Shiftless) select SCOPE_IDENTITY()";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@LocationName", location.LocationName));
                        cmd.Parameters.Add(new SqlParameter("@EnableGM", location.EnableGM));
                        cmd.Parameters.Add(new SqlParameter("@IsDisabled", location.IsDisabled));
                        cmd.Parameters.Add(new SqlParameter("@Shiftless", location.Shiftless));

                        var _locationID = await cmd.ExecuteScalarAsync();
                        int.TryParse(_locationID.ToString(), out int locationID);
                        return locationID;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Locations] Exception: " + ex.Message);
                return -1;
            }
        }


        /// <summary>
        /// Updates the passed location.
        /// </summary>
        /// <returns></returns>
        public async Task<bool> UpdateLocationAsync(Location location)
        {
            if (!string.IsNullOrEmpty(location.LocationName))
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
                            cmd.Parameters.Add(new SqlParameter("@LocationID", location.LocationID));
                            cmd.Parameters.Add(new SqlParameter("@LocationName", location.LocationName));
                            cmd.Parameters.Add(new SqlParameter("@EnableGM", location.EnableGM));
                            cmd.Parameters.Add(new SqlParameter("@IsDisabled", location.IsDisabled));
                            cmd.Parameters.Add(new SqlParameter("@Shiftless", location.Shiftless));

                            await cmd.ExecuteNonQueryAsync();

                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    Debug.WriteLine("[Locations] Exception: " + ex.Message);
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified location
        /// </summary>
        /// <param name="location"></param>
        /// <returns></returns>
        public async Task<bool> DeleteLocationAsync(Location location)
        {
            lastEx = null;
            string Query = "DELETE FROM Location WHERE LocationID=@LocationID";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@LocationID", location.LocationID));

                        await cmd.ExecuteNonQueryAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Locations] Exception: " + ex.Message);
                return false;
            }
        }

    }
}
