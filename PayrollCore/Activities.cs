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

        /// <summary>
        /// Adds a new activity to the database and returns the new activity ID.
        /// </summary>
        /// <param name="activity"></param>
        /// <returns></returns>
        public async Task<int> AddActivityAsync(Activity activity, Claim claim)
        {
            lastEx = null;
            int activityId = -1;

            string Query = "BEGIN TRANSACTION ";

            if (activity.meeting != null)
            {
                Query = "INSERT INTO Activity(UserID, LocationID, InTime, OutTime, MeetingID, ApprovedHours) VALUES(@UserID, @LocationID, @InTime, @OutTime, @StartShift, @EndShift, @MeetingID, @ApprovedHours)";
            }
            else
            {
                Query = "INSERT INTO Activity(UserID, LocationID, InTime, OutTime, StartShift, EndShift, SpecialTask, ApprovedHours) VALUES(@UserID, @LocationID, @InTime, @OutTime, @StartShift, @EndShift, @SpecialTask)";
            }

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        SqlTransaction transaction = conn.BeginTransaction();

                        cmd.Connection = conn;
                        cmd.Transaction = transaction;

                        try
                        {
                            cmd.CommandText = Query;
                            cmd.Parameters.Add(new SqlParameter("@UserID", activity.userID));
                            cmd.Parameters.Add(new SqlParameter("@LocationID", activity.locationID));
                            cmd.Parameters.Add(new SqlParameter("@InTime", activity.inTime));
                            cmd.Parameters.Add(new SqlParameter("@OutTime", activity.outTime));
                            cmd.Parameters.Add(new SqlParameter("@ApprovedHours", activity.ApprovedHours));

                            if (activity.meeting != null)
                            {
                                cmd.Parameters.Add(new SqlParameter("@MeetingID", activity.meeting.meetingID));
                            }
                            else
                            {
                                cmd.Parameters.Add(new SqlParameter("@StartShift", activity.StartShift.shiftID));
                                cmd.Parameters.Add(new SqlParameter("@EndShift", activity.EndShift.shiftID));
                                cmd.Parameters.Add(new SqlParameter("@SpecialTask", activity.IsSpecialTask));
                            }

                            var _activityId = await cmd.ExecuteScalarAsync();
                            int.TryParse(_activityId.ToString(), out int activityID);

                            cmd.CommandText = "INSERT INTO Claims(ClaimType, ClaimableAmount, ApplicableRate, ClaimDate, ActivityId) VALUES ('Work', @ClaimableAmount, @ApplicableRate, GETDATE(), @ActivityID)";
                            cmd.Parameters.Add(new SqlParameter("@ClaimableAmount", claim.ClaimableAmount));
                            cmd.Parameters.Add(new SqlParameter("@ApplicableRate", claim.ApplicableRate.rateID));
                            cmd.Parameters.Add(new SqlParameter("@ActivityID", activityID));

                            int AffectedRows = await cmd.ExecuteNonQueryAsync();
                            if (AffectedRows == 1)
                            {
                                transaction.Commit();
                            }
                        }
                        catch (Exception ex)
                        {
                            lastEx = ex;
                            Debug.WriteLine("[Activities] Transaction Exception: " + ex.Message);
                            transaction.Rollback();
                            Debug.WriteLine("[Activities] Transaction rollbacked.");
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Activities] Exception: " + ex.Message);
            }

            return activityId;
        }

        public async Task<int> AddActivityAsync(Activity activity)
        {
            return -1;
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

            if (activity.meeting != null)
            {
                Query = "UPDATE Activity SET UserID=@UserID, LocationID=@LocationID, InTime=@InTime, OutTime=@OutTime, MeetingID=@MeetingID, ApprovedHours=@ApprovedHours WHERE ActivityID=@ActivityID";
            }
            else
            {
                Query = "UPDATE Activity SET UserID=@UserID, LocationID=@LocationID, InTime=@InTime, OutTime=@OutTime, StartShift=@StartShift, EndShift=@EndShift, SpecialTask=@SpecialTask, ApprovedHours=@ApprovedHours WHERE ActivityID=@ActivityID";
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
                        cmd.Parameters.Add(new SqlParameter("@UserID", activity.userID));
                        cmd.Parameters.Add(new SqlParameter("@LocationID", activity.locationID));
                        cmd.Parameters.Add(new SqlParameter("@InTime", activity.inTime));
                        cmd.Parameters.Add(new SqlParameter("@OutTime", activity.outTime));
                        cmd.Parameters.Add(new SqlParameter("@ApprovedHours", activity.ApprovedHours));

                        if (activity.meeting != null)
                        {
                            cmd.Parameters.Add(new SqlParameter("@MeetingID", activity.meeting.meetingID));
                        }
                        else
                        {
                            cmd.Parameters.Add(new SqlParameter("@StartShift", activity.StartShift.shiftID));
                            cmd.Parameters.Add(new SqlParameter("@EndShift", activity.EndShift.shiftID));
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

        public async Task<bool> UpdateActivityAsync(Activity activity, Claim claim)
        {
            return false;
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
