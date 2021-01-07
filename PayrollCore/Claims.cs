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
    public class Claims : DataObject
    {
        public Claims(string _connString)
        {
            connString = _connString;
        }

        /// <summary>
        /// Adds a new claim to the dabatase
        /// </summary>
        /// <param name="claim"></param>
        /// <returns></returns>
        public async Task<bool> AddClaimsAsync(Claim claim)
        {
            lastEx = null;

            string Query = "INSERT INTO Claims(ClaimType, ClaimableAmount, ApplicableRate, ClaimDate, ActivityId) VALUES(@ClaimType, @ClaimableAmount, @ApplicableRate, GETDATE(), @ActivityId)";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@ClaimType", claim.ClaimType));
                        cmd.Parameters.Add(new SqlParameter("@ClaimableAmount", claim.ClaimableAmount));
                        cmd.Parameters.Add(new SqlParameter("@ApplicableRate", claim.ApplicableRate.RateID));
                        cmd.Parameters.Add(new SqlParameter("@ActivityId", claim.ActivityID));
                        
                        await cmd.ExecuteNonQueryAsync();
                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Claims] Exception: " + ex.Message);
                return false;
            }
        }
    }
}
