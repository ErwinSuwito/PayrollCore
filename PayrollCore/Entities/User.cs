using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PayrollCore.Entities
{
    public class User : INotifyPropertyChanged
    {
        public string UserID { get; set; }
        public string FullName { get; set; }
        public bool IsFromAD { get; set; }
        public bool IsDisabled { get; set; }
        public UserGroup UserGroup { get; set; }
        public bool IsNewUser { get; set; }

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
