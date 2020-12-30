using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PayrollCore.Entities
{
    public class MeetingUserGroup : INotifyPropertyChanged
    {
        public int meeting_group_id { get; set; }
        public int MeetingID { get; set; }
        public int UserGroupId { get; set; }
        public string MeetingName { get; set; }
        public TimeSpan StartTime { get; set; }


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
