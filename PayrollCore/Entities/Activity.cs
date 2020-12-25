using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollCore.Entities
{
    /// <summary>
    /// An activity is any user sign in for shift, meeting or special task.
    /// Roster items are stored as activity, with PartOfRoster set to true, 
    /// and HasLoggedIn depends on whether user has signed in for that shift or not.
    /// </summary>
    public class Activity : INotifyPropertyChanged
    {
        public int ActivityID { get; set; }
        public string userID 
        {
            get
            {
                return userID;
            } 
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    userID = value;
                }
            }
        }
        public int locationID { get; set; }
        public DateTime inTime { get; set; }
        public DateTime outTime { get; set; }
        /// <summary>
        /// actualOutTime is used to temporarily store sign out time when 
        /// user signs out late. 
        /// </summary>
        public DateTime actualOutTime { get; set; }
        public Shift StartShift { get; set; }
        public Shift EndShift { get; set; }
        /// <summary>
        /// The meeting the user is attending. Null if activity is not a meeting.
        /// </summary>
        public Meeting meeting { get; set; }
        /// <summary>
        /// Identifies if the activity is a special task. True if special task.
        /// </summary>
        public bool IsSpecialTask { get; set; }
        public double ApprovedHours { get; set; }
        public bool RequireNotification { get; set; }
        /// <summary>
        /// Helper property to know if user has logged in
        /// </summary>
        public bool HasLoggedIn { get; set; }
        /// <summary>
        /// Helper property if activity is in the roster.
        /// </summary>
        public bool PartOfRoster { get; set; }
        public bool NoActivity { get; set; }

        /// <summary>
        /// The reason to send a notification
        /// 1 - Late sign in
        /// 2 - Late sign out
        /// </summary>
        public int NotificationReason { get; set; }

        public NotifyReason notifyReason { get; set; }

        public enum NotifyReason
        {
            LateSignIn,
            LateSignOut,
            EarlySignOut
        }

        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyEventChanged(string propertyName)
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
