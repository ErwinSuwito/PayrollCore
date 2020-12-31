using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayrollCore.Entities;

namespace PayrollCore
{
    public class UserState
    {
        public User User { get; set; }
        public Activity LatestActivity { get; set; }
        public Activity LatestMeeting { get; set; }
        public double ApprovedHours { get; set; }

        public async Task<bool> Login(string userId)
        {
            User user = await Client.Instance.Users.GetUserByIdAsync(userId);
            if (user == null)
            {
                return false;
            }
            else
            {
                this.User = user;
            }

            LatestActivity = await Client.Instance.Activities.GetLatestActivity(this.User.UserID, Client.Instance.LocationId, true);
            LatestMeeting = await Client.Instance.Activities.GetLatestActivity(this.User.UserID, Client.Instance.LocationId, false);

            return true;
        }

        public void Logout()
        {
            User = null;
            LatestActivity = null;
            LatestMeeting = null;
        }
    }
}
