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
    public class Activities
    {
        public Exception lastEx
        {
            get;
            private set;
        }

        public string connString;
        public Activities(string _connString)
        {
            connString = _connString;
        }

        public async Task<Activity> GetActivityAsync(int ActivityID)
        {
            string Query = "SELECT * FROM Activity WHERE ActivityID=@ActivityID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@ActivityID", ActivityID));

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (dr.Read())
                            {
                                Activity activity = new Activity();
                                activity.ActivityID = dr.GetInt32(0);
                                activity.userID = dr.GetString(1);
                                activity.locationID = dr.GetInt32(2);
                                activity.inTime = dr.GetDateTime(3);

                                if (!dr.IsDBNull(4))
                                {
                                    activity.outTime = dr.GetDateTime(4);
                                }

                                if (!dr.IsDBNull(5))
                                {
                                    activity.StartShift = new Shift() { shiftID = dr.GetInt32(5) };
                                }

                                if (!dr.IsDBNull(6))
                                {
                                    activity.EndShift = new Shift() { shiftID = dr.GetInt32(6) };
                                }

                                if (!dr.IsDBNull(7))
                                {
                                    activity.meeting = new Meeting() { meetingID = dr.GetInt32(7) };
                                }

                                activity.IsSpecialTask = dr.GetBoolean(8);

                                if (!dr.IsDBNull(9))
                                {
                                    activity.ApprovedHours = dr.GetDouble(9);
                                    activity.HasLoggedIn = dr.GetBoolean(10);
                                    activity.PartOfRoster = dr.GetBoolean(11);
                                }

                                return activity;
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
    }
}
