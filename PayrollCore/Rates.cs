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
    public class Rates
    {
        public string connString;
        public Rates(string _connString)
        {
            connString = _connString;
        }
    }
}
