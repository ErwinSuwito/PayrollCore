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
    public class Activities
    {
        public string connString;
        public Activities(string _connString)
        {
            connString = _connString;
        }
    }
}
