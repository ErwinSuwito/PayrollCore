using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace PayrollCore.Entities
{
    public class Meeting : INotifyPropertyChanged
    {
        public int MeetingID { get; set; }
        public int LocationID { get; set; }
        public string MeetingName { get; set; }
        public int MeetingDay { get; set; }
        public bool IsDisabled { get; set; }
        public bool IsNewMeeting { get; set; }
        public TimeSpan StartTime { get; set; }
        public Rate rate { get; set; }

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
