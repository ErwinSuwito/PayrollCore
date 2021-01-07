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
    public class Users : DataObject
    {
        public Users(string _connString)
        {
            connString = _connString;
        }

        /// <summary>
        /// Gets the requested user
        /// </summary>
        /// <param name="GroupID"></param>
        /// <returns></returns>
        public async Task<User> GetUserAsync(string userID)
        {
            string Query = "SELECT * FROM Users WHERE UserID=@UserID";

            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@UserID", userID));

                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (dr.Read())
                            {
                                User user = new User();
                                user.UserID = dr.GetString(0);
                                user.FullName = dr.GetString(1);
                                user.IsFromAD = dr.GetBoolean(2);
                                user.IsDisabled = dr.GetBoolean(3);
                                user.UserGroup = new UserGroup() { GroupID = dr.GetInt32(4) };

                                return user;
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Users] Exception: " + ex.Message);
            }

            return null;
        }

        /// <summary>
        /// Gets all users and returns in an ObservableCollection
        /// </summary>
        /// <param name="GetDisabled">True to include disabled user groups</param>
        /// <returns></returns>
        public async Task<ObservableCollection<User>> GetUsersAsync(bool GetDisabled)
        {
            lastEx = null;
            string Query = "SELECT * FROM Users";

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
                            ObservableCollection<User> users = new ObservableCollection<User>();

                            while (dr.Read())
                            {
                                User user = new User();
                                user.UserID = dr.GetString(0);
                                user.FullName = dr.GetString(1);
                                user.IsFromAD = dr.GetBoolean(2);
                                user.IsDisabled = dr.GetBoolean(3);
                                user.UserGroup = new UserGroup() { GroupID = dr.GetInt32(4) };

                                users.Add(user);
                            }

                            return users;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Users] Exception: " + ex.Message);
                return null;
            }
        }

        /// <summary>
        /// Adds a new user group
        /// </summary>
        /// <param name="userGroup"></param>
        /// <returns></returns>
        public async Task<bool> AddUserAsync(User user)
        {
            lastEx = null;

            string Query = "INSERT INTO Users(UserID, FullName, FromAD, GroupID) VALUES(@UserID, @FullName, @FromAD, @GroupID)";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@UserID", user.UserID));
                        cmd.Parameters.Add(new SqlParameter("@FullName", user.FullName));
                        cmd.Parameters.Add(new SqlParameter("@FromAD", user.IsFromAD));
                        cmd.Parameters.Add(new SqlParameter("@GroupID", user.UserGroup.GroupID));

                        await cmd.ExecuteNonQueryAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Users] Exception: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Deletes the specified user group from the database
        /// </summary>
        /// <param name="userGroup"></param>
        /// <returns></returns>
        public async Task<bool> DeleteUserAsync(User user)
        {
            string Query = "DELETE FROM Users WHERE UserID=@UserID";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@UserID", user.UserID));

                        await cmd.ExecuteNonQueryAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Users] Exception: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Updates the specified rate
        /// </summary>
        /// <param name="rate">The rate to be updated</param>
        /// <returns></returns>
        public async Task<bool> UpdateUserAsync(User user)
        {
            lastEx = null;

            string Query = "UPDATE Users SET FullName=@FullName, FromAD=@FromAD, IsDisabled=@IsDisabled, GroupID=@GroupID WHERE UserID=@UserID";
            try
            {
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@UserID", user.UserID));
                        cmd.Parameters.Add(new SqlParameter("@FullName", user.FullName));
                        cmd.Parameters.Add(new SqlParameter("@FromAD", user.IsFromAD));
                        cmd.Parameters.Add(new SqlParameter("@GroupID", user.UserGroup.GroupID));
                        cmd.Parameters.Add(new SqlParameter("@IsDisabled", user.UserGroup.IsDisabled));

                        await cmd.ExecuteNonQueryAsync();

                        return true;
                    }
                }
            }
            catch (Exception ex)
            {
                lastEx = ex;
                Debug.WriteLine("[Users] Exception: " + ex.Message);
                return false;
            }
        }

    }
}
