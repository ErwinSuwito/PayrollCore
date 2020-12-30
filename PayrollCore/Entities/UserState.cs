﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PayrollCore.Entities;

namespace PayrollCore.Entities
{
    public class UserState
    {
        public User User { get; set; }
        public Activity LatestActivity { get; set; }
        public Activity LatestMeeting { get; set; }
        public double ApprovedHours { get; set; }
    }
}
