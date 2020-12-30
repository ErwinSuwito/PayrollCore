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

        /// <summary>
        /// Gets the activity by its ID.
        /// </summary>
        /// <param name="ActivityID"></param>
        /// <returns></returns>
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
                                activity.UserID = dr.GetString(1);
                                activity.LocationID = dr.GetInt32(2);
                                activity.InTime = dr.GetDateTime(3);

                                if (!dr.IsDBNull(4))
                                {
                                    activity.OutTime = dr.GetDateTime(4);
                                }

                                if (!dr.IsDBNull(5))
                                {
                                    activity.StartShift = new Shift() { ShiftID = dr.GetInt32(5) };
                                }

                                if (!dr.IsDBNull(6))
                                {
                                    activity.EndShift = new Shift() { ShiftID = dr.GetInt32(6) };
                                }

                                if (!dr.IsDBNull(7))
                                {
                                    activity.meeting = new Meeting() { MeetingID = dr.GetInt32(7) };
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


        /// <summary>
        /// Adds a new activity to the database.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="IsUserLoggedIn"></param>
        /// <param name="IsPartOfRoster"></param>
        /// <returns></returns>
        public async Task<int> AddActivityAsync(Activity activity)
        {
            lastEx = null;
            string Query;

            // Checks if the passed activity is a meeting.
            if (activity.meeting == null)
            {
                // Activity is not a meeting
                Query = "INSERT INTO Activity(UserID, LocationID, InTime, StartShift, EndShift, SpecialTask, HasLoggedIn, PartOfRoster)" +
                    " VALUES(@UserID, @LocationID, @InTime, @StartShift, @EndShift, @SpecialTask, @HasLoggedIn, @PartOfRoster)";
            }
            else
            {
                // Activity is a meeting
                Query = "INSERT INTO Activity(UserID, LocationID, InTime, MeetingID)" +
                    " VALUES(@UserID, @LocationID, @InTime, @MeetingID)";
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@UserID", activity.UserID));
                        cmd.Parameters.Add(new SqlParameter("@LocationID", activity.LocationID));
                        cmd.Parameters.Add(new SqlParameter("@InTime", activity.InTime));
                        cmd.Parameters.Add(new SqlParameter("@HasLoggedIn", activity.HasLoggedIn));
                        cmd.Parameters.Add(new SqlParameter("@PartOfRoster", activity.PartOfRoster));

                        if (activity.meeting != null)
                        {
                            cmd.Parameters.Add(new SqlParameter("@MeetingID", activity.meeting.MeetingID));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@StartShift", activity.StartShift.ShiftID));
                            cmd.Parameters.Add(new SqlParameter("@EndShift", activity.EndShift.ShiftID));
                            cmd.Parameters.Add(new SqlParameter("@SpecialTask", activity.IsSpecialTask));
                        }

                        var result = await cmd.ExecuteScalarAsync();

                        int.TryParse(result.ToString(), out int ActivityID);
                        return ActivityID;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Activities] Failed to add Activity. Exception caught.");
                Debug.WriteLine(ex.Message);
                return -1;
            }
        }

        /// <summary>
        /// Updates the specified activity
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public async Task<bool> UpdateActivityAsync(Activity activity)
        {
            lastEx = null;
            string Query;

            if (activity.meeting == null)
            {
                Query = "UPDATE Activity SET UserID=@UserID, LocationID=@LocationID, InTime=@InTime, OutTime=@OutTime, MeetingID=@MeetingID, ApprovedHours=@ApprovedHours, HasLoggedIn=@HasLoggedIn, PartOfRoster=@PartOfRoster WHERE ActivityID=@ActivityID";
            }
            else
            {
                Query = "UPDATE Activity SET UserID=@UserID, LocationID=@LocationID, InTime=@InTime, OutTime=@OutTime, StartShift=@StartShift, EndShift=@EndShift, SpecialTask=@SpecialTask, ApprovedHours=@ApprovedHours, HasLoggedIn=@HasLoggedIn, PartOfRoster=@PartOfRoster WHERE ActivityID=@ActivityID";
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@ActivityID", activity.ActivityID));
                        cmd.Parameters.Add(new SqlParameter("@UserID", activity.UserID));
                        cmd.Parameters.Add(new SqlParameter("@LocationID", activity.LocationID));
                        cmd.Parameters.Add(new SqlParameter("@InTime", activity.InTime));
                        cmd.Parameters.Add(new SqlParameter("@OutTime", activity.OutTime));
                        cmd.Parameters.Add(new SqlParameter("@ApprovedHours", activity.ApprovedHours));
                        cmd.Parameters.Add(new SqlParameter("@HasLoggedIn", activity.HasLoggedIn));
                        cmd.Parameters.Add(new SqlParameter("@PartOfRoster", activity.PartOfRoster));

                        if (activity.meeting != null)
                        {
                            cmd.Parameters.Add(new SqlParameter("@MeetingID", activity.meeting.MeetingID));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@StartShift", activity.StartShift.ShiftID));
                            cmd.Parameters.Add(new SqlParameter("@EndShift", activity.EndShift.ShiftID));
                            cmd.Parameters.Add(new SqlParameter("@SpecialTask", activity.IsSpecialTask));
                        }

                        await cmd.ExecuteNonQueryAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Activities] Exception: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates the specified activity and inserts Claim to the database.
        /// </summary>
        /// <param name="activity"></param>
        /// <param name="claim"></param>
        /// <returns></returns>
        public async Task<bool> UpdateActivityAsync(Activity activity, Claim claim)
        {
            bool IsUpdateSuccess = await UpdateActivityAsync(activity);

            if (IsUpdateSuccess == true)
            {
                IsUpdateSuccess = await Client.Instance.Claims.AddClaimsAsync(claim);
            }

            return IsUpdateSuccess;
        }

        /// <summary>
        /// Deletes the activity by their passed activity id
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public async Task<bool> DeleteActivityAsync(int activityId)
        {
            string Query = "DELETE FROM Activity WHERE ActivityID=@ActivityID";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@ActivityID", activityId));

                        await cmd.ExecuteNonQueryAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Activities] Exception: " + ex.Message);
                return false;
            }
        }

    }
}
