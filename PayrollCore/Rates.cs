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
    public class Rates
    {
        public Exception lastEx
        {
            get;
            private set;
        }

        public string connString;
        public Rates(string _connString)
        {
            connString = _connString;
        }

        /// <summary>
        /// Gets the requested Rate
        /// </summary>
        /// <param name="RateID"></param>
        /// <returns></returns>
        public async Task<Rate> GetRateById(int RateID)
        {
            lastEx = null;

            string Query = "SELECT * FROM Rate WHERE RateID=@RateID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@RateID", RateID));

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (dr.Read())
                            {
                                Rate rate = new Rate();
                                rate.RateID = dr.GetInt32(0);
                                rate.RateDesc = dr.GetString(1);
                                rate.Amount = dr.GetFloat(2);
                                rate.isDisabled = dr.GetBoolean(3);

                                return rate;
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
        /// Get all rates on the database and returns it in an ObservableCollection
        /// </summary>
        /// <returns></returns>
        public async Task<ObservableCollection<Rate>> GetAllRatesAsync(bool GetDisabled)
        {
            lastEx = null;
            string Query = "SELECT * FROM Rate";

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
                            ObservableCollection<Rate> rates = new ObservableCollection<Rate>();

                            while (dr.Read())
                            {
                                Rate rate = new Rate();
                                rate.RateID = dr.GetInt32(0);
                                rate.RateDesc = dr.GetString(1);
                                rate.Amount = dr.GetFloat(2);
                                rate.isDisabled = dr.GetBoolean(3);

                                rates.Add(rate);
                            }

                            return rates;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[DataAccess] Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Adds a rate to the database
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public async Task<bool> AddRateAsync(Rate rate)
        {
            lastEx = null;

            string Query = "INSERT INTO rate(RateDesc, rate) VALUES(@RateDesc, @Rate)";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@RateDesc", rate.RateDesc));
                        cmd.Parameters.Add(new SqlParameter("@Rate", rate.Amount));

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

        /// <summary>
        /// Deletes the specified rate
        /// </summary>
        /// <param name="rate"></param>
        /// <returns></returns>
        public async Task<bool> DeleteRateAsync(Rate rate)
        {
            string Query = "DELETE FROM Rate WHERE RateID=@RateID";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@RateID", rate.RateID));

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

        /// <summary>
        /// Updates the specified rate
        /// </summary>
        /// <param name="rate">The rate to be updated</param>
        /// <returns></returns>
        public async Task<bool> UpdateRateAsync(Rate rate)
        {
            lastEx = null;

            string Query = "UPDATE Rate SET RateDesc=@RateDesc, Rate=@Rate, IsDisabled=@IsDisabled WHERE RateID=@RateID";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@RateDesc", rate.RateDesc));
                        cmd.Parameters.Add(new SqlParameter("@Rate", rate.Amount));
                        cmd.Parameters.Add(new SqlParameter("@IsDisabled", rate.isDisabled));
                        cmd.Parameters.Add(new SqlParameter("@RateID", rate.RateID));

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
