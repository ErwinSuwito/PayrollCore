﻿using System;
using System.Collections.Generic;
using System.Text;
using System.Data.SqlClient;
using System.Diagnostics;
using System.Collections.ObjectModel;
using System.Threading.Tasks;
using PayrollCore.Entities;

namespace PayrollCore
{
    public class Users
    {
        public string connString;
        public Users(string _connString)
        {
            connString = _connString;
        }
    }
}