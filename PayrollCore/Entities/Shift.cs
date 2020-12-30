using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollCore.Entities
{
    public class Shift : INotifyPropertyChanged
    {
        public int ShiftID { get; set; }
        public string ShiftName { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public int LocationID { get; set; }
        public Rate DefaultRate { get; set; }
        public bool selected { get; set; }
        public bool IsDisabled { get; set; }
        public string dg_isDisabled { get; set; }
        public bool WeekendOnly { get; set; }

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
