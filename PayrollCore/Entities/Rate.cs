using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollCore.Entities
{
    public class Rate : INotifyPropertyChanged
    {
        public int RateID { get; set; }
        public string RateDesc { get; set; }
        public float Amount { get; set; }
        public bool isDisabled { get; set; }

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
