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
        public string UserID 
        {
            get
            {
                return UserID;
            } 
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    UserID = value;
                }
            }
        }
        public int LocationID { get; set; }
        public DateTime InTime { get; set; }
        public DateTime OutTime { get; set; }
        /// <summary>
        /// actualOutTime is used to temporarily store sign out time when 
        /// user signs out late. 
        /// </summary>
        public DateTime ActualOutTime { get; set; }
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
        // public int NotificationReason { get; set; }

        public NotifyReason NotificationReason { get; set; }

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

        /// <summary>
        /// Marks the activity as done and generates required additional data, then
        /// update the activity data in database and insert claim information when applicable.
        /// </summary>
        public void CompleteActivity()
        {
            ActualOutTime = DateTime.Now;
            CheckTimeAndCalcHours();
        }

        /// <summary>
        /// Checks the sign in, sign out times and compare them to shift start/end time.
        /// </summary>
        private void CheckTimeAndCalcHours()
        {
            if (meeting != null || (meeting == null && StartShift.ShiftName == "Normal sign in") || (meeting == null && IsSpecialTask == true))
            {
                // Activity is a meeting, shiftless or a special task
            }
            else
            {
                if (InTime.DayOfYear < DateTime.Now.DayOfYear)
                {
                    // User signs out late on another day
                    this.RequireNotification = true;
                    this.NotificationReason = NotifyReason.LateSignOut;
                }
                else if (ActualOutTime.TimeOfDay < StartShift.StartTime)
                {
                    // User signs out before shift start
                    this.OutTime = DateTime.Today + StartShift.StartTime;
                }
                else if (ActualOutTime.TimeOfDay < EndShift.EndTime)
                {
                    // User signs out early
                    this.RequireNotification = true;
                    this.NotificationReason = NotifyReason.EarlySignOut;
                    this.OutTime = ActualOutTime;
                }

                // Calculate break if total work hours exceed set settings
                // Gets the settings for BreakDuration and NeedBreakDuration
                Client.Instance.GlobalSettings.Settings.TryGetValue("BreakDuration", out string _breakDuration);
                Client.Instance.GlobalSettings.Settings.TryGetValue("NeedBreakDuration", out string _needBreakDuration);

                int breakDuration = 30;
                int needBreakDuration = 6;
                TimeSpan _approvedHours = InTime - OutTime;

                if (_breakDuration != null && _needBreakDuration != null)
                {
                    int.TryParse(_breakDuration, out breakDuration);
                    int.TryParse(_needBreakDuration, out needBreakDuration);
                }

                // Checks if the total minutes worked is over the needBreakDuration. If it exceeds, 
                // calculates the deduction needed and calculates the final hours approved.
                if (_approvedHours.TotalHours >= needBreakDuration)
                {
                    int removeTimes = (int)(_approvedHours.TotalHours / needBreakDuration);

                    for(int i = 0; i <= removeTimes; i++)
                    {
                        _approvedHours.Subtract(new TimeSpan(0, breakDuration, 0));
                    }
                }

                ApprovedHours = _approvedHours.TotalHours;
            }
        }
    }
}
