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
    public class Shifts
    {
        public Exception lastEx
        {
            get;
            private set;
        }

        public string connString;
        public Shifts(string _connString)
        {
            connString = _connString;
        }

        /// <summary>
        /// Get the requested shift
        /// </summary>
        /// <param name="ShiftID"></param>
        /// <returns></returns>
        public async Task<Shift> GetShiftByIdAsync(int ShiftID)
        {
            string Query = "SELECT * FROM Shifts WHERE ShiftID=@ShiftID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@ShiftID", ShiftID));

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (dr.Read())
                            {
                                Shift shift = new Shift();
                                shift.ShiftID = dr.GetInt32(0);
                                shift.ShiftName = dr.GetString(1);
                                shift.StartTime = dr.GetTimeSpan(2);
                                shift.EndTime = dr.GetTimeSpan(3);
                                shift.LocationID = dr.GetInt32(4);
                                shift.DefaultRate = new Rate() { RateID = dr.GetInt32(5) };
                                shift.IsDisabled = dr.GetBoolean(6);
                                shift.WeekendOnly = dr.GetBoolean(7);

                                return shift;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Shifts] Exception: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Get all shifts in the table
        /// </summary>
        /// <param name="GetDisabled"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<Shift>> GetAllShiftsAsync(bool GetDisabled)
        {
            lastEx = null;
            string Query = "SELECT * FROM Shifts";

            if (!GetDisabled)
            {
                Query += " WHERE IsDisabled=0";
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
                            ObservableCollection<Shift> shifts = new ObservableCollection<Shift>();

                            while (dr.Read())
                            {
                                Shift shift = new Shift();
                                shift.ShiftID = dr.GetInt32(0);
                                shift.ShiftName = dr.GetString(1);
                                shift.StartTime = dr.GetTimeSpan(2);
                                shift.EndTime = dr.GetTimeSpan(3);
                                shift.LocationID = dr.GetInt32(4);
                                shift.DefaultRate = new Rate() { RateID = dr.GetInt32(5) };
                                shift.IsDisabled = dr.GetBoolean(6);
                                shift.WeekendOnly = dr.GetBoolean(7);

                                shifts.Add(shift);
                            }

                            return shifts;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Shifts] Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets all shifts on the specified location
        /// </summary>
        /// <param name="GetDisabled"></param>
        /// <param name="LocationID"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<Shift>> GetAllShiftsAsync(bool GetDisabled, int LocationID)
        {
            lastEx = null;
            string Query = "SELECT * FROM Shifts WHERE LocationID=@LocationID";

            if (!GetDisabled)
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
                        cmd.Parameters.Add(new SqlParameter("@LocationID", LocationID));

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            ObservableCollection<Shift> shifts = new ObservableCollection<Shift>();

                            while (dr.Read())
                            {
                                Shift shift = new Shift();
                                shift.ShiftID = dr.GetInt32(0);
                                shift.ShiftName = dr.GetString(1);
                                shift.StartTime = dr.GetTimeSpan(2);
                                shift.EndTime = dr.GetTimeSpan(3);
                                shift.LocationID = dr.GetInt32(4);
                                shift.DefaultRate = new Rate() { RateID = dr.GetInt32(5) };
                                shift.IsDisabled = dr.GetBoolean(6);
                                shift.WeekendOnly = dr.GetBoolean(7);

                                shifts.Add(shift);
                            }

                            return shifts;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Shifts] Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets disabled shifts based on its LocationID and ShiftName. Used to get
        /// shifts for special task and normal sign in shift.
        /// </summary>
        /// <param name="locationId"></param>
        /// <param name="shiftName"></param>
        /// <returns></returns>
        public async Task<Shift> GetSpecialShifts(int locationId, string shiftName)
        {
            var shifts = await GetAllShiftsAsync(true, locationId);

            foreach (Shift _shift in shifts)
            {
                if (_shift.ShiftName == shiftName && _shift.IsDisabled == true)
                {
                    return _shift;
                }
            }

            return null;
        }

        /// <summary>
        /// Adds a new shift
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public async Task<bool> AddNewShiftAsync(Shift shift)
        {
            lastEx = null;

            string Query = "INSERT INTO Shifts(ShiftName, StartTime, EndTime, LocationID, RateID, WeekendOnly, IsDisabled) VALUES(@ShiftName, @StartTime, @EndTime, @LocationID, @RateID, @WeekendOnly, @IsDisabled)";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@ShiftName", shift.ShiftName));
                        cmd.Parameters.Add(new SqlParameter("@StartTime", shift.StartTime));
                        cmd.Parameters.Add(new SqlParameter("@EndTime", shift.EndTime));
                        cmd.Parameters.Add(new SqlParameter("@LocationID", shift.LocationID));
                        cmd.Parameters.Add(new SqlParameter("@RateID", shift.DefaultRate.RateID));
                        cmd.Parameters.Add(new SqlParameter("@WeekendOnly", shift.WeekendOnly));
                        cmd.Parameters.Add(new SqlParameter("@IsDisabled", shift.IsDisabled));

                        await cmd.ExecuteNonQueryAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Shifts] Exception: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified shift
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public async Task<bool> DeleteShiftAsync(Shift shift)
        {
            string Query = "DELETE FROM Shifts WHERE ShiftID=@ShiftID";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@ShiftID", shift.ShiftID));

                        await cmd.ExecuteNonQueryAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Shifts] Exception: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates the specified shift
        /// </summary>
        /// <param name="shift"></param>
        /// <returns></returns>
        public async Task<bool> UpdateShiftAsync(Shift shift)
        {
            lastEx = null;

            string Query = "UPDATE Shifts SET ShiftName=@ShiftName, StartTime=@StartTime, EndTime=@EndTime, LocationID=@LocationID, RateID=@RateID, WeekendOnly=@WeekendOnly, IsDisabled=@IsDisabled WHERE ShiftID=@ShiftID";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@ShiftName", shift.ShiftName));
                        cmd.Parameters.Add(new SqlParameter("@StartTime", shift.StartTime));
                        cmd.Parameters.Add(new SqlParameter("@EndTime", shift.EndTime));
                        cmd.Parameters.Add(new SqlParameter("@LocationID", shift.LocationID));
                        cmd.Parameters.Add(new SqlParameter("@RateID", shift.DefaultRate.RateID));
                        cmd.Parameters.Add(new SqlParameter("@WeekendOnly", shift.WeekendOnly));
                        cmd.Parameters.Add(new SqlParameter("@IsDisabled", shift.IsDisabled));
                        cmd.Parameters.Add(new SqlParameter("@ShiftID", shift.ShiftID));

                        await cmd.ExecuteNonQueryAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Shifts] Exception: " + ex.Message);
                return false;
            }
        }
    }
}
