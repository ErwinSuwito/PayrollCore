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
    public class UserGroups : DataObject
    {
        public UserGroups(string _connString)
        {
            connString = _connString;
        }

        /// <summary>
        /// Gets the requested user group
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public async Task<UserGroup> GetUserGroupAsync(int GroupID)
        {
            string Query = "SELECT * FROM user_group WHERE GroupID=@GroupID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@GroupID", GroupID));

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (dr.Read())
                            {
                                UserGroup userGroup = new UserGroup();
                                userGroup.GroupID = dr.GetInt32(0);
                                userGroup.GroupName = dr.GetString(1);
                                userGroup.DefaultRate = new Rate() { RateID = dr.GetInt32(2) };
                                userGroup.ShowAdminSettings = dr.GetBoolean(3);
                                userGroup.EnableFaceRec = dr.GetBoolean(4);

                                return userGroup;
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
        /// Gets all user groups and returns in an ObservableCollection
        /// </summary>
        /// <param name="GetDisabled">True to include disabled user groups</param>
        /// <returns></returns>
        public async Task<ObservableCollection<UserGroup>> GetUserGroupsAsync(bool GetDisabled)
        {
            lastEx = null;
            string Query = "SELECT * FROM user_group";

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
                            ObservableCollection<UserGroup> userGroups = new ObservableCollection<UserGroup>();

                            while (dr.Read())
                            {
                                UserGroup userGroup = new UserGroup();
                                userGroup.GroupID = dr.GetInt32(0);
                                userGroup.GroupName = dr.GetString(1);
                                userGroup.DefaultRate = new Rate() { RateID = dr.GetInt32(2) };
                                userGroup.ShowAdminSettings = dr.GetBoolean(3);
                                userGroup.EnableFaceRec = dr.GetBoolean(4);

                                userGroups.Add(userGroup);
                            }

                            return userGroups;
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
        /// Adds a new user group
        /// </summary>
        /// <param name="userGroup"></param>
        /// <returns></returns>
        public async Task<bool> AddUserGroupAsync(UserGroup userGroup)
        {
            lastEx = null;

            string Query = "INSERT INTO user_group(GroupName, RateID, ShowAdminSettings, EnableFaceRect) VALUES(@GroupID, @GroupName, @RateID, @ShowAdminSettings, @EnableFaceRec)";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@GroupID", userGroup.GroupID));
                        cmd.Parameters.Add(new SqlParameter("@GroupName", userGroup.GroupName));
                        cmd.Parameters.Add(new SqlParameter("@RateID", userGroup.DefaultRate.RateID));
                        cmd.Parameters.Add(new SqlParameter("@ShowAdminSettings", userGroup.ShowAdminSettings));
                        cmd.Parameters.Add(new SqlParameter("@EnableFaceRec", userGroup.EnableFaceRec));

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
        /// Deletes the specified user group from the database
        /// </summary>
        /// <param name="userGroup"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUserGroupAsync(UserGroup userGroup)
        {
            string Query = "DELETE FROM user_group WHERE GroupID=@GroupID";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@GroupID", userGroup.GroupID));

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
        public async Task<bool> UpdateUserGroupAsync(UserGroup userGroup)
        {
            lastEx = null;

            string Query = "UPDATE user_group SET GroupName=@GroupName, RateID=@RateID, ShowAdminSettings=@ShowAdminSettings, EnableFaceRec=@EnableFaceRec, IsDisabled=@IsDisabled WHERE GroupID=@GroupID";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@GroupName", userGroup.GroupName));
                        cmd.Parameters.Add(new SqlParameter("@RateID", userGroup.DefaultRate.RateID));
                        cmd.Parameters.Add(new SqlParameter("@ShowAdminSettings", userGroup.ShowAdminSettings));
                        cmd.Parameters.Add(new SqlParameter("@EnableFaceRec", userGroup.EnableFaceRec));
                        cmd.Parameters.Add(new SqlParameter("@IsDisabled", userGroup.IsDisabled));
                        cmd.Parameters.Add(new SqlParameter("@GroupID", userGroup.GroupID));

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
