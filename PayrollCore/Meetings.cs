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
    public class Meetings : DataObject
    {
        public Meetings(string _connString)
        {
            connString = _connString;
        }

        /// <summary>
        /// Gets the requested meeting
        /// </summary>
        /// <param name="MeetingID"></param>
        /// <returns></returns>
        public async Task<Meeting> GetMeetingByIdAsync(int MeetingID)
        {
            string Query = "SELECT * FROM Meeting WHERE MeetingID=@MeetingID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@MeetingID", MeetingID));

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (dr.Read())
                            {
                                Meeting meeting = new Meeting();
                                meeting.MeetingID = dr.GetInt32(0);
                                meeting.MeetingName = dr.GetString(1);
                                meeting.LocationID = dr.GetInt32(2);
                                meeting.MeetingDay = dr.GetInt32(3);
                                meeting.IsDisabled = dr.GetBoolean(4);
                                meeting.rate = new Rate() { RateID = dr.GetInt32(5) };
                                meeting.StartTime = dr.GetTimeSpan(6);

                                return meeting;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Meetings] Exception: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Gets all meetings in the database
        /// </summary>
        /// <param name="GetDisabled"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<Meeting>> GetAllMeetingsAsync(bool GetDisabled)
        {
            lastEx = null;
            string Query = "SELECT * FROM Meeting";

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
                            ObservableCollection<Meeting> meetings = new ObservableCollection<Meeting>();

                            while (dr.Read())
                            {
                                Meeting meeting = new Meeting();
                                meeting.MeetingID = dr.GetInt32(0);
                                meeting.MeetingName = dr.GetString(1);
                                meeting.LocationID = dr.GetInt32(2);
                                meeting.MeetingDay = dr.GetInt32(3);
                                meeting.IsDisabled = dr.GetBoolean(4);
                                meeting.rate = new Rate() { RateID = dr.GetInt32(5) };
                                meeting.StartTime = dr.GetTimeSpan(6);

                                meetings.Add(meeting);
                            }

                            return meetings;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Meetings] Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets all the meetings on the specified location
        /// </summary>
        /// <param name="GetDisabled"></param>
        /// <param name="LocationID"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<Meeting>> GetAllMeetingsAsync(bool GetDisabled, int LocationID)
        {
            lastEx = null;
            string Query = "SELECT * FROM Meeting WHERE LocationID=@LocationID";

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
                            ObservableCollection<Meeting> meetings = new ObservableCollection<Meeting>();

                            while (dr.Read())
                            {
                                Meeting meeting = new Meeting();
                                meeting.MeetingID = dr.GetInt32(0);
                                meeting.MeetingName = dr.GetString(1);
                                meeting.LocationID = dr.GetInt32(2);
                                meeting.MeetingDay = dr.GetInt32(3);
                                meeting.IsDisabled = dr.GetBoolean(4);
                                meeting.rate = new Rate() { RateID = dr.GetInt32(5) };
                                meeting.StartTime = dr.GetTimeSpan(6);

                                meetings.Add(meeting);
                            }

                            return meetings;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Meetings] Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Gets meetings based on the selected location, user group and meeting day
        /// </summary>
        /// <param name="LocationID"></param>
        /// <param name="userGroupId"></param>
        /// <param name="MeetingDay"></param>
        /// <returns></returns>
        public async Task<ObservableCollection<Meeting>> GetMeetingsAsync(int LocationID, int userGroupId, int MeetingDay, bool GetDisabled)
        {
            lastEx = null;
            string Query = "SELECT * FROM Meeting JOIN Meeting_Group ON Meeting_Group.MeetingID = Meeting.MeetingID WHERE LocationID=@LocationID AND Meeting_Group.UserGroupID=@UserGroupID AND MeetingDay=@MeetingDay";

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
                        cmd.Parameters.Add(new SqlParameter("@UserGroupID", userGroupId));
                        cmd.Parameters.Add(new SqlParameter("@MeetingDay", MeetingDay));

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {

                            ObservableCollection<Meeting> meetings = new ObservableCollection<Meeting>();

                            while (dr.Read())
                            {
                                Meeting meeting = new Meeting();
                                meeting.MeetingID = dr.GetInt32(0);
                                meeting.MeetingName = dr.GetString(1);
                                meeting.LocationID = dr.GetInt32(2);
                                meeting.MeetingDay = dr.GetInt32(3);
                                meeting.IsDisabled = dr.GetBoolean(4);
                                meeting.rate = new Rate() { Amount = dr.GetInt32(5) };
                                meeting.StartTime = dr.GetTimeSpan(6);

                                meetings.Add(meeting);
                            }

                            return meetings;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Meetings] Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Adds the new meeting and required MeetingUserGroup to database.
        /// </summary>
        /// <param name="meeting"></param>
        /// <param name="meetingUserGroups"></param>
        /// <returns></returns>
        public async Task<bool> AddMeetingAsync(Meeting meeting, List<MeetingUserGroup> meetingUserGroups)
        {
            lastEx = null;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                SqlCommand cmd = conn.CreateCommand();
                SqlTransaction transaction = conn.BeginTransaction("Add Meeting");

                cmd.Connection = conn;
                cmd.Transaction = transaction;

                try
                {
                    cmd.CommandText = "INSERT INTO Meeting(MeetingName, LocationID, MeetingDay, RateID, StartTime) VALUES(@MeetingName, @LocationID, @MeetingDay, @RateID, @StartTime) select SCOPE_IDENTITY()";
                    cmd.Parameters.Add(new SqlParameter("@MeetingName", meeting.MeetingName));
                    cmd.Parameters.Add(new SqlParameter("@LocationID", meeting.LocationID));
                    cmd.Parameters.Add(new SqlParameter("@MeetingDay", meeting.MeetingDay));
                    cmd.Parameters.Add(new SqlParameter("@RateID", meeting.rate.RateID));
                    cmd.Parameters.Add(new SqlParameter("@StartTime", meeting.StartTime));

                    var _MeetingID = await cmd.ExecuteScalarAsync();
                    int.TryParse(_MeetingID.ToString(), out int MeetingID);

                    cmd.CommandText = "INSERT INTO Meeting_Group(MeetingID, UserGroupID) VALUES(@MeetingID, @UserGroupID)";
                    foreach (MeetingUserGroup group in meetingUserGroups)
                    {
                        group.MeetingID = MeetingID;
                        cmd.Parameters.Add(new SqlParameter("@MeetingID", group.MeetingID));
                        cmd.Parameters.Add(new SqlParameter("@UserGroupID", group.UserGroupId));
                        await cmd.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    Debug.WriteLine("[Meetings] Exception: " + ex.Message);
                    
                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception _ex)
                    {
                        Debug.WriteLine("[Meetings] Exception caught while commiting transaction: " + ex.GetType());
                        Debug.WriteLine(ex.Message);
                        lastEx = _ex;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Disables the passed meeting
        /// </summary>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public async Task<bool> DisableMeetingAsync(Meeting meeting)
        {
            meeting.IsDisabled = true;
            return await _UpdateMeetingAsync(meeting);
        }

        /// <summary>
        /// Updates the passed Meeting and MeetingUserGroup
        /// </summary>
        /// <param name="meeting"></param>
        /// <param name="meetingUserGroups"></param>
        /// <returns></returns>
        public async Task<bool> UpdateMeetingAsync(Meeting meeting, List<MeetingUserGroup> meetingUserGroups)
        {
            lastEx = null;

            using (SqlConnection conn = new SqlConnection(connString))
            {
                conn.Open();

                SqlCommand cmd = conn.CreateCommand();
                SqlTransaction transaction = conn.BeginTransaction("Update Meeting");

                cmd.Connection = conn;
                cmd.Transaction = transaction;

                try
                {
                    cmd.CommandText = "UPDATE Meeting SET MeetingName=@MeetingName, LocationID=@LocationID, MeetingDay=@MeetingDay, IsDisabled=@IsDisabled, RateID=@RateID, StartTime=@StartTime WHERE MeetingID=@MeetingID";
                    cmd.Parameters.Add(new SqlParameter("@MeetingName", meeting.MeetingName));
                    cmd.Parameters.Add(new SqlParameter("@LocationID", meeting.LocationID));
                    cmd.Parameters.Add(new SqlParameter("@MeetingDay", meeting.MeetingDay));
                    cmd.Parameters.Add(new SqlParameter("@RateID", meeting.rate.RateID));
                    cmd.Parameters.Add(new SqlParameter("@StartTime", meeting.StartTime));
                    cmd.Parameters.Add(new SqlParameter("@IsDisabled", meeting.IsDisabled));
                    cmd.Parameters.Add(new SqlParameter("@MeetingID", meeting.MeetingID));
                    await cmd.ExecuteNonQueryAsync();

                    cmd.CommandText = "UPDATE Meeting_Group SET MeetingID=@MeetingID, UserGroupID=@UserGroupID)";
                    foreach (MeetingUserGroup group in meetingUserGroups)
                    {
                        cmd.Parameters.Add(new SqlParameter("@MeetingID", group.MeetingID));
                        cmd.Parameters.Add(new SqlParameter("@UserGroupID", group.UserGroupId));
                        await cmd.ExecuteNonQueryAsync();
                    }

                    transaction.Commit();
                    return true;
                }
                catch (Exception ex)
                {
                    lastEx = ex;
                    Debug.WriteLine("[Meetings] Exception caught while commiting transaction: " + ex.GetType());
                    Debug.WriteLine(ex.Message);

                    try
                    {
                        transaction.Rollback();
                    }
                    catch (Exception _ex)
                    {
                        Debug.WriteLine("[Meetings] Exception caught while rolling back transaction: " + _ex.GetType());
                        Debug.WriteLine(ex.Message);
                        lastEx = _ex;
                    }

                    return false;
                }
            }
        }

        /// <summary>
        /// Deletes the meeting in the database if there is no user ever signed in 
        /// for the meeting. Disables the meeting otherwise.
        /// </summary>
        /// <param name="meeting"></param>
        /// <returns></returns>
        public async Task<bool> DeleteMeetingAsync(Meeting meeting)
        {
            int count = await Client.Instance.Activities.CountMeetingAsync(meeting.MeetingID);
            if (count > 0)
            {
                return await DisableMeetingAsync(meeting);
            }
            else
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();

                    SqlCommand cmd = conn.CreateCommand();
                    SqlTransaction transaction = conn.BeginTransaction("Delete Meeting");

                    cmd.Connection = conn;
                    cmd.Transaction = transaction;

                    try
                    {
                        cmd.CommandText = "DELETE FROM Meeting_Group WHERE MeetingID=@MeetingId";
                        cmd.Parameters.Add(new SqlParameter("@MeetingId", meeting.MeetingID));
                        await cmd.ExecuteNonQueryAsync();

                        cmd.CommandText = "DELETE FROM Meeting WHERE MeetingID=@MeetingId";
                        await cmd.ExecuteNonQueryAsync();

                        transaction.Commit();
                        return true;
                    }
                    catch (Exception ex)
                    {
                        lastEx = ex;
                        Debug.WriteLine("[Meetings] Exception caught while commiting transaction: " + ex.GetType());
                        Debug.WriteLine(ex.Message);

                        try
                        {
                            transaction.Rollback();
                        }
                        catch (Exception _ex)
                        {
                            lastEx = ex;
                            Debug.WriteLine("[Meetings] Exception caught while rolling back transaction: " + _ex.GetType());
                            Debug.WriteLine(_ex.Message);
                        }

                        return false;
                    }
                }
            }
        }
        
        /// <summary>
        /// Gets the MeetingGroup objects belonging to the passed MeetingID
        /// </summary>
        /// <param name="meetingID"></param>
        /// <returns></returns>
        public async Task<List<MeetingUserGroup>> GetMeetingGroupAsync(int meetingID)
        {
            lastEx = null;
            string Query = "SELECT * FROM Meeting_Group WHERE MeetingID=@MeetingID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@MeetingID", meetingID));

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            List<MeetingUserGroup> meetingGroups = new List<MeetingUserGroup>();

                            while (dr.Read())
                            {
                                MeetingUserGroup meetingGroup = new MeetingUserGroup();
                                meetingGroup.meeting_group_id = dr.GetInt32(0);
                                meetingGroup.MeetingID = dr.GetInt32(1);
                                meetingGroup.UserGroupId = dr.GetInt32(2);

                                meetingGroups.Add(meetingGroup);
                            }

                            return meetingGroups;
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
        /// Internal method to update a meeting
        /// </summary>
        /// <param name="meeting"></param>
        /// <returns></returns>
        protected async Task<bool> _UpdateMeetingAsync(Meeting meeting)
        {
            lastEx = null;

            string Query = "UPDATE Meeting SET MeetingName=@MeetingName, LocationID=@LocationID, MeetingDay=@MeetingDay, IsDisabled=@IsDisabled, RateID=@RateID, StartTime=@StartTime WHERE MeetingID=@MeetingID";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@MeetingName", meeting.MeetingName));
                        cmd.Parameters.Add(new SqlParameter("@LocationID", meeting.LocationID));
                        cmd.Parameters.Add(new SqlParameter("@MeetingDay", meeting.MeetingDay));
                        cmd.Parameters.Add(new SqlParameter("@RateID", meeting.rate.RateID));
                        cmd.Parameters.Add(new SqlParameter("@StartTime", meeting.StartTime));
                        cmd.Parameters.Add(new SqlParameter("@IsDisabled", meeting.IsDisabled));
                        cmd.Parameters.Add(new SqlParameter("@MeetingID", meeting.MeetingID));

                        await cmd.ExecuteNonQueryAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Meetings] Exception: " + ex.Message);
                return false;
            }
        }

    }
}
