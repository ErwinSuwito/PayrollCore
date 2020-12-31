using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PayrollCore.Entities;
using System.Text.RegularExpressions;

namespace PayrollCore
{
    public class Cards
    {
        public Exception lastEx
        {
            get;
            private set;
        }

        public string connString;
        public Cards(string _connString)
        {
            connString = _connString;
        }

        /// <summary>
        /// Gets the username from the passed cardID
        /// </summary>
        /// <param name="cardId"></param>
        /// <returns></returns>
        public async Task<string> GetUserIdFromCard(string cardId)
        {
            string username = "";
            string pattern = @"\D\D\d\d\d\d\d";

            try
            {
                lastEx = null;

                string Query = "SELECT Name FROM CardDetail WHERE Badgenumber=@BadgeNumber";

                using (SqlConnection conn = new SqlConnection(connString))
                {
                    conn.Open();
                    using (SqlCommand cmd = conn.CreateCommand())
                    {
                        cmd.CommandText = Query;
                        cmd.Parameters.Add(new SqlParameter("@BadgeNumber", cardId));
                        using (SqlDataReader dr = await cmd.ExecuteReaderAsync())
                        {
                            while (dr.Read())
                            {
                                username = dr.GetString(0);
                            }

                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine("[DataAccess] Exception: " + ex.Message);
                lastEx = ex;
            }


            if (!string.IsNullOrEmpty(username))
            {
                RegexOptions options = RegexOptions.IgnoreCase | RegexOptions.Singleline;

                if (Regex.IsMatch(username, pattern, options))
                {
                    username += "@mail.apu.edu.my";
                }
                else
                {
                    username += "@cloudmails.apu.edu.my";
                }

                return username;
            }
            else
            {
                return null;
            }
        }

    }
}
